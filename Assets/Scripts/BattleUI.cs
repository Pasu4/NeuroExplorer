using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class BattleUI : MonoBehaviour
    {
        public Transform hand;
        public float handCardDistance = 10f;

        [Space(10)]
        public Transform drawPile;
        public Transform discardPile;
        public Transform erasePile;

        public List<CardUI> handCards;
        public List<CardUI> drawCards;
        public List<CardUI> discardedCards;
        public List<CardUI> erasedCards;
        public List<CardUI> deck;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(GameManager.Instance.gameMode != GameMode.Battle) return;

            // Move hand cards to their positions
            for(int i = 0; i < handCards.Count; i++)
            {
                Vector2 target = (i - handCards.Count / 2f) * handCardDistance * Vector2.right;
                if(Vector2.SqrMagnitude(target - handCards[i].target) > 0.01f)
                {
                    handCards[i].SetTarget(target);
                }
            }
        }

        public void Discard(CardUI card)
        {
            handCards.Remove(card);
            discardedCards.Add(card);

            card.transform.SetParent(discardPile);
            card.SetTarget(Vector2.zero);
        }

        public void Erase(CardUI card)
        {
            handCards.Remove(card);
            deck.Remove(card);
            card.card.OnErase(GetContext());

            card.transform.SetParent(erasePile);
            card.SetTarget(Vector2.zero);
        }

        public void Reshuffle()
        {
            drawCards.AddRange(discardedCards);
            foreach(CardUI card in discardedCards)
            {
                card.transform.SetParent(drawPile);
            }
            discardedCards.Clear();
            Shuffle();
        }

        public void Shuffle()
        {
            drawCards = drawCards.OrderBy(_ => Random.value).ToList();
            for(int i = 0; i < drawCards.Count; i++)
            {
                drawCards[i].transform.SetSiblingIndex(i);
            }
        }

        public void Draw(int count)
        {
            for(int i = 0; i < count; i++)
            {
                // Cannot draw more cards
                if(drawCards.Count == 0 && discardedCards.Count == 0)
                    break;

                if(drawCards.Count == 0)
                    Reshuffle();

                CardUI drawnCard = drawCards[^1];
                drawCards.Remove(drawnCard);
                handCards.Add(drawnCard);
                drawnCard.transform.SetParent(hand);
                drawnCard.card.OnEnterHand(GetContext());
            }
        }

        public void OrderHand()
        {
            for(int i = 0; i < handCards.Count; i++)
            {
                handCards[i].transform.SetSiblingIndex(i);
            }
        }

        public bool InHand(CardUI card)
        {
            return handCards.Contains(card);
        }

        public BattleContext GetContext()
        {
            return new BattleContext
            {
                BattleUI = this
            };
        }
    }
}