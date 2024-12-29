﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class BattleUI : MonoBehaviour
    {
        public Transform hand;
        public float handCardDistance = 10f;
        public float pileCardDistance = 2f;
        public float enemyDistance = 50f;

        public BarUI playerHpBar;
        public TextMeshProUGUI playerMpText;

        [Space(10)]
        public Transform drawPile;
        public Transform discardPile;
        public Transform erasePile;
        public Transform enemyParent;

        public GameObject battleCardPrefab;
        public GameObject enemyPrefab;

        [Space(10)]
        public List<BattleCardUI> handCards;
        public List<BattleCardUI> drawCards;
        public List<BattleCardUI> discardedCards;
        public List<BattleCardUI> erasedCards;
        public List<BattleCardUI> deck;
        public List<EnemyUI> enemies;
        public EnemyUI targetEnemy;

        public bool playerTurn = false;
        public string encounterId;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            GameManager gm = GameManager.Instance;

            if(gm.gameMode != GameMode.Battle) return;

            // Move cards to their positions

            // Hand
            for(int i = 0; i < handCards.Count; i++)
            {
                Vector2 target = (i - handCards.Count / 2f) * handCardDistance * Vector2.right;
                handCards[i].SetTarget(target);
            }

            // Draw pile
            for(int i = 0; i < drawCards.Count; i++)
            {
                Vector2 target = i * pileCardDistance * Vector2.up;
                drawCards[i].SetTarget(target);
            }

            // Discard pile
            for(int i = 0; i < discardedCards.Count; i++)
            {
                Vector2 target = i * pileCardDistance * Vector2.up;
                discardedCards[i].SetTarget(target);
            }

            // HP bar
            playerHpBar.maxValue = gm.maxHp;
            playerHpBar.value = gm.hp;

            // MP text
            playerMpText.text = $"Allocated RAM: {Utils.FileSizeString(gm.mp)} / {Utils.FileSizeString(gm.maxMp)}";
        }

        public void PlayCard(BattleCardUI card)
        {
            if(card.card.Attack > 0)
            {
                AttackEnemy(targetEnemy, card.card.Attack);
            }
            if(card.card.Defense > 0)
            {
                GameManager.Instance.block += card.card.Defense;
            }

            if(card.card.Erase)
                Erase(card);
            else
                Discard(card);
        }

        public void Discard(BattleCardUI card)
        {
            handCards.Remove(card);
            discardedCards.Add(card);
            card.card.OnDiscard(GetContext());

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
                card.Reveal(false);
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

                BattleCardUI drawnCard = drawCards[^1]; // Because the last card is on top
                drawCards.Remove(drawnCard);
                handCards.Add(drawnCard);
                drawnCard.transform.SetParent(hand);
                drawnCard.Reveal(true);
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

        public void StartBattle(Enemy[] enemies, string encounterId)
        {
            StartCoroutine(CBattle(enemies, encounterId));
        }

        public void CreateHandCard(Card card) => AddCard(card, handCards, hand, true);
        public void CreateDrawCard(Card card) => AddCard(card, drawCards, drawPile, false);
        public void CreateDiscardedCard(Card card) => AddCard(card, discardedCards, discardPile, false);

        private void AddCard(Card card, List<BattleCardUI> pile, Transform parent, bool reveal)
        {
            GameObject go = Instantiate(battleCardPrefab, parent);
            BattleCardUI bc = go.GetComponent<BattleCardUI>();
            bc.SetCard(card);
            bc.Reveal(reveal);
        }

        public void AttackPlayer(long damage)
        {
            GameManager.Instance.block -= damage;
            if(GameManager.Instance.block < 0)
            {
                GameManager.Instance.hp += GameManager.Instance.block;
                GameManager.Instance.block = 0;
            }
            if(GameManager.Instance.hp <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }

        public void AttackEnemy(EnemyUI enemy, long damage)
        {
            enemy.enemy.block -= damage;
            if(enemy.enemy.block < 0)
            {
                enemy.enemy.hp += enemy.enemy.block;
                enemy.enemy.block = 0;
            }
            if(enemy.enemy.hp <= 0)
            {
                enemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }
        }

        public BattleContext GetContext()
        {
            return new BattleContext
            {
                BattleUI = this
            };
        }

        private IEnumerator CBattle(Enemy[] enemies, string encounterId)
        {
            this.encounterId = encounterId;

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
                bc.battleUI = this;
                bc.SetCard(card);
                bc.Reveal(false);
            }

            Shuffle();

            // Delete old enemies
            foreach(Transform child in enemyParent)
            {
                Destroy(child.gameObject);
            }

            // Spawn new enemies
            int enemyCount = enemies.Length;
            for(int i = 0; i < enemyCount; i++)
            {
                GameObject go = Instantiate(enemyPrefab, enemyParent);
                go.transform.localPosition = (i - (enemyCount - 1) / 2f) * enemyDistance * Vector2.right;
                EnemyUI e = go.GetComponent<EnemyUI>();
                e.SetEnemy(enemies[i]);
                this.enemies.Add(e);
            }
            targetEnemy = this.enemies[0];

            // Draw initial cards

            while(true)
            {
                Debug.Log("Drawing cards");
                yield return CDraw(5);

                playerTurn = true;
                while(playerTurn)
                    yield return null; // Wait for player to end the turn

                Debug.Log("Player turn ended");

                // Notify all cards that the turn ended
                foreach(BattleCardUI card in handCards)
                {
                    card.card.OnTurnEnd(GetContext());
                }

                // Discard all hand cards
                List<BattleCardUI> handCardsCopy = handCards.ToList();
                foreach(BattleCardUI card in handCardsCopy)
                {
                    if(card.card.Volatile)
                    {
                        Erase(card);
                    }
                    else if(!card.card.Keep)
                    {
                        Discard(card);
                    }
                }

                // Execute enemy actions
                foreach(Enemy enemy in enemies)
                {
                    enemy.DoTurn(GetContext());
                    yield return new WaitForSeconds(1.0f);
                }
            }
        }

        private IEnumerator CDraw(int count)
        {
            for(int i = 0; i < count; i++)
            {
                Draw(1);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}