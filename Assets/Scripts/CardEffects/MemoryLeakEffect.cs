using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class MemoryLeakEffect : CardEffect
    {
        long dmg;
        public override string Description => $"Take {Utils.FileSizeString(dmg)} of damage when drawn.";

        public MemoryLeakEffect(long dmg)
        {
            this.dmg = dmg;
        }

        public override void OnEnterHand(BattleContext ctx)
        {
            ctx.battleUI.AttackPlayer(dmg);
        }
    }
}
