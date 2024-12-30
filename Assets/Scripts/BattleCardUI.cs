using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BattleCardUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public BattleUI battleUI;
        public Vector2 target;
        public AnimationCurve moveCurve;
        public float moveTime;
        public bool erase;

        public CardUI cardUI;
        public GameObject cardBack;
        public Card card;
        public bool hovered = false;
        public bool selected = false;

        Coroutine moveCoro = null;

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

        public void SetTarget(Vector2 newTarget)
        {
            // If already near position
            if(Vector2.SqrMagnitude(newTarget - (Vector2) transform.localPosition) < 0.01f)
            {
                transform.localPosition = newTarget;
                target = newTarget;
                return;
            }

            // If same target
            if(newTarget == target)
            {
                return;
            }

            // Move
            if(moveCoro != null)
            {
                StopCoroutine(moveCoro);
            }
            target = newTarget;
            moveCoro = StartCoroutine(CMoveTo());
        }

        private IEnumerator CMoveTo()
        {
            Vector2 start = transform.localPosition;
            for(float t = 0f; t < moveTime; t += Time.deltaTime)
            {
                transform.localPosition = Vector2.Lerp(start, target, moveCurve.Evaluate(t / moveTime));
                yield return null;
            }
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