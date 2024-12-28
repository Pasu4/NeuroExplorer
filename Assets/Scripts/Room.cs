using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public GameObject filePrefab;
        public GameObject dirPrefab;
        public GameObject chestPrefab;

        public GameObject wallTop;
        public GameObject wallBottom;
        public GameObject wallLeft;
        public GameObject wallRight;

        public List<GameObject> groundObjects;
        public string folderPath;

        SpriteRenderer spriteRenderer;

        // Use this for initialization
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            ChangeRoom(GameManager.Instance.startPath);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ChangeRoom(string newPath)
        {
            string fromPath = folderPath;
            folderPath = newPath;
            System.Random random = GameManager.Instance.CreatePathRandom(newPath, "RoomLayout");

            // Destroy old ground objects
            foreach(GameObject go in groundObjects)
            {
                Destroy(go);
            }
            groundObjects.Clear();

            // Find files and folders
            string[] files, dirs;
            if(newPath.Contains(Path.DirectorySeparatorChar)) // Problems if there is no path separator
            {
                files = Directory.GetFiles(newPath);
                dirs = Directory.GetDirectories(newPath);
            }
            else
            {
                files = Directory.GetFiles(newPath + "\\");
                dirs = Directory.GetDirectories(newPath + "\\");
            }

            int objCount = files.Length + dirs.Length;

            // Calculate room dimensions
            float area = objCount * cellSize * cellSize;
            float sqrtArea = Mathf.Sqrt(area);
            float sqrtMaxRoomAspectRatio = Mathf.Sqrt(maxRoomAspectRatio);
            
            float width = random.Range(sqrtArea / sqrtMaxRoomAspectRatio, sqrtArea * sqrtMaxRoomAspectRatio);
            float height = Mathf.Ceil(area / width);
            width = Mathf.Ceil(width);
            int iWidth = Mathf.CeilToInt(width / cellSize);
            int iHeight = Mathf.CeilToInt(height / cellSize);
            width = Mathf.Ceil(iWidth * cellSize);
            height = Mathf.Ceil(iHeight * cellSize);

            SetSize(new Vector2(width, height));

            GameObject[,] grid = new GameObject[iWidth, iHeight];
            int index = 0;

            // Place objects
            foreach(string file in files)
            {
                // TODO: Links
                //if(Path.GetExtension(file).ToLower() == ".lnk")
                //    grid[index % iWidth, index / iWidth] = CreateLink(file);
                //else
                //    grid[index % iWidth, index / iWidth] = CreateFile(file);
                grid[index % iWidth, index / iWidth] = CreateFile(file);
                index++;
            }
            foreach(string dir in dirs)
            {
                if(Utils.HasReadPermission(dir) && Directory.GetDirectories(dir).Length == 0)
                    grid[index % iWidth, index / iWidth] = CreateChest(dir);
                else
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

        public void SetSize(Vector2 size)
        {
            spriteRenderer.size = size;
            SetWallPositions();
        }

        void SetWallPositions()
        {
            wallTop.transform.localPosition     = Vector3.up    * (0.5f * spriteRenderer.size.y + 0.5f);
            wallBottom.transform.localPosition  = Vector3.down  * (0.5f * spriteRenderer.size.y + 0.5f);
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