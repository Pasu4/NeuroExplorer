using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class FilianEnemy : Enemy
    {
        private int count = 0;

        public FilianEnemy(Sprite sprite)
        {
            name = "FilAIn";
            this.sprite = sprite;
            maxHp = 300.kB() + GameManager.Instance.difficulty * 100.kB();
            strength = 20.kB() + GameManager.Instance.difficulty * 10.kB();
            attackFactor = 1.0f;
            defendFactor = 1.0f;
        }

        public override EnemyAction ChooseNextAction(BattleContext ctx)
        {
            if(ctx.battleUI.enemies.Count == 1)
            {
                return new SummonAction(EnemyResources.Drone);
            }

            switch(count++)
            {
                case 0:
                    return new TrojanAction(CardResources.ForkBomb, 2);

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
            };
        }
    }
}
