using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public abstract class GroundObject : MonoBehaviour
    {
        public string realPath;
        public string displayPath;
        public string DisplayName => Path.GetFileName(displayPath);

        protected System.Random random;

        // Use this for initialization
        protected virtual void Start()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {

        }

        public virtual void Init(Room room, string realPath)
        {
            this.realPath = realPath;
            displayPath = GameManager.Instance.obfuscate ? GameManager.Instance.ObfuscatePath(realPath) : realPath;

            InitRandom();

            // Generate position
            transform.localPosition = room.RandomPosition(random);

            // Debug
            GetComponentInChildren<TextMeshProUGUI>().text = Path.GetFileName(displayPath);
        }

        protected abstract void InitRandom();
    }
}