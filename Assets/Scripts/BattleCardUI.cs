using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BattleCardUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public BattleUI battleUI;
        public bool erase;

        public CardUI cardUI;
        public GameObject cardBack;
        public Card card;
        public bool hovered = false;
        public bool selected = false;

        // Use this for initialization
        void Start()
        {
            //target = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPointerDown(PointerEventData ev)
        {
            if(!battleUI.InHand(this)) return;

            if(card.fileSize > GameManager.Instance.mp)
            {
                GameManager.Instance.CreateTextEffect("Insufficient Memory", Color.red, transform.position);
                return;
            }

            hovered = false;

            if(card.requiresTarget)
            {
                battleUI.selectedCard = this;
                GameManager.Instance.CreateTextEffect("Select target", Color.blue, transform.position);
                selected = true;
            }    
            else
                battleUI.PlayCard(this);
        }

        public void OnPointerEnter(PointerEventData ev)
        {
            if(!battleUI.InHand(this)) return;

            hovered = true;
            transform.SetAsLastSibling();
        }

        public void OnPointerExit(PointerEventData ev)
        {
            if(!battleUI.InHand(this)) return;

            hovered = false;
            battleUI.OrderHand();
        }

        public void SetCard(Card card)
        {
            this.card = card;
            cardUI.SetCard(card);
            cardBack.GetComponent<Image>().sprite = GameManager.Instance.cardSprites.GetBackSprite(card.type);
        }

        public void Reveal(bool reveal)
        {
            cardUI.gameObject.SetActive(reveal);
            cardBack.SetActive(!reveal);
        }
    }
}