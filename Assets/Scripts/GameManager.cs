using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public GameMode gameMode = GameMode.MainMenu;

        public string startPath;
        public bool obfuscate;
        public int gameSeed;

        public Player player;
        public List<Card> deck;

        public FileSprite[] fileSprites;
        public Sprite[] defaultSprites;

        private readonly SHA1 sha1 = SHA1.Create();
        private Regex allowedFolderPathRegex;

        private void Awake()
        {
            Instance = this;
        }

        // Use this for initialization
        void Start()
        {
            startPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if(gameSeed < 0)
            {
                RandomNumberGenerator secureRng = RandomNumberGenerator.Create();
                byte[] b = new byte[4];
                secureRng.GetBytes(b);
                gameSeed = Mathf.Abs(BitConverter.ToInt32(b, 0));
            }

            allowedFolderPathRegex = new Regex(@"
                ^C:(?:
                    \\Windows
                    | \\Users (?:
                        \\\w+ (?:
                            \\source (?:
                                \\repos
                            )?
                            | \\Downloads
                            | \\Games
                            | \\Videos
                            | \\OneDrive
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
                )$
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

            StringBuilder obfuscatedPathBuilder = new(split[0]);
            StringBuilder realPathBuilder = new(split[0]);

            foreach(string s in split[1..])
            {
                realPathBuilder.Append(Path.DirectorySeparatorChar);
                obfuscatedPathBuilder.Append(Path.DirectorySeparatorChar);

                realPathBuilder.Append(s);

                // Obfuscate only if the folder path is not on the list
                if(allowedFolderPathRegex.IsMatch(realPathBuilder.ToString()))
                {
                    obfuscatedPathBuilder.Append(s);
                }
                else
                {
                    obfuscatedPathBuilder.Append(ObfuscateString(s));
                }
            }

            obfuscatedPathBuilder.Append(ext);

            return obfuscatedPathBuilder.ToString();
        }

        private string ObfuscateString(string str)
        {
            string hashStr = BitConverter.ToString(
                    sha1.ComputeHash(Encoding.UTF8.GetBytes($"{str}:{gameSeed}"))
                ).Replace("-", "")
                .ToLowerInvariant();
            while(str.Length > hashStr.Length)
                hashStr += BitConverter.ToString(
                        sha1.ComputeHash(Encoding.UTF8.GetBytes(hashStr))
                    ).Replace("-", "")
                    .ToLowerInvariant();

            return hashStr[..str.Length];
        }
    }
}