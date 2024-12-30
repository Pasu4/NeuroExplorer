using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts
{
    public class Room : MonoBehaviour
    {
        public Player player;

        public float cellSize = 2.0f;
        [Range(0, 1)]
        public float placementRandomness = 0.8f;
        [Min(1)]
        public float maxRoomAspectRatio = 4.0f;
        public float minRoomArea = 25f;

        public GameObject filePrefab;
        public GameObject dirPrefab;
        public GameObject chestPrefab;
        public GameObject encounterPrefab;

        public GameObject wallTop;
        public GameObject wallBottom;
        public GameObject wallLeft;
        public GameObject wallRight;

        public List<GameObject> groundObjects;
        public List<GameObject> encounters;
        public string realPath;
        public string displayPath;

        SpriteRenderer spriteRenderer;

        // TODO add boss rooms
        private Regex noEnemyRooms = new(@"
            ^C:\\Users\\\w+(?:\\OneDrive)?\\Desktop$
        ", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        readonly List<FakeDir> fakeDirs = new()
        {
            new(@"C:\Users", "Vedal"),
            new(@"C:\Users\Vedal", "source"),
            new(@"C:\Users\Vedal\source", "repos"),
            new(@"C:\Users\Vedal\source\repos", "FilAIn"),

            new(@"C:\Program Data", "VedalAI"),
            new(@"C:\Program Data\VedalAI", "AImila"),

            new(@"C:\Windows", "Final"),
            new(@"C:\Windows\Final", "Boss"),
            new(@"C:\Windows\Final\Boss", "AIris"),
        };

        // Use this for initialization
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            //ChangeRoom(GameManager.Instance.startPath);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeRoom(string newPath)
        {
            string fromPath = realPath;
            realPath = newPath;
            displayPath = GameManager.Instance.obfuscate ? GameManager.Instance.ObfuscatePath(realPath, true) : realPath;
            System.Random random = GameManager.Instance.CreatePathRandom(newPath, "RoomLayout");

            // Destroy old ground objects
            foreach(GameObject go in groundObjects)
                Destroy(go);
            groundObjects.Clear();

            foreach(GameObject go in encounters)
                Destroy(go);
            encounters.Clear();

            if(newPath == @"C:\Users\Vedal\source\repos\FilAIn")
            {
                SpecialRoom(@"C:\Users\Vedal\source\repos\FilAIn");
                return;
            }
            if(newPath == @"C:\Users\Vedal\AppData\LocalLow\AImila")
            {
                SpecialRoom(@"C:\Users\Vedal\AppData\LocalLow\AImila");
                return;
            }
            if(newPath == @"C:\Windows\Final")
            {
                SpecialRoom(@"C:\Windows\Final");
                return;
            }
            if(newPath == @"C:\Windows\Final\Boss")
            {
                SpecialRoom(@"C:\Windows\Final\Boss");
                return;
            }
            if(newPath == @"C:\Windows\Final\Boss\AIris")
            {
                SpecialRoom(@"C:\Windows\Final\Boss\AIris");
                return;
            }

            List<string> files = new();
            List<string> dirs = new();

            if(Directory.Exists(newPath))
            {
                if(newPath.Contains(Path.DirectorySeparatorChar)) // Problems if there is no path separator
                {
                    files = Directory.GetFiles(newPath).ToList();
                    dirs = Directory.GetDirectories(newPath).ToList();
                }
                else
                {
                    files = Directory.GetFiles(newPath + "\\").ToList();
                    dirs = Directory.GetDirectories(newPath + "\\").ToList();
                }
            }

            // Add fake dirs
            dirs.AddRange(fakeDirs.Where(d => d.location == newPath).Select(d => d.location + Path.DirectorySeparatorChar + d.name));

            int objCount = files.Count + dirs.Count;

            // Calculate room dimensions
            float area = objCount * cellSize * cellSize;
            area = Mathf.Max(minRoomArea, area);
            float sqrtArea = Mathf.Sqrt(area);
            float sqrtMaxRoomAspectRatio = Mathf.Sqrt(maxRoomAspectRatio);
            
            float width = random.Range(sqrtArea / sqrtMaxRoomAspectRatio, sqrtArea * sqrtMaxRoomAspectRatio);
            float height = Mathf.Ceil(area / width);
            width = Mathf.Ceil(width);
            int iWidth = Mathf.CeilToInt(width / cellSize);
            int iHeight = Mathf.CeilToInt(height / cellSize);
            width = Mathf.Ceil(iWidth * cellSize);
            height = Mathf.Ceil(iHeight * cellSize);

            SetSize(new Vector2(width+3, height+3)); // margin so objects are not on the wall

            GameObject[,] grid = new GameObject[iWidth, iHeight];
            int index = 0;

            // Place objects
            foreach(string file in files)
            {
                //if(Path.GetExtension(file).ToLower() == ".lnk")
                //    grid[index % iWidth, index / iWidth] = CreateLink(file);
                //else
                //    grid[index % iWidth, index / iWidth] = CreateFile(file);
                grid[index % iWidth, index / iWidth] = CreateFile(file);
                index++;
            }
            foreach(string dir in dirs)
            {
                //if(Utils.HasReadPermission(dir) && Directory.GetDirectories(dir).Length == 0)
                //    grid[index % iWidth, index / iWidth] = CreateChest(dir);
                //else
                //    grid[index % iWidth, index / iWidth] = CreateDir(dir);
                grid[index % iWidth, index / iWidth] = CreateDir(dir);
                index++;
            }

            // Place up navigation
            if(newPath.Contains(Path.DirectorySeparatorChar)) // Only if it is not the top folder
            {
                grid[index % iWidth, index / iWidth] = CreateUpDir(newPath);
                index++;
            }

            // Position elements
            grid.Shuffle2D(random);
            for(int x = 0; x < iWidth; x++)
            {
                for(int y = 0; y < iHeight; y++)
                {
                    if(grid[x, y] == null) continue;

                    grid[x, y].transform.position = new Vector2(
                        -width  / 2f + (x + 0.5f + random.Range(-placementRandomness, placementRandomness) / 2f) * cellSize,
                        -height / 2f + (y + 0.5f + random.Range(-placementRandomness, placementRandomness) / 2f) * cellSize
                    );
                }
            }

            // Set player position
            GameObject fromObj = groundObjects
                .FirstOrDefault(go => go.GetComponent<GroundObject>().realPath == fromPath);
            if(fromObj != null)
                player.transform.position = fromObj.transform.position;
            else
                player.transform.position = Vector2.zero;

            // Spawn encounters
            if(!noEnemyRooms.IsMatch(newPath))
            {
                int count = groundObjects.Count / 10 + 1;
                List<GroundObject> guardedObjects = groundObjects
                    .Select(o => o.GetComponent<GroundObject>())
                    .OrderByDescending(o => o is GroundFile)
                    .ThenByDescending(o => o is GroundFile f ? f.fileSize : 0)
                    .ThenBy(_ => random.Next())
                    .Take(count)
                    .ToList();

                long averageFileSize;
                try
                {
                    averageFileSize = (long) groundObjects
                        .Select(o => o.GetComponent<GroundObject>())
                        .OfType<GroundFile>()
                        .Average(o => o.fileSize);
                }
                catch(System.InvalidOperationException)
                {
                    averageFileSize = 10000;
                }

                for(int i = 0; i < guardedObjects.Count; i++)
                {
                    if(Vector2.Distance(player.transform.position, guardedObjects[i].transform.position) < 5)
                        continue; // Don't spawn too close to player

                    CreateEncounter(guardedObjects[i].realPath, guardedObjects[i].transform.position, GameManager.Instance.defaultEnemyStrength);
                    //if(guardedObjects[i] is GroundFile f)
                    //{
                    //    CreateEncounter(f.realPath, f.transform.position, (long) (Mathf.Sqrt(f.fileSize) * GameManager.Instance.enemyStrengthMultiplier));
                    //}
                    //else if(guardedObjects[i] is GroundDir d)
                    //{
                    //    CreateEncounter(d.realPath, d.transform.position, (long) Mathf.Sqrt(averageFileSize));
                    //}
                }
            }
        }

        public GameObject CreateFile(string path)
        {
            GameObject fileObj = Instantiate(filePrefab, transform);
            groundObjects.Add(fileObj);
            fileObj.GetComponent<GroundObject>().Init(this, path);
            return fileObj;
        }

        public GameObject CreateDir(string path)
        {
            GameObject dirObj = Instantiate(dirPrefab, transform);
            groundObjects.Add(dirObj);
            dirObj.GetComponent<GroundObject>().Init(this, path);
            return dirObj;
        }

        //public GameObject CreateLink(string path)
        //{

        //}

        public GameObject CreateChest(string path)
        {
            GameObject chestObj = Instantiate(chestPrefab, transform);
            groundObjects.Add(chestObj);
            chestObj.GetComponent<GroundObject>().Init(this, path);
            return chestObj;
        }

        public GameObject CreateUpDir(string path)
        {
            int lastSeperator = path.LastIndexOf(Path.DirectorySeparatorChar);
            string parentPath = path.Remove(lastSeperator);
            Debug.Log(parentPath);

            GameObject dirObj = Instantiate(dirPrefab, transform);
            groundObjects.Add(dirObj);
            dirObj.GetComponent<GroundDir>().Init(this, parentPath);
            dirObj.GetComponent<GroundDir>().SetUpDir();
            return dirObj;
        }

        private void CreateEncounter(string id, Vector2 position, long strength)
        {
            GameObject go = Instantiate(encounterPrefab);
            encounters.Add(go);
            go.transform.position = position;
            go.transform.position = (Vector2) go.transform.position + Random.insideUnitCircle * 2;
            go.GetComponent<Encounter>().Init(id, strength);
        }

        private void SpecialRoom(string path)
        {
            GameObject exit = CreateUpDir(path);

            if(path == @"C:\Users\Vedal\source\repos\FilAIn")
            {
                SetSize(new Vector2(10, 10));
                exit.transform.position = player.transform.position = Vector2.down * 4;
            }
            else if(path == @"C:\Users\Vedal\AppData\LocalLow\AImila")
            {
                SetSize(new Vector2(20, 20));
                exit.transform.position = player.transform.position = Vector2.down * 9;
            }
            else if(path == @"C:\Windows\Final")
            {
                SetSize(new Vector2(5, 25));
                exit.transform.position = player.transform.position = Vector2.down * 11;

                GameObject dir = CreateDir(@"C:\Windows\Final\Boss");
                dir.transform.position = Vector2.up * 9;
            }
            else if(path == @"C:\Windows\Final\Boss")
            {
                SetSize(new Vector2(5, 25));
                exit.transform.position = player.transform.position = Vector2.down * 11;

                GameObject dir = CreateDir(@"C:\Windows\Final\Boss\AIris");
                dir.transform.position = Vector2.up * 9;
            }
            else if(path == @"C:\Windows\Final\Boss\AIris")
            {
                SetSize(new Vector2(5, 25));
                exit.transform.position = player.transform.position = Vector2.down * 11;
            }
        }

        public void SetSize(Vector2 size)
        {
            spriteRenderer.size = size;
            SetWallPositions();
        }

        void SetWallPositions()
        {
            wallTop.transform.localPosition     = Vector3.up    * (0.5f * spriteRenderer.size.y - 0.5f);
            wallBottom.transform.localPosition  = Vector3.down  * (0.5f * spriteRenderer.size.y - 0.25f);
            wallLeft.transform.localPosition    = Vector3.left  * (0.5f * spriteRenderer.size.x + 0.5f);
            wallRight.transform.localPosition   = Vector3.right * (0.5f * spriteRenderer.size.x + 0.5f);
        }

        public Vector2 RandomPosition(System.Random random)
        {
            float x = (0.5f * spriteRenderer.size.x - 0.5f);
            float y = (0.5f * spriteRenderer.size.y - 0.5f);
            return new Vector2(
                random.Range(-x, x),
                random.Range(-y, y)
            );
        }
    }
}