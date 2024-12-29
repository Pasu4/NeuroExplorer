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

        Coroutine moveCoro = null;

        // Use this for initialization
        void Start()
        {
            target = transform.position;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPointerDown(PointerEventData ev)
        {
            if(!battleUI.InHand(this)) return;

            if(card.FileSize > GameManager.Instance.mp)
            {
                GameManager.Instance.CreateTextEffect("Not enoungh RAM", Color.red, transform.position);
                return;
            }

            battleUI.PlayCard(this);
        }

        public void OnPointerEnter(PointerEventData ev)
        {
            if(!battleUI.InHand(this)) return;

            transform.SetAsLastSibling();
        }

        public void OnPointerExit(PointerEventData ev)
        {
            if(!battleUI.InHand(this)) return;

            battleUI.OrderHand();
        }

        public void SetTarget(Vector2 newTarget)
        {
            if(Vector2.SqrMagnitude(newTarget - (Vector2) transform.position) < 0.01f)
            {
                transform.position = newTarget;
                return;
            }
            if(moveCoro != null)
            {
                StopCoroutine(moveCoro);
            }
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
            cardBack.GetComponent<Image>().sprite = GameManager.Instance.cardSprites.GetBackSprite(card.Type);
        }

        public void Reveal(bool reveal)
        {
            cardUI.gameObject.SetActive(reveal);
            cardBack.SetActive(!reveal);
        }
    }
}