using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class MemmoveEffect : CardEffect
    {
        public override string Description => $"{nameStyleOpen}memmove:{nameStyleClose} Reshuffle your deck";

        public override void OnPlay(BattleContext ctx)
        {
            ctx.battleUI.Reshuffle();
        }
    }
}
