using Assets.Scripts.CardEffects;
using Assets.Scripts.Integration.Actions;
using NeuroSdk.Actions;
using NeuroSdk.Messages.Outgoing;
using Newtonsoft.Json;
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
        public float handCardMaxDistance = 10f;
        public float handCardMaxOffset = 50f;
        public float pileCardDistance = 2f;
        public float enemyMaxDistance = 50f;
        public float enemyMaxOffset = 100f;
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
        public TextAsset neuroBattleTutorial;

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
        public int turn = 0;
        public bool waitingForAction = false;
        private bool firstBattle = true;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            GameManager gm = GameManager.Instance;

            if(gm.gameMode != GameMode.Battle) return;

            // Move objects to their positions

            // Hand
            for(int i = 0; i < handCards.Count; i++)
            {
                Vector2 target = Utils.DistributeBetweenCentered(
                    Vector2.right * handCardMaxOffset, Vector2.left * handCardMaxOffset,
                    handCards.Count, i, handCardMaxDistance
                );
                if(handCards[i].hovered || handCards[i].selected)
                    target += Vector2.up * 16;
                handCards[i].GetComponent<MoveObject>().SetTarget(target);
            }

            // Draw pile
            for(int i = 0; i < drawCards.Count; i++)
            {
                Vector2 target = i * pileCardDistance * Vector2.up;
                drawCards[i].GetComponent<MoveObject>().SetTarget(target);
            }

            // Discard pile
            for(int i = 0; i < discardedCards.Count; i++)
            {
                Vector2 target = i * pileCardDistance * Vector2.up;
                discardedCards[i].GetComponent<MoveObject>().SetTarget(target);
            }

            // Erase pile
            for(int i = 0; i < erasedCards.Count; i++)
            {
                erasedCards[i].GetComponent<MoveObject>().SetTarget(Vector2.zero);
            }

            // Enemies
            for(int i = 0; i < enemies.Count; i++)
            {
                Vector2 target = Utils.DistributeBetweenCentered(
                    Vector2.left * enemyMaxOffset, Vector2.right * enemyMaxOffset,
                    enemies.Count, i, enemyMaxDistance
                );
                enemies[i].GetComponent<MoveObject>().SetTarget(target);
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
                if(card.card.multi)
                {
                    foreach(EnemyUI enemy in enemies.ToList()) // Copy
                    {
                        AttackEnemy(enemy, card.card.attack);
                        GameManager.Instance.CreateTextEffect("-" + Utils.FileSizeString(card.card.attack), Color.red, enemy.transform.position);
                    }
                }
                else
                {
                    AttackEnemy(targetEnemy, card.card.attack);
                    GameManager.Instance.CreateTextEffect("-" + Utils.FileSizeString(card.card.attack), Color.red, targetEnemy.transform.position);
                }
            }
            if(card.card.defense > 0)
            {
                GameManager.Instance.block += card.card.defense;
            }

            card.card.OnPlay(GetContext());

            if(card.card.erase)
                Erase(card);
            else if(!card.card.async)
                Discard(card);

            CheckGame();
        }

        public void Discard(BattleCardUI card)
        {
            // Make absolutely sure this card is not in multiple piles at once
            handCards.Remove(card);
            drawCards.Remove(card);
            discardedCards.Remove(card);
            erasedCards.Remove(card);
            deck.Remove(card);

            discardedCards.Add(card);
            deck.Add(card);
            card.card.OnDiscard(GetContext());

            card.transform.SetParent(discardPile);
            card.GetComponent<MoveObject>().targetValid = false;
            // card.SetTarget(Vector2.zero);
        }

        public void Erase(BattleCardUI card)
        {
            // Make absolutely sure this card is not in multiple piles at once
            handCards.Remove(card);
            drawCards.Remove(card);
            discardedCards.Remove(card);
            erasedCards.Remove(card);
            deck.Remove(card);

            erasedCards.Add(card);
            card.card.OnErase(GetContext());

            card.transform.SetParent(erasePile);
            card.GetComponent<MoveObject>().targetValid = false;
            card.GetComponent<MoveObject>().SetTarget(Vector2.zero);
        }

        public void Reshuffle()
        {
            drawCards.AddRange(discardedCards);
            foreach(BattleCardUI card in discardedCards)
            {
                card.transform.SetParent(drawPile);
                card.GetComponent<MoveObject>().targetValid = false;
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
                drawnCard.GetComponent<MoveObject>().targetValid = false;
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
            //GameManager.Instance.IntegrationRoomEnd();
            StopAllCoroutines(); // Just for safety
            StartCoroutine(CBattle(enemies, encounterId, isSpecial));
        }

        public void CreateHandCard(Card card, Vector2 source) => AddCard(card, handCards, hand, true, source);
        public void CreateHandCard(Card card) => AddCard(card, handCards, hand, true, hand.transform.position);
        public void CreateDrawCard(Card card, Vector2 source) => AddCard(card, drawCards, drawPile, false, source);
        public void CreateDrawCard(Card card) => AddCard(card, drawCards, drawPile, false, drawPile.transform.position);
        public void CreateDiscardedCard(Card card, Vector2 source) => AddCard(card, discardedCards, discardPile, true, source);
        public void CreateDiscardedCard(Card card) => AddCard(card, discardedCards, discardPile, true, discardPile.transform.position);

        private void AddCard(Card card, List<BattleCardUI> pile, Transform parent, bool reveal, Vector2 source)
        {
            GameObject go = Instantiate(battleCardPrefab, parent);
            go.transform.position = source;
            BattleCardUI bc = go.GetComponent<BattleCardUI>();
            bc.SetCard(card);
            bc.Reveal(reveal);
            pile.Add(bc);
            deck.Add(bc);
            bc.battleUI = this;
        }

        public EnemyUI CreateEnemy(Enemy enemy, Vector2 position)
        {
            enemy = enemy.Copy();

            GameObject go = Instantiate(enemyPrefab, enemyParent);
            go.transform.position = position;
            EnemyUI e = go.GetComponent<EnemyUI>();
            e.SetEnemy(enemy);
            e.battleUI = this;
            enemies.Add(e);
            return e;
        }

        public void AttackPlayer(long damage)
        {
            GameManager.Instance.sfxSource.PlayOneShot(GameManager.Instance.sfx.hit);
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
            GameManager.Instance.sfxSource.PlayOneShot(GameManager.Instance.sfx.hit);
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

        public string GetActionState()
        {
            var state = new
            {
                enemies = enemies.Select(e => new
                {
                    e.enemy.name,
                    hp = Utils.FileSizeString(e.enemy.hp),
                    block = Utils.FileSizeString(e.enemy.block),
                    nextAction = e.enemy.nextAction.NeuroDescription
                }),
                handCards = handCards.Select((c, index) => new
                {
                    index,
                    c.card.name,
                    c.card.type,
                    mpCost = Utils.FileSizeString(c.card.fileSize),
                    c.card.requiresTarget,
                    description = c.card.GetDescription().TrimRTF(),
                }),
                playerHp = Utils.FileSizeString(GameManager.Instance.hp),
                playerMp = Utils.FileSizeString(GameManager.Instance.mp),
                playerBlock = Utils.FileSizeString(GameManager.Instance.block),
                drawPileCount = drawCards.Count,
                discardPileCount = discardedCards.Count
            };
            return JsonConvert.SerializeObject(state, Formatting.None);
        }

        private IEnumerator CBattle(Enemy[] enemies, string encounterId, bool isSpecial)
        {
            battleWon = false;
            turn = 0;
            this.encounterId = encounterId;
            this.isSpecial = isSpecial;
            enemies = enemies.Select(e => e.Copy()).ToArray();

            if(firstBattle)
            {
                firstBattle = false;
                Context.Send(neuroBattleTutorial.text);
            }

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
            Dictionary<string, int> enemyIndices = enemies
                .GroupBy(e => e.name, (name, es) => (name, es.Count()))
                .Where(g => g.Item2 > 1)
                .ToDictionary(g => g.name, _ => 0);

            for(int i = 0; i < enemyCount; i++)
            {
                if(enemyIndices.ContainsKey(enemies[i].name))
                {
                    enemies[i].name += " " + (char) ('A' + enemyIndices[enemies[i].name]++);
                }

                CreateEnemy(enemies[i], enemyParent.position);
            }
            targetEnemy = this.enemies[0];

            while(true)
            {

                GameManager.Instance.mp = GameManager.Instance.maxMp;
                GameManager.Instance.block = 0;
                yield return CDraw(5);

                if(deck.Count(c => c.card.cardEffects.Any(e => e is SemaphoreEffect)) >= 4)
                {
                    GameManager.Instance.GameOver();
                }

                playerTurn = true;

                // Wait for player to end the turn
                while(playerTurn && !battleWon)
                    yield return CPlayerAction();

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

                turn++;
            }
        }

        private IEnumerator CPlayerAction()
        {
            waitingForAction = true;
            ActionWindow.Create(gameObject)
                .AddAction(new PlayCardAction(this))
                .AddAction(new EndTurnAction())
                .SetForce(0, "Either play a card or end your turn. Only end your turn if you don't want to play any more cards this turn.", GetActionState(), true)
                .SetEnd(() => !(waitingForAction && playerTurn && !battleWon))
                .Register();

            while(waitingForAction && playerTurn && !battleWon) yield return null;

            yield return null; // Wait for actions to be unregistered
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