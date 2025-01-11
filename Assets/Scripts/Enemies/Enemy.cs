using NeuroSdk.Messages.Outgoing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public abstract class Enemy
    {
        public string name;
        public long strength;
        public long hp = 100000;
        public long maxHp = 100000;
        public long block = 0;
        public Sprite sprite;
        public float attackFactor = 1.0f;
        public float defendFactor = 1.0f;

        public EnemyAction nextAction = new DoNothingAction();

        public long Attack => (long) (strength * attackFactor);
        public long Defense => (long) (strength * defendFactor);

        public Enemy Copy()
        {
            return (Enemy) MemberwiseClone();
        }

        public virtual void Init(BattleContext ctx)
        {
            hp = maxHp;
            nextAction = ChooseNextAction(ctx);
        }

        public virtual void DoTurn(BattleContext ctx)
        {
            block = 0;

            nextAction.Execute(ctx);

            nextAction = ChooseNextAction(ctx);
        }

        public abstract EnemyAction ChooseNextAction(BattleContext context);
    }
}
