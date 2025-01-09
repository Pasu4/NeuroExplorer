using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.CardEffects
{
    public class NeuroDescriptionEffect : CardEffect
    {
        public override string Description => "";
        public string neuroDescription;

        public NeuroDescriptionEffect(string neuroDescription)
        {
            this.neuroDescription = neuroDescription;
        }
    }
}
