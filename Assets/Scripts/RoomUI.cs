using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class RoomUI : MonoBehaviour
    {
        public TextMeshProUGUI storageText;
        public BarUI storageBar;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            GameManager gm = GameManager.Instance;

            storageText.text = $"Free storage: {Utils.FileSizeString(gm.FreeStorage)} / {Utils.FileSizeString(gm.inventorySpace)}";
            storageBar.maxValue = gm.inventorySpace;
            storageBar.value = gm.UsedStorage;
        }
    }
}