using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class ForkEffect : CardEffect
    {
        public override string Description => "If not played, adds another copy to the discard pile";

        public override void OnTurnEnd(BattleContext ctx)
        {
            ctx.battleUI.CreateDiscardedCard(CardResources.GetCard("fork_bomb"));
        }
    }
}
