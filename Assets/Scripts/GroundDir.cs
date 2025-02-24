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
        public bool isUpDir = false;

        public override string DisplayName => displayPath == "C:" ? "C:" : Path.GetFileName(displayPath);

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

        public override void Click()
        {
            base.Click();

            if(GameManager.Instance.gameMode != GameMode.Room) return;

            Player player = GameManager.Instance.player;
            if(Vector2.Distance(player.transform.position, transform.position) > player.interactionRange)
                return;

            if(locked)
            {
                GameManager.Instance.CreateTextEffect("Locked", Color.red, transform.position);
                return;
            }

            if(!CanEnter())
            {
                GameManager.Instance.CreateTextEffect("Cannot enter", Color.red, transform.position);
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
                GetComponent<SpriteRenderer>().sprite = GameManager.Instance.lockedDirSprite;
            }
        }

        protected override void InitRandom()
        {
            random = GameManager.Instance.CreatePathRandom(DisplayName, "InitDir");
        }

        public void SetUpDir()
        {
            GetComponentInChildren<TextMeshProUGUI>(true).text = "Back to " + DisplayName;
            GetComponent<SpriteRenderer>().sprite = GameManager.Instance.upDirSprite;
            isUpDir = true;
        }

        public bool CanEnter()
        {
            GameManager gm = GameManager.Instance;
            return !locked
                && Vector2.Distance(gm.player.transform.position, transform.position) <= gm.player.interactionRange;
        }

        public bool CanNeuroDescend()
        {
            GameManager gm = GameManager.Instance;
            return !locked
                && !isUpDir
                && Vector2.Distance(transform.position, GameManager.Instance.player.transform.position) < GameManager.Instance.neuroVisionRange;
        }
    }
}