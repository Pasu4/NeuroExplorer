using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class SemaphoreEffect : CardEffect
    {
        public override string Description => "If 4 Semaphores are in your deck, you lose the game.";
        // Handled by BattleUI
    }
}
