using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CardUI : MonoBehaviour
    {
        public TextMeshProUGUI title;
        public TextMeshProUGUI description;
        public TextMeshProUGUI fileSize;
        public Image image;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetCard(Card card)
        {
            title.text = card.name;
            description.text = card.GetDescription();
            fileSize.text = Utils.FileSizeString(card.fileSize);
            image.sprite = card.sprite;
            GetComponent<Image>().sprite = GameManager.Instance.cardSprites.GetFrontSprite(card.type);
        }
    }
}