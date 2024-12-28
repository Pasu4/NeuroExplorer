using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal class Card
    {
        private Random random;

        public float FileSize { get; private set; }
        public float Attack { get; private set; } = 0;
        public float Defense { get; private set; } = 0;
        public CardEffect[] CardEffects { get; private set; }

        public Card(string path)
        {
            random = GameManager.Instance.CreatePathRandom(path, "CardStats");

            
        }
    }
}
