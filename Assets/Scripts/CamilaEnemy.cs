using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class CamilaEnemy : Enemy
    {
        bool phase2 = false;

        public CamilaEnemy(Sprite sprite)
        {
            this.sprite = sprite;
            maxHp = 1_500_000 + GameManager.Instance.difficulty * 1_000_000;
            hp    = maxHp;
            strength = 200_000 + GameManager.Instance.difficulty * 100_000;
            attackFactor = 1.0f;
            defendFactor = 1.0f;
            trojanCard = CardResources.GetCard("mutex");
            trojanCount = 1;
            nextAction = new TrojanAction();
            multiChance = 0.0f;
        }

        public override void Init()
        {
            // Already set in ctor
        }

        public override void DoTurn(BattleContext ctx)
        {
            if(hp < maxHp / 2 && !phase2)
            {
                phase2 = true;

                trojanCount = 2;
                attackFactor = 1.5f;

                GameManager.Instance.CreateTextEffect("Phase change", Color.red, (Vector2) ctx.activeEnemy.transform.position + Vector2.up * 0.5f);
            }

            base.DoTurn(ctx);
        }

        public override EnemyAction ChooseNextAction(BattleContext ctx)
        {
            if(!phase2)
            {
                return ((ctx.battleUI.turn + 1) % 5) switch
                {
                    0 => new TrojanAction(),
                    1 => new AttackAction((long) (strength * Random.Range(0.8f, 1.2f) * attackFactor)),
                    2 => new DefendAction((long) (strength * Random.Range(0.8f, 1.2f) * defendFactor)),
                    3 => new AttackAction((long) (strength * Random.Range(0.8f, 1.2f) * attackFactor)),
                    4 => new DefendAction((long) (strength * Random.Range(0.8f, 1.2f) * defendFactor)),
                    _ => throw new System.Exception("Unreachable")
                };
            }
            else
            {
                return ((ctx.battleUI.turn + 1) % 2) switch
                {
                    0 => new TrojanAction(),
                    1 => new AttackAction((long) (strength * Random.Range(0.8f, 1.2f) * attackFactor)),
                    _ => throw new System.Exception("Unreachable")
                };
            }
        }
    }
}
