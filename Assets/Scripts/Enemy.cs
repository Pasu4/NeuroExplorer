using System;
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
        public long strength;
        public long hp;
        public long maxHp;
        public long block;
        public Sprite sprite;

        public EnemyAction nextAction;

        public virtual void DoTurn(BattleContext ctx)
        {
            block = 0;
            if(nextAction is AttackAction aAct)
                DoAttackAction(ctx, aAct);
            else if(nextAction is DefendAction dAct)
                DoDefendAction(ctx, dAct);
            else if(nextAction is TrojanAction tAct)
                DoTrojanAction(ctx, tAct);
        }

        public virtual void DoAttackAction(BattleContext ctx, AttackAction action)
        {
            for(int i = 0; i < action.times; i++)
            {
                ctx.BattleUI.AttackPlayer(action.damage);
            }
        }

        public virtual void DoDefendAction(BattleContext ctx, DefendAction action)
        {
            block = action.block;
        }

        public virtual void DoTrojanAction(BattleContext ctx, TrojanAction action)
        {
            ctx.BattleUI.CreateDiscardedCard(new Card
            {
                Name = "Null Pointer",
                Erase = true,
                FileSize = 5_000L,
                Sprite = GameManager.Instance.GetFileSprite("Null Pointer")
            });
        }

        public virtual EnemyAction ChooseNextAction(BattleContext context)
        {
            if(nextAction is AttackAction)
                return new DefendAction(strength);
            else
                return new AttackAction(strength);
        }
    }
}
