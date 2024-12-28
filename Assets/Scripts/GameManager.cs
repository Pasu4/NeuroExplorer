using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [HideInInspector]
        public string startPath;
        public bool obfuscate;

        public FileSprite[] fileSprites;
        public Sprite[] defaultSprites;

        private SHA1 sha1 = SHA1.Create();
        private Regex allowedFolderPathRegex;

        private const string obfuscateLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        private void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {
            startPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            allowedFolderPathRegex = new Regex(@"
                C:(?:
                    \\Windows
                    | \\Users (?:
                        \\\w+ (?:
                            \\source (?:
                                \\repos
                            )?
                            | \\Downloads
                            | \\Games
                            | \\Videos
                            | (?:\\OneDrive)? (?: # OneDrive folder may or may not be above these
                                \\Desktop
                                | \\Music
                                | \\Documents
                            )
                            | \\AppData (?:
                                \\Local
                                | \\LocalLow
                                | \\Roaming
                            )?
                        )
                    )?
                    | \\Program\sFiles
                    | \\Program\sFiles\s\(x86\)
                    | \\Temp
                )
            ", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public System.Random CreatePathRandom(string path, string tag)
        {
            string seed = path + ':' + tag;

            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(seed));

            return new System.Random(BitConverter.ToInt32(hash, 0));
        }

        public string ObfuscatePath(string realPath)
        {
            string ext = Path.GetExtension(realPath);
            string pathWithoutExtension = realPath[..^ext.Length];

            string[] split = pathWithoutExtension.Split(Path.DirectorySeparatorChar);

            if(split.Length == 0) return ext;

            StringBuilder obfuscatedPathBuilder = new StringBuilder();
            StringBuilder realPathBuilder = new StringBuilder();
            obfuscatedPathBuilder.Append(split[0]);

            System.Random random = CreatePathRandom(realPath, "Obfuscate");

            foreach(string s in split[1..])
            {
                realPathBuilder.Append(Path.DirectorySeparatorChar);
                obfuscatedPathBuilder.Append(Path.DirectorySeparatorChar);

                realPathBuilder.Append(s);

                // Obfuscate only if the folder path is not on the list
                if(allowedFolderPathRegex.IsMatch(realPathBuilder.ToString()))
                {
                    Debug.Log(realPathBuilder + " matches ");
                    obfuscatedPathBuilder.Append(s);
                }
                else
                    obfuscatedPathBuilder.Append(new string(random.ChooseMany(obfuscateLetters, s.Length).ToArray()));
            }

            obfuscatedPathBuilder.Append(ext);

            Debug.Log(obfuscatedPathBuilder.ToString());

            return obfuscatedPathBuilder.ToString();
        }
    }
}