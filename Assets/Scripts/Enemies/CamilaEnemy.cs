using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class CamilaEnemy : Enemy
    {
        private int count = 0;
        private int phase = 1;

        public CamilaEnemy(Sprite sprite)
        {
            name = "AImila";
            this.sprite = sprite;
            maxHp = 4_000_000 + GameManager.Instance.difficulty * 1_000_000;
            strength = 200_000 + GameManager.Instance.difficulty * 100_000;
            attackFactor = 1.0f;
            defendFactor = 1.0f;
        }

        public override void DoTurn(BattleContext ctx)
        {
            if(hp < maxHp / 2 && phase == 1)
            {
                phase = 2;
                count = 0;
                attackFactor *= 1.5f;

                GameManager.Instance.CreateTextEffect("Phase change", Color.red, (Vector2) ctx.activeEnemy.transform.position + Vector2.up * 0.5f);
            }

            base.DoTurn(ctx);
        }

        public override EnemyAction ChooseNextAction(BattleContext ctx)
        {
            if(ctx.battleUI.enemies.Count == 1)
            {
                return new SummonAction(EnemyResources.NeuroYukkuri);
            }

            // Phase 1
            if(phase == 1)
            {
                switch(count++)
                {
                    case 0:
                        return new TrojanAction(CardResources.Mutex);

                    case 1:
                        return new AttackAction(Attack);

                    case 2:
                        return new DefendAction(Defense);

                    case 3:
                        return new AttackAction(Attack);

                    case 4:
                        count = 0;
                        return new DefendAction(Defense);

                    default:
                        count = 0;
                        return new DoNothingAction();
                }
            }
            // Phase 2
            else
            {
                switch(count++)
                {
                    case 0:
                        return new TrojanAction(CardResources.Mutex, 2);

                    case 1:
                        count = 0;
                        return new AttackAction(Attack);

                    default:
                        count = 0;
                        return new DoNothingAction();
                };
            }
        }
    }
}
