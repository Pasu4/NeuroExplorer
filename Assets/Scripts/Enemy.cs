using NeuroSdk.Messages.Outgoing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class Enemy
    {
        public string name;
        public long strength;
        public long hp = 100000;
        public long maxHp = 100000;
        public long block = 0;
        public Sprite sprite;
        public Card trojanCard;
        public int attackWeight = 10;
        public int defenseWeight = 10;
        public int trojanWeight = 10;
        public float attackFactor = 1.0f;
        public float defendFactor = 1.0f;
        public int trojanCount = 1;
        [Range(0, 1)]
        public float multiChance = 0.25f;
        public int maxMulti = 2;
        public string trojanId = "";
        public bool targetDrawPile = false;

        public EnemyAction nextAction;

        public Enemy Copy()
        {
            return (Enemy) MemberwiseClone();
        }

        public virtual void Init()
        {
            maxHp = (long) (maxHp * GameManager.Instance.enemyHpScale * Random.Range(0.8f, 1.2f));
            hp = maxHp;

            nextAction ??= Utils.ChooseWeighted<EnemyAction>(new System.Random(),
                (attackWeight, new AttackAction((long) (strength * Random.Range(0.8f, 1.2f) * attackFactor), Random.value < multiChance ? Random.Range(2, maxMulti) : 1)),
                (defenseWeight, new DefendAction((long) (strength * Random.Range(0.8f, 1.2f) * defendFactor))),
                (trojanWeight, new TrojanAction())
            );

            if(!string.IsNullOrEmpty(trojanId))
            {
                trojanCard = CardResources.GetCard(trojanId);
            }
        }

        public virtual void DoTurn(BattleContext ctx)
        {
            block = 0;

            nextAction.Execute(ctx);

            nextAction = ChooseNextAction(ctx);
        }

        public virtual EnemyAction ChooseNextAction(BattleContext context)
        {
            return Utils.ChooseWeighted<EnemyAction>(new System.Random(),
                (attackWeight, new AttackAction((long) (strength * Random.Range(0.8f, 1.2f) * attackFactor), Random.value < multiChance ? Random.Range(2, maxMulti) : 1)),
                (defenseWeight, new DefendAction((long) (strength * Random.Range(0.8f, 1.2f) * defendFactor))),
                (trojanWeight, new TrojanAction())
            );
        }
    }
}
