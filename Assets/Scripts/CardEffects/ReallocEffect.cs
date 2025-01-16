using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class ReallocEffect : CardEffect
    {
        public override string Description => $"{nameStyleOpen}realloc:{nameStyleClose} Discard your entire hand and draw the same amount of cards.";

        public override void OnPlay(BattleContext ctx)
        {
            List<BattleCardUI> handCards = ctx.battleUI.handCards.ToList(); // Copy
            foreach(BattleCardUI card in handCards)
            {
                ctx.battleUI.Discard(card);
            }
            ctx.battleUI.Draw(handCards.Count, false); // Count of copied list
        }
    }
}
