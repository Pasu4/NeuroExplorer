using System;
using URandom = UnityEngine.Random;

namespace Assets.Scripts.Enemies
{
    public class SimpleEnemy : Enemy
    {
        public Func<BattleContext, SimpleEnemy, EnemyAction> nextActionProvider = (ctx, self) => new DoNothingAction();

        public override EnemyAction ChooseNextAction(BattleContext context)
        {
            return nextActionProvider(context, this);
        }
    }
}
