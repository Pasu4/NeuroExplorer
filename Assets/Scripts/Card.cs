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

        public Card(string path)
        {
            random = GameManager.Instance.CreatePathRandom(path, "CardStats");
        }
    }
}
