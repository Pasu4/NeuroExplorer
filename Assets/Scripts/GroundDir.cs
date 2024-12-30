using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

namespace Assets.Scripts
{
    public class GroundDir : GroundObject
    {
        public bool locked = false;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        protected override void OnMouseDown()
        {
            base.OnMouseDown();

            if(GameManager.Instance.gameMode != GameMode.Room) return;

            Player player = GameManager.Instance.player;
            if(Vector2.Distance(player.transform.position, transform.position) > player.interactionRange)
                return;

            if(locked)
            {
                GameManager.Instance.CreateTextEffect("Locked", Color.red, transform.position);
                return;
            }

            GameManager.Instance.TransitionRoom(realPath);
        }

        public override void Init(Room room, string realPath)
        {
            base.Init(room, realPath);

            if(Directory.Exists(realPath) && !Utils.HasReadPermission(realPath) || realPath == "C:\\$Recycle.Bin")
            {
                // Set locked
                locked = true;
                GetComponent<SpriteRenderer>().color = new Color(0.75f, 0, 0);
                // TODO: Change sprite
            }
        }

        protected override void InitRandom()
        {
            random = GameManager.Instance.CreatePathRandom(DisplayName, "InitDir");
        }

        public void SetUpDir()
        {
            GetComponentInChildren<TextMeshProUGUI>(true).text = "Back to " + Path.GetFileName(displayPath);
            // TODO: Change sprite to up
            GetComponent<SpriteRenderer>().color = Color.HSVToRGB(0f, 0.75f, 1f);
        }
    }
}