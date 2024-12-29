using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class SegfaultEffect : CardEffect
    {
        public override string Description => "If not played, take 3 kB of damage.";

        public override void OnTurnEnd(BattleContext ctx)
        {
            ctx.BattleUI.AttackPlayer(3000);
        }
    }
}
