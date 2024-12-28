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

        public GameObject battleCardPrefab;

        public List<BattleCardUI> handCards;
        public List<BattleCardUI> drawCards;
        public List<BattleCardUI> discardedCards;
        public List<BattleCardUI> erasedCards;
        public List<BattleCardUI> deck;

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

        public void Discard(BattleCardUI card)
        {
            handCards.Remove(card);
            discardedCards.Add(card);

            card.transform.SetParent(discardPile);
            card.SetTarget(Vector2.zero);
        }

        public void Erase(BattleCardUI card)
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
            foreach(BattleCardUI card in discardedCards)
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

                BattleCardUI drawnCard = drawCards[^1];
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

        public bool InHand(BattleCardUI card)
        {
            return handCards.Contains(card);
        }

        public void StartBattle()
        {
            // Delete old cards
            foreach(BattleCardUI c in deck)
                if(c) Destroy(c);

            foreach(BattleCardUI c in erasedCards)
                if(c) Destroy(c);

            handCards.Clear();
            drawCards.Clear();
            discardedCards.Clear();
            erasedCards.Clear();
            deck.Clear();

            // Spawn new cards
            foreach(Card card in GameManager.Instance.deck)
            {
                GameObject go = Instantiate(battleCardPrefab, drawPile);
                BattleCardUI bc = go.GetComponent<BattleCardUI>();
                bc.SetCard(card);
            }

            // TODO?
            Draw(5);
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