using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class AirisEnemy : Enemy
    {
        private int count;

        public AirisEnemy(Sprite sprite)
        {
            name = "AIris";
            this.sprite = sprite;
            maxHp = 15_000_000 + GameManager.Instance.difficulty * 10_000_000;
            strength = 2_000_000 + GameManager.Instance.difficulty * 1_000_000;
            attackFactor = 1.0f;
            defendFactor = 1.0f;
        }

        public override EnemyAction ChooseNextAction(BattleContext ctx)
        {
            if(ctx.battleUI.enemies.Count == 1)
            {
                return new SummonAction(EnemyResources.NeuroYukkuri);
            }

            switch(count++)
            {
                case 0:
                    return new TrojanAction(CardResources.Semaphore);

                case 1:
                    return new AttackAction(Attack);

                case 2:
                    count = 0;
                    return new DefendAction(Defense);

                default:
                    count = 0;
                    return null;
            }
        }
    }
}
