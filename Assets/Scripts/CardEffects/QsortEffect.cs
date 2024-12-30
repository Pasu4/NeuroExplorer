using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class QsortEffect : CardEffect
    {
        public override string Description => $"{nameStyleOpen}qsort:{nameStyleClose} Order draw pile by file size.";

        public override void OnPlay(BattleContext ctx)
        {
            ctx.battleUI.drawCards = ctx.battleUI.drawCards.OrderBy(c => c.card.fileSize).ToList();
            for(int i = 0; i < ctx.battleUI.drawCards.Count; i++)
            {
                ctx.battleUI.drawCards[i].transform.SetAsLastSibling();
            }
        }
    }
}
