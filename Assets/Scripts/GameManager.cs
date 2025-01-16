using Assets.Scripts.Enemies;
using Assets.Scripts.Integration.Actions;
using NeuroSdk.Actions;
using NeuroSdk.Messages.Outgoing;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using SRandom = System.Random;
using URandom = UnityEngine.Random;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameMode gameMode = GameMode.MainMenu;

        public string startPath;
        public string currentObjectiveText;
        public string currentObjectivePath;
        public bool obfuscate;
        public int gameSeed;
        public int difficulty = -1;
        public int progress = 0;
        [HideInInspector]
        public bool obfuscateSet = false;
        public bool skipIntro = false;
        public long defaultEnemyStrength;
        public float enemyHpScale = 1;
        public float neuroVisionRange = 10f;

        public InventoryUI inventoryUI;
        public RoomUI roomUI;
        public BattleUI battleUI;
        public Image fadeScreen;
        public Room room;
        public DialogueUI dialogueUI;
        public GameObject mainCamera;

        public Player player;
        public long hp;
        public long maxHp;
        public long mp;
        public long maxMp;
        public long block;
        public List<Card> deck = new();
        public long inventorySpace;
        public long UsedStorage => deck.Sum(c => c.fileSize);
        public long FreeStorage => inventorySpace - UsedStorage;

        public FileSprite[] fileSprites;
        public Sprite[] defaultSprites;
        public CardSprites cardSprites;
        public Sprite enemyActionAttackSprite;
        public Sprite enemyActionDefendSprite;
        public Sprite enemyActionTrojanSprite;
        public Sprite enemyActionDoNothingSprite;
        public Sprite enemyActionSummonSprite;
        public Sprite enemyActionBuffSprite;
        public Sprite enemyActionHealSprite;
        [HideInInspector]
        public Enemy[] Enemies => new Enemy[] { EnemyResources.Drone, EnemyResources.GymbagDrone, EnemyResources.NeuroYukkuri };
        public Sprite lockedDirSprite;
        public Sprite upDirSprite;
        public AudioSource roomSource;
        public AudioSource battleSource;
        public AudioClip battleClip;
        public AudioClip bossClip;
        public AudioClip finalBossClip;
        public TextAsset fakeDirText;
        public TextAsset fakeFileText;
        public string[] fakeDirs;
        public Dictionary<string, string[]> fakeFilesByExt;

        public AudioSource sfxSource;

        public GameObject textEffectPrefab;
        public GameObject neuroPlayer;
        public GameObject evilPlayer;
        public GameObject argPlayer;

        private readonly SHA1 sha1 = SHA1.Create();
        private Regex allowedFolderPathRegex;

        private InputAction inventoryAction;
        private InputAction cancelAction;

        public List<string> defeatedEncounters;

        [Space(20)]
        [Header("Resources")]
        public SoundEffectsResources sfx;
        public EnemySpriteResources enemySprites;

        private void Awake()
        {
#if UNITY_EDITOR
            Application.runInBackground = true;
#endif

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
                    \\Windows(\\.+)?
                    | \\Users (?:
                        \\Vedal
                        | \\\w+ (?:
                            \\source (?:
                                \\repos (?:
                                    \\FilAIn (?:
                                        \\.+
                                    )?
                                )?
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
                    | \\Program\sFiles (?:
                        \\VedalAI (?:
                            \\AImila (?:
                                \\.+
                            )?
                        )?
                    )?
                    | \\Program\sFiles\s\(x86\)
                    | \\Temp
                )$
                | .*\\desktop\.ini$
            ", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);

            fakeDirs = fakeDirText.text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            fakeFilesByExt = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(fakeFileText.text);

            roomSource.Stop();
            battleSource.Stop();

            StartCoroutine(CStart());
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

        public void SetObfuscate(bool value)
        {
            obfuscate = value;
            obfuscateSet = true;
        }

        public void SetDifficulty(int value)
        {
            difficulty = value;
        }

        public System.Random CreatePathRandom(string path, string tag)
        {
            string seed = path + ':' + tag;

            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(seed));

            return new System.Random(BitConverter.ToInt32(hash, 0));
        }

        public string ObfuscatePath(string realPath, bool isDir)
        {
            string ext = isDir ? "" : Path.GetExtension(realPath);

            string[] split = realPath.Split(Path.DirectorySeparatorChar);

            if(split.Length == 0) return ext;

            StringBuilder fakePathBuilder = new(split[0]);
            StringBuilder realPathBuilder = new(split[0]);

            int end = isDir ? 0 : 1;

            foreach(string s in split[1..^end])
            {
                realPathBuilder.Append(Path.DirectorySeparatorChar);
                fakePathBuilder.Append(Path.DirectorySeparatorChar);

                bool isUserDir = realPathBuilder.ToString() == "C:\\Users\\";

                realPathBuilder.Append(s);

                SRandom random = CreatePathRandom(realPathBuilder.ToString(), "obfuscate_dir");

                // Obfuscate only if the folder path is not on the list
                if(allowedFolderPathRegex.IsMatch(realPathBuilder.ToString()))
                {
                    fakePathBuilder.Append(s);
                }
                else
                {
                    if(isUserDir)
                        fakePathBuilder.Append(random.Choose("Vedal", "Neuro", "Evil", "Anny", "Alex", "Tony", "Kayori", "Samantha"));
                    else
                        fakePathBuilder.Append(random.Choose(fakeDirs));
                }
            }

            if(!isDir)
            {
                realPathBuilder.Append(Path.DirectorySeparatorChar);
                fakePathBuilder.Append(Path.DirectorySeparatorChar);

                realPathBuilder.Append(Path.GetFileName(realPath));

                if(allowedFolderPathRegex.IsMatch(realPathBuilder.ToString()))
                {
                    fakePathBuilder.Append(Path.GetFileName(realPath));
                }
                else
                {
                    SRandom random = CreatePathRandom(realPathBuilder.ToString(), "obfuscate_file");

                    if(!fakeFilesByExt.TryGetValue(ext, out string[] possibleNames))
                        possibleNames = new[] { random.NextDouble().ToString("F7", CultureInfo.InvariantCulture)[2..] };

                    fakePathBuilder.Append(random.Choose(possibleNames));
                    fakePathBuilder.Append(ext);
                }
            }

            return fakePathBuilder.ToString();
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

        public void CreateTextEffect(string text, Color color, Vector2 position) => CreateTextEffect(text, color, position, Vector2.up);

        public void CreateTextEffect(string text, Color color, Vector2 position, Vector2 direction)
        {
            GameObject textEffect = Instantiate(textEffectPrefab);
            TextMeshProUGUI tmp = textEffect.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = text;
            tmp.color = color;
            textEffect.transform.position = position;
            textEffect.GetComponent<TextEffect>().direction = direction;
        }

        public Sprite GetFileSprite(string name, System.Random random)
        {
            IEnumerable<Sprite> possibleSprites = fileSprites
                .Where(fs => fs.fileExtension.Split(';', StringSplitOptions.RemoveEmptyEntries).Contains(name.ToLower()))
                .Select(fs => fs.sprite);

            if(possibleSprites.Count() == 0)
                possibleSprites = defaultSprites;

            return random.Choose(possibleSprites);
        }

        public Sprite GetFileSprite(string name) => GetFileSprite(name, new System.Random());

        public void StartBattle(Enemy[] enemies, string encounterId, bool isSpecial, AudioClip music)
        {
            battleUI.battleWon = false;
            NeuroActionHandler.UnregisterActions(AdvanceDialogueAction.staticName);
            StartCoroutine(CTransitionBattle(enemies, encounterId, isSpecial, music));
        }

        public void GameOver() => StartCoroutine(CGameOver());

        public void BattleWin(bool reward) => StartCoroutine(CBattleWin(reward));

        public void TransitionRoom(string realPath) => StartCoroutine(CTransitionRoom(realPath));

        public void BossScene(BossTrigger trigger)
        {
            switch(trigger.dialogueId)
            {
                case "filian":
                    StartCoroutine(difficulty switch
                    {
                        //1 => dialogueUI.EvilFilianScene(),
                        //2 => dialogueUI.UnknownFilianScene(),
                        _ => dialogueUI.NeuroFilianScene(trigger),
                    });
                    break;
                case "camila":
                    StartCoroutine(difficulty switch
                    {
                        //1 => dialogueUI.EvilCamilaScene(),
                        //2 => dialogueUI.UnknownCamilaScene(),
                        _ => dialogueUI.NeuroCamilaScene(trigger),
                    });
                    break;
                case "airis":
                    StartCoroutine(difficulty switch
                    {
                        //1 => dialogueUI.EvilAirisScene(),
                        //2 => dialogueUI.UnknownAirisScene(),
                        _ => dialogueUI.NeuroAirisScene(trigger),
                    });
                    break;
            }
        }

        public IEnumerator CStart()
        {
            yield return null; // Wait for other scripts to initialize

            gameMode = GameMode.Dialogue;
            dialogueUI.background.color = Color.black;
            fadeScreen.color = Color.black;

            dialogueUI.gameObject.SetActive(true);
            dialogueUI.textboxObj.SetActive(false);
            dialogueUI.obfuscateDialog.SetActive(true);

            Context.Send(
                "In this game, you will navigate the folders in Vedal's PC. " +
                "Each folder is visualized as a room, and you can either go to the parent directory by ascending a ladder " +
                "or go to a child directory by descending a ladder. " +
                "You have to navigate the rooms like a directory tree. " +
                "For example, to get from 'C:\\Users\\Neuro' to 'C:\\Users\\Vedal', you have to first ascend to 'C:\\Users' and then descend to 'C:\\Users\\Vedal'. " +
                "Files can be picked up and used as cards to fight enemies. " +
                "Cards with a larger file size usually deal and block more damage, and have stronger secondary effects."
            );

            if(!skipIntro)
            {
                yield return new WaitForSeconds(1.0f);
                yield return FadeIn(Color.black);

                NeuroActionHandler.RegisterActions(new SetObfuscateAction());
                Context.Send("As a safety measure, you must activate streamer mode to start the game.");

                while(!obfuscateSet) yield return null; // Wait for dialog complete

                NeuroActionHandler.UnregisterActions(SetObfuscateAction.staticName);

                yield return FadeOut(Color.black);
                yield return new WaitForSeconds(1.0f);
            }
            else
            {
                SetObfuscate(true);
            }

            dialogueUI.obfuscateDialog.SetActive(false);
            dialogueUI.textboxObj.SetActive(true);
            fadeScreen.color = Color.clear;
            roomSource.Play();

            if(!skipIntro)
                yield return dialogueUI.CStartScene();
            else
            {
                difficulty = 0;
                dialogueUI.gameObject.SetActive(false);
                currentObjectivePath = @"C:\Users\Vedal\source\repos";
                currentObjectiveText = "Folders called 'source' and 'repos' in Vedal's user folder";
            }

            dialogueUI.background.color = Color.clear;

            GameObject playerObj = Instantiate(difficulty switch { 0 => neuroPlayer, 1 => evilPlayer, 2 => argPlayer, _ => throw new NotImplementedException() });
            mainCamera.transform.SetParent(playerObj.transform);
            mainCamera.transform.localPosition = Vector3.back * 10;
            player = playerObj.GetComponent<Player>();

            enemyHpScale += enemyHpScale * difficulty / 2f;
            defaultEnemyStrength += (long) (defaultEnemyStrength * difficulty / 2f);

            deck.AddRange(new Card[] { CardResources.Osu, CardResources.Minecraft, CardResources.SlayTheSpire, CardResources.DesktopIni, CardResources.Prompt, CardResources.NeuroLog });

            if(difficulty == 0)      deck.Add(CardResources.Gymbag);
            else if(difficulty == 1) deck.Add(CardResources.MetalPipe);
            else if(difficulty == 2) deck.Add(CardResources.Arg);

            gameMode = GameMode.Room;
            room.ChangeRoom(startPath);

            yield return FadeIn(Color.black);

            NeuroRoomStart();
        }

        private IEnumerator CTransitionRoom(string realPath)
        {
            //IntegrationRoomEnd();
            sfxSource.PlayOneShot(sfx.ladder);
            gameMode = GameMode.Transition;

            yield return FadeOut(Color.black);

            room.ChangeRoom(realPath);
            gameMode = GameMode.Room;

            yield return FadeIn(Color.black);

            NeuroRoomStart();
        }

        private IEnumerator CTransitionBattle(Enemy[] enemies, string encounterId, bool isSpecial, AudioClip music)
        {
            gameMode = GameMode.Transition;
            roomSource.Pause();
            yield return FadeOut(Color.white);

            gameMode = GameMode.Battle;
            battleUI.gameObject.SetActive(true);
            battleUI.StartBattle(enemies, encounterId, isSpecial);
            gameMode = GameMode.Battle;
            battleSource.clip = music;
            battleSource.Play();
            yield return FadeIn(Color.white);
        }

        private IEnumerator CGameOver()
        {
            gameMode = GameMode.Transition;
            battleUI.StopAllCoroutines();
            dialogueUI.StopAllCoroutines();
            dialogueUI.gameObject.SetActive(false);
            battleUI.waitingForAction = false; // End action window

            yield return FadeOut(Color.black);

            battleSource.Stop();
            battleUI.gameObject.SetActive(false);

            yield return new WaitForSeconds(1.0f);

            CreateTextEffect("<size=300><b>YOU DIED</b></size>\n<size=100>and lost some cards</size>", Color.red, player.transform.position, Vector3.zero);
            Context.Send("You died and lost some cards.");
            yield return new WaitForSeconds(3.0f);

            deck = deck
                .OrderBy(_ => URandom.value)
                .Take(deck.Count/2)
                .ToList();
            hp = maxHp;

            roomSource.UnPause();
            room.ChangeRoom(startPath);
            gameMode = GameMode.Room;
            NeuroRoomStart();

            yield return FadeIn(Color.black);
        }

        private IEnumerator CBattleWin(bool reward)
        {
            battleUI.StopAllCoroutines();

            Context.Send("You have defeated the enemies and won the battle.");

            yield return FadeOut(Color.black);
            battleUI.gameObject.SetActive(false);
            defeatedEncounters.Add(battleUI.encounterId);
            battleSource.Stop();
            roomSource.UnPause();

            // Some random bonus
            if(reward)
            {
                string rewardText = "";
                Utils.ChooseWeighted<Action>(new System.Random(),
                    (10, new(() => {
                        long amount = (long) (maxHp * URandom.Range(0.05f, 0.10f));
                        maxHp += amount;
                        hp += amount;
                        rewardText = $"Data integrity increased by {Utils.FileSizeString(amount)}";
                    })),
                    (10, new(() => {
                        long amount = (long) (maxMp * URandom.Range(0.05f, 0.10f));
                        maxMp += amount;
                        mp += amount;
                        rewardText = $"Available RAM increased by {Utils.FileSizeString(amount)}";
                    })),
                    (10, new(() => {
                        long amount = (long) (inventorySpace * URandom.Range(0.05f, 0.10f));
                        inventorySpace += amount;
                        rewardText = $"Available Storage increased by {Utils.FileSizeString(amount)}";
                    })),
                    (10, new(() => {
                        Card card = new Card(
                            URandom.value.ToString("F7", CultureInfo.InvariantCulture)[2..] + ".log",
                            (long) (maxMp * URandom.Range(0.5f, 1.0f)),
                            GetFileSprite("")
                        );
                        deck.Add(card);
                        rewardText = "You got a new card";
                    }))
                )();

                CreateTextEffect(rewardText, Color.green, player.transform.position);
            }

            gameMode = GameMode.Room;
            yield return FadeIn(Color.black);

            NeuroRoomStart();
        }

        public IEnumerator FadeOut(Color color)
        {
            fadeScreen.color = color;
            for(float a = 0f; a < 1f; a += Time.deltaTime)
            {
                Color c = fadeScreen.color;
                c.a = a;
                fadeScreen.color = c;
                yield return null;
            }
            Color c2 = fadeScreen.color;
            c2.a = 1f;
            fadeScreen.color = c2;
        }

        public IEnumerator FadeIn(Color color)
        {
            fadeScreen.color = color;
            for(float a = 1f; a > 0f; a -= Time.deltaTime)
            {
                Color c = fadeScreen.color;
                c.a = a;
                fadeScreen.color = c;
                yield return null;
            }
            Color c2 = fadeScreen.color;
            c2.a = 0f;
            fadeScreen.color = c2;
        }

        public void NeuroRoomStart()
        {
            if(GetComponent<ActionWindow>() != null)
            {
                Debug.LogError("Action window already exists.");
            }

            ActionWindow actionWindow = ActionWindow.Create(gameObject)
                .AddAction(new ExploreRoomAction())
                .AddAction(new RoomAscendAction())
                .AddAction(new RoomDescendAction())
                .AddAction(new RoomPickupItemAction());

            // Can only engage the boss if this is a boss room and the boss is alive
            if(room.isBossRoom && FindFirstObjectByType<BossTrigger>() != null)
                actionWindow.AddAction(new EngageBossAction());

            // Can only heal if there is a healing item in the room
            if(room.displayPath.EndsWith("Desktop"))
                actionWindow.AddAction(new UseHealPointAction());

            actionWindow
                .SetEnd(() => gameMode != GameMode.Room)
                .Register();

            SendRoomContext();
        }
        //public void IntegrationRoomEnd()
        //{
        //    NeuroActionHandler.UnregisterActions(
        //        ExploreRoomAction.staticName,
        //        RoomAscendAction.staticName,
        //        RoomDescendAction.staticName,
        //        RoomPickupItemAction.staticName,
        //        EngageBossAction.staticName,
        //        HealAction.staticName
        //    );
        //}

        public void SendRoomContext()
        {
            StringBuilder sb = new();

            sb.AppendLine($"You are currently at {room.displayPath}.");

            if(room.displayPath == currentObjectivePath)
                sb.AppendLine("You have arrived at your target, the boss should be one level down from here.");
            else if(room.displayPath.StartsWith(currentObjectivePath))
                sb.AppendLine("The boss is in this room. Engage them if you feel strong enough to beat them.");
            else if(!string.IsNullOrEmpty(currentObjectivePath))
                sb.AppendLine($"Your next target is at {currentObjectivePath}.");

            // Storage
            sb.AppendLine($"\nYou currently have {Utils.FileSizeString(FreeStorage)} of free storage.");

            // Nearby rooms
            IEnumerable<string> nearbyRooms = room.groundObjects
                .Select(go => go.GetComponent<GroundDir>())
                .Where(dir => dir != null && !dir.locked && !dir.isUpDir && Vector2.Distance(dir.transform.position, player.transform.position) <= neuroVisionRange)
                .Select(dir => dir.DisplayName);

            if(nearbyRooms.Any())
            {
                sb.AppendLine("\nThe following rooms are nearby: ");
                sb.AppendLine(string.Join("; ", nearbyRooms));
            }
            else
                sb.AppendLine("\nThere are no rooms nearby that you can enter.");

            // Nearby files
            IEnumerable<string> nearbyFiles = room.groundObjects
                .Select(go => go.GetComponent<GroundFile>())
                .Where(file => file != null && Vector2.Distance(file.transform.position, player.transform.position) <= neuroVisionRange)
                .Select(file => $"\"{file.DisplayName}\" ({Utils.FileSizeString(file.fileSize)})");

            if(nearbyFiles.Any())
            {
                sb.AppendLine("\nThe following items are nearby: ");
                sb.AppendLine(string.Join("; ", nearbyFiles));
            }
            else
                sb.AppendLine("\nThere are no items nearby that you can pick up.");

            // Healing item
            if(FindFirstObjectByType<Heal>() != null)
                sb.AppendLine("\nThere is a healing point in the room.");

            // Send
            Context.Send(sb.ToString());
        }
    }
}