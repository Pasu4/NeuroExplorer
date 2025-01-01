using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using URandom = UnityEngine.Random;

namespace Assets.Scripts.CardEffects
{
    public class ClearerrEffect : CardEffect
    {
        public int count;
        public override string Description => $"{nameStyleOpen}clearerr({count}):{nameStyleClose} Erase up to {count} trojan cards from the deck.";

        public ClearerrEffect(int count)
        {
            this.count = count;
        }

        public override void OnPlay(BattleContext ctx)
        {
            BattleCardUI[] cs = ctx.battleUI.deck
                .Where(c => c.card.type == Card.CardType.Trojan)
                .OrderBy(_ => URandom.value)
                .Take(count)
                .ToArray();
            foreach(BattleCardUI c in cs)
            {
                ctx.battleUI.Erase(c);
            }
        }
    }
}
