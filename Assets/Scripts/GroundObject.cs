using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class GroundObject : MonoBehaviour
    {
        public string realPath;
        public string displayPath;

        public GameObject label;

        public string DisplayName => displayPath == "C:" ? "C:" : Path.GetFileName(displayPath);

        protected System.Random random;
        protected Room room;

        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public virtual void Click()
        {
            if(GameManager.Instance.gameMode != GameMode.Room) return;

            Player player = GameManager.Instance.player;
            if(Vector2.Distance(player.transform.position, transform.position) > player.interactionRange)
            {
                GameManager.Instance.CreateTextEffect("Too far", Color.red, transform.position);
            }
        }

        protected void OnMouseDown()
        {
            Click();
        }

        protected virtual void OnMouseEnter()
        {
            if(GameManager.Instance.gameMode != GameMode.Room) return;
            //label.SetActive(true);
        }

        protected virtual void OnMouseExit()
        {
            //label.SetActive(false);
        }

        public virtual void Init(Room room, string realPath)
        {
            this.room = room;
            this.realPath = realPath;
            displayPath = GameManager.Instance.obfuscate ? GameManager.Instance.ObfuscatePath(realPath, this is GroundDir) : realPath;

            InitRandom();

            //// Generate position
            //transform.localPosition = room.RandomPosition(random);

            // Debug
            GetComponentInChildren<TextMeshProUGUI>(true).text = Path.GetFileName(displayPath);
        }

        protected abstract void InitRandom();
    }
}