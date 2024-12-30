using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class ClearerrEffect : CardEffect
    {
        public override string Description => $"{nameStyleOpen}clearerr:{nameStyleClose} Erase all trojan cards in the deck.";

        public override void OnPlay(BattleContext ctx)
        {
            var cs = ctx.battleUI.deck.Where(c => c.card.type == Card.CardType.Trojan);
            foreach(var c in cs)
            {
                ctx.battleUI.Erase(c);
            }
        }
    }
}
