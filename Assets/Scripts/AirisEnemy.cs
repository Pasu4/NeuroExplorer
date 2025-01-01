using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class AirisEnemy : Enemy
    {
        public AirisEnemy(Sprite sprite)
        {
            this.sprite = sprite;
            maxHp = 15_000_000 + GameManager.Instance.difficulty * 10_000_000;
            hp    = maxHp;
            strength = 2_000_000 + GameManager.Instance.difficulty * 1_000_000;
            attackFactor = 1.0f;
            defendFactor = 1.0f;
            trojanCard = CardResources.GetCard("semaphore");
            trojanCount = 1;
            nextAction = new TrojanAction();
            multiChance = 0.0f;
            targetDrawPile = true;
        }

        public override void Init()
        {
            // Already set in ctor
        }

        public override EnemyAction ChooseNextAction(BattleContext ctx)
        {
            return ((ctx.battleUI.turn + 1) % 3) switch
            {
                0 => new TrojanAction(),
                1 => new AttackAction((long) (strength * Random.Range(0.8f, 1.2f) * attackFactor)),
                2 => new DefendAction((long) (strength * Random.Range(0.8f, 1.2f) * defendFactor)),
                _ => throw new System.Exception("Unreachable")
            };
        }
    }
}
