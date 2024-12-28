using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class MallocEffect : CardEffect
    {
        readonly int count = 0;

        public override string Description => $"{nameStyleOpen}malloc({count}):{nameStyleClose} Draw {count} cards.";

        public MallocEffect(int count)
        {
            this.count = count;
        }

        public override void OnPlay(BattleContext ctx)
        {
            base.OnPlay(ctx);

            ctx.BattleUI.Draw(count);
        }
    }
}
