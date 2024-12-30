using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class MutexEffect : CardEffect
    {
        public override string Description => "Cannot play another card while this card is in hand";
        // Handled by BattleUI
    }
}
