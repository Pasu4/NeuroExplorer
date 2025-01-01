using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class ContinueEffect : CardEffect
    {
        public override string Description => $"{nameStyleOpen}continue:{nameStyleClose} The targeted enemy cannot attack this turn.";

        public override void OnPlay(BattleContext ctx)
        {
            ctx.battleUI.targetEnemy.enemy.nextAction = new DoNothingAction();
        }
    }
}
