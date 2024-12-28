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
        public float avgItemsPerUnit = 0.25f;
        [Min(1)]
        public float maxRoomAspectRatio = 4.0f;

        public GameObject filePrefab;
        public GameObject dirPrefab;

        public GameObject wallTop;
        public GameObject wallBottom;
        public GameObject wallLeft;
        public GameObject wallRight;

        public List<GroundObject> groundObjects;
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
            folderPath = newPath;
            System.Random random = GameManager.Instance.CreatePathRandom(newPath, "RoomLayout");
            Debug.Log($"Random number at {newPath}: {random.Next()}");

            // Destroy old ground objects
            foreach(GroundObject go in groundObjects)
            {
                Destroy(go);
            }

            // Find files and folders
            string[] files = Directory.GetFiles(newPath);
            string[] dirs = Directory.GetDirectories(newPath);

            int objCount = files.Length + dirs.Length;

            // Calculate room dimensions
            float area = objCount / avgItemsPerUnit;
            float sqrtArea = Mathf.Sqrt(area);
            float sqrtMaxRoomAspectRatio = Mathf.Sqrt(maxRoomAspectRatio);
            
            float width = random.Range(sqrtArea / sqrtMaxRoomAspectRatio, sqrtArea * sqrtMaxRoomAspectRatio);
            float height = area / width;

            SetSize(new Vector2(width, height));

            // Place objects
            foreach(string file in files)
            {
                CreateFile(file);
            }
            foreach(string dir in dirs)
            {
                CreateDir(dir);
            }
        }

        public void CreateFile(string file)
        {
            string path = Path.GetFullPath(Path.Combine(folderPath, file));

            GameObject fileObj = Instantiate(filePrefab, transform);
            fileObj.GetComponent<GroundObject>().Init(this, path);
        }

        public void CreateDir(string dir)
        {
            string path = Path.GetFullPath(Path.Combine(folderPath, dir));

            GameObject fileObj = Instantiate(filePrefab, transform);
            fileObj.GetComponent<GroundObject>().Init(this, path);
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