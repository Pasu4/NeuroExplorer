using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class FilianEnemy : Enemy
    {
        public FilianEnemy(Sprite sprite)
        {
            name = "FilAIn";
            this.sprite = sprite;
            maxHp = 150_000 + GameManager.Instance.difficulty * 100_000;
            hp    = maxHp;
            strength = 20_000 + GameManager.Instance.difficulty * 10_000;
            attackFactor = 1.0f;
            defendFactor = 1.0f;
            trojanCard = CardResources.GetCard("fork_bomb");
            trojanCount = 2;
            nextAction = new TrojanAction();
            multiChance = 0.0f;
        }

        public override void Init()
        {
            // Already set in ctor
        }

        public override EnemyAction ChooseNextAction(BattleContext ctx)
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
    }
}
