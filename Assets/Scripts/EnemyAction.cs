using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public abstract class EnemyAction
    {
        public abstract string Description { get; }
    }

    public class AttackAction : EnemyAction
    {
        public long damage;
        public int times;

        public override string Description => $"The enemy intends to deal {Utils.FileSizeString(damage)} of damage to you.";

        public AttackAction(long damage, int times = 1)
        {
            this.damage = damage;
            this.times = times;
        }
    }

    public class DefendAction : EnemyAction
    {
        public long block;

        public override string Description => $"The enemy intends to block {Utils.FileSizeString(block)} of damage next turn.";

        public DefendAction(long block)
        {
            this.block = block;
        }
    }

    public class TrojanAction : EnemyAction
    {
        public override string Description => "The enemy intends to add one or more negative cards to your deck.";
    }

    public class DoNothingAction : EnemyAction
    {
        public override string Description => "The enemy intends to do nothing this turn.";
    }

    public class SummonAction : EnemyAction
    {
        public override string Description => "The enemy intends to summon an ally.";
    }

    public class BuffAction : EnemyAction
    {
        public override string Description => "The enemy intends to make itself stronger.";
    }
}
