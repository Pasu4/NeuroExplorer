using Assets.Scripts.CardEffects;
using System.Collections;
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
        public bool isSpecial = false;

        public BarUI playerHpBar;
        public TextMeshProUGUI playerMpText;
        public TextMeshProUGUI playerHpText;
        public Transform hpEffectSpawn;

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
        public BattleCardUI selectedCard;
        public string encounterId;
        public bool battleWon = false;

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
                Vector2 target = new(-((i - (handCards.Count - 1) / 2f) * handCardDistance), handCards[i].hovered || handCards[i].selected ? 16 : 0);
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

            // Erase pile
            for(int i = 0; i < erasedCards.Count; i++)
            {
                erasedCards[i].SetTarget(Vector2.zero);
            }

            // HP bar
            playerHpBar.maxValue = gm.maxHp;
            playerHpBar.value = gm.hp;

            playerHpText.text = $"Data Integrity: {Utils.FileSizeString(gm.hp)} / {Utils.FileSizeString(gm.maxHp)}";

            // MP text
            playerMpText.text = $"Backup Memory: {Utils.FileSizeString(gm.block)}\n"
                + $"Available Memory: {Utils.FileSizeString(gm.mp)} / {Utils.FileSizeString(gm.maxMp)}";
        }

        public void PlayCard(BattleCardUI card)
        {
            selectedCard = null;
            foreach(BattleCardUI c in handCards)
                c.selected = false;

            // Allow playing only mutex cards
            if(handCards.Any(c => c.card.cardEffects.Any(e => e is MutexEffect)) && card.card.cardEffects.All(e => e is not MutexEffect))
            {
                GameManager.Instance.CreateTextEffect("Deadlock", Color.red, card.transform.position);
                return;
            }

            GameManager.Instance.mp -= card.card.fileSize;

            if(card.card.attack > 0)
            {
                AttackEnemy(targetEnemy, card.card.attack);
                GameManager.Instance.CreateTextEffect("-" + Utils.FileSizeString(card.card.attack), Color.red, targetEnemy.transform.position);
            }
            if(card.card.defense > 0)
            {
                GameManager.Instance.block += card.card.defense;
            }

            card.card.OnPlay(GetContext());

            if(card.card.erase)
                Erase(card);
            else
                Discard(card);

            CheckGame();
        }

        public void Discard(BattleCardUI card)
        {
            handCards.Remove(card);
            discardedCards.Add(card);
            card.card.OnDiscard(GetContext());

            card.transform.SetParent(discardPile);
            card.targetValid = false;
            // card.SetTarget(Vector2.zero);
        }

        public void Erase(BattleCardUI card)
        {
            handCards.Remove(card);
            deck.Remove(card);
            card.card.OnErase(GetContext());

            card.transform.SetParent(erasePile);
            card.targetValid = false;
            card.SetTarget(Vector2.zero);
        }

        public void Reshuffle()
        {
            drawCards.AddRange(discardedCards);
            foreach(BattleCardUI card in discardedCards)
            {
                card.transform.SetParent(drawPile);
                card.targetValid = false;
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
                drawnCard.targetValid = false;
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

        public void CheckGame()
        {
            if(GameManager.Instance.hp < 0)
            {
                GameManager.Instance.GameOver();
            }
            if(enemies.Count == 0 && !isSpecial)
            {
                GameManager.Instance.BattleWin(!isSpecial);
            }
            battleWon = enemies.Count == 0 && isSpecial;
        }

        public void StartBattle(Enemy[] enemies, string encounterId, bool isSpecial)
        {
            StopAllCoroutines(); // Just for safety
            StartCoroutine(CBattle(enemies, encounterId, isSpecial));
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
            pile.Add(bc);
            deck.Add(bc);
            bc.battleUI = this;
        }

        public void AttackPlayer(long damage)
        {
            GameManager.Instance.sfxSource.PlayOneShot(GameManager.Instance.hitClip);
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

            GameManager.Instance.CreateTextEffect("-" + Utils.FileSizeString(damage), Color.red, hpEffectSpawn.position, Vector2.down);
        }

        public void AttackEnemy(EnemyUI enemy, long damage)
        {
            GameManager.Instance.sfxSource.PlayOneShot(GameManager.Instance.hitClip);
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
                battleUI = this
            };
        }

        private IEnumerator CBattle(Enemy[] enemies, string encounterId, bool isSpecial)
        {
            battleWon = false;
            this.encounterId = encounterId;
            this.isSpecial = isSpecial;

            // Delete old cards
            foreach(BattleCardUI c in deck)
                if(c) Destroy(c.gameObject);

            foreach(BattleCardUI c in erasedCards)
                if(c) Destroy(c.gameObject);

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
                drawCards.Add(bc);
                deck.Add(bc);
            }

            Shuffle();

            // Delete old enemies
            foreach(Transform child in enemyParent)
            {
                Destroy(child.gameObject);
            }
            this.enemies.Clear();

            // Spawn new enemies
            int enemyCount = enemies.Length;
            for(int i = 0; i < enemyCount; i++)
            {
                GameObject go = Instantiate(enemyPrefab, enemyParent);
                go.transform.localPosition = (i - (enemyCount - 1) / 2f) * enemyDistance * Vector2.right;
                EnemyUI e = go.GetComponent<EnemyUI>();
                e.SetEnemy(enemies[i]);
                e.battleUI = this;
                this.enemies.Add(e);
            }
            targetEnemy = this.enemies[0];

            while(true)
            {

                GameManager.Instance.mp = GameManager.Instance.maxMp;
                GameManager.Instance.block = 0;
                Debug.Log("Drawing cards");
                yield return CDraw(5);

                if(deck.Count(c => c.card.cardEffects.Any(e => e is SemaphoreEffect)) >= 4)
                {
                    GameManager.Instance.GameOver();
                }

                playerTurn = true;
                while(playerTurn && !battleWon)
                    yield return null; // Wait for player to end the turn

                if(battleWon)
                    yield break;

                // Notify all cards that the turn ended
                foreach(BattleCardUI card in handCards)
                {
                    card.card.OnTurnEnd(GetContext());
                }

                // Discard all hand cards
                List<BattleCardUI> handCardsCopy = handCards.ToList();
                foreach(BattleCardUI card in handCardsCopy)
                {
                    if(card.card.@volatile)
                    {
                        Erase(card);
                    }
                    else if(!card.card.keep)
                    {
                        Discard(card);
                    }
                }

                // Execute enemy actions
                foreach(EnemyUI enemy in this.enemies)
                {
                    var ctx = GetContext();
                    ctx.activeEnemy = enemy;
                    enemy.enemy.DoTurn(ctx);
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

        public void EndPlayerTurn()
        {
            playerTurn = false;
        }
    }
}