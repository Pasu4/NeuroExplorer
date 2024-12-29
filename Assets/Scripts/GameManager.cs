using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameMode gameMode = GameMode.MainMenu;

        public string startPath;
        public bool obfuscate;
        public int gameSeed;

        public InventoryUI inventoryUI;
        public RoomUI roomUI;
        public BattleUI battleUI;

        public Player player;
        public long hp;
        public long maxHp;
        public long mp;
        public long maxMp;
        public long block;
        public List<Card> deck = new();
        public long inventorySpace;
        public long UsedStorage => deck.Sum(c => c.FileSize);
        public long FreeStorage => inventorySpace - UsedStorage;

        public FileSprite[] fileSprites;
        public Sprite[] defaultSprites;
        public CardSprites cardSprites;
        public Sprite enemyActionAttackSprite;
        public Sprite enemyActionDefendSprite;
        public Sprite enemyActionTrojanSprite;

        public GameObject textEffectPrefab;

        private readonly SHA1 sha1 = SHA1.Create();
        private Regex allowedFolderPathRegex;

        private InputAction inventoryAction;
        private InputAction cancelAction;

        public List<string> defeatedEncounters;

        private void Awake()
        {
            Instance = this;

            inventoryAction = InputSystem.actions.FindAction("Inventory");
            cancelAction = InputSystem.actions.FindAction("Cancel");
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
            | desktop\.ini$
            ", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
        }

        // Update is called once per frame
        void Update()
        {
            if(inventoryAction.WasPerformedThisFrame() && gameMode == GameMode.Room)
            {
                
                gameMode = GameMode.Inventory;
                inventoryUI.gameObject.SetActive(true);
                inventoryUI.Init();
            }
            else if((inventoryAction.WasPerformedThisFrame() || cancelAction.WasPerformedThisFrame()) && gameMode == GameMode.Inventory)
            {
                gameMode = GameMode.Room;
                inventoryUI.gameObject.SetActive(false);
            }
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

            string[] split = realPath.Split(Path.DirectorySeparatorChar);

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

            // Correct extension
            obfuscatedPathBuilder.Length -= ext.Length;
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

        public void CreateTextEffect(string text, Color color, Vector2 position)
        {
            GameObject textEffect = Instantiate(textEffectPrefab);
            TextMeshProUGUI tmp = textEffect.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.color = color;
            textEffect.transform.position = position;
        }

        public Sprite GetFileSprite(string name, System.Random random)
        {
            IEnumerable<Sprite> possibleSprites = fileSprites
                .Where(fs => fs.fileExtension == name)
                .Select(fs => fs.sprite);

            if(possibleSprites.Count() == 0)
                possibleSprites = defaultSprites;

            return random.Choose(possibleSprites);
        }

        public Sprite GetFileSprite(string name) => GetFileSprite(name, new System.Random());

        public void StartBattle(Enemy[] enemies, string encounterId)
        {
            gameMode = GameMode.Battle;
            battleUI.gameObject.SetActive(true);
            battleUI.StartBattle(enemies, encounterId);
        }

        public void EndBattle()
        {
            gameMode = GameMode.Room;
            battleUI.gameObject.SetActive(false);
        }

        public void GameOver()
        {
            // TODO
        }
    }
}