using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public abstract class EnemyAction { }

    public class AttackAction : EnemyAction
    {
        public long damage;
        public int times;

        public AttackAction(long damage, int times = 1)
        {
            this.damage = damage;
            this.times = times;
        }
    }

    public class DefendAction : EnemyAction
    {
        public long block;
        public DefendAction(long block)
        {
            this.block = block;
        }
    }

    public class TrojanAction : EnemyAction { }

    public class DoNothingAction : EnemyAction { }
}
