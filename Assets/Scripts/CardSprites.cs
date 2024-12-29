using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    [System.Serializable]
    public class CardSprites
    {
        public Sprite attackFront;
        public Sprite attackBack;
        public Sprite defenseFront;
        public Sprite defenseBack;
        public Sprite toolFront;
        public Sprite toolBack;
        public Sprite resourceFront;
        public Sprite resourceBack;
        public Sprite trojanFront;
        public Sprite trojanBack;

        public Sprite GetFrontSprite(Card.CardType type) => type switch
        {
            Card.CardType.Attack => attackFront,
            Card.CardType.Defense => defenseFront,
            Card.CardType.Tool => toolFront,
            Card.CardType.Resource => resourceFront,
            Card.CardType.Trojan => trojanFront,
            _ => throw new NotImplementedException()
        };

        public Sprite GetBackSprite(Card.CardType type) => type switch
        {
            Card.CardType.Attack => attackBack,
            Card.CardType.Defense => defenseBack,
            Card.CardType.Tool => toolBack,
            Card.CardType.Resource => resourceBack,
            Card.CardType.Trojan => trojanBack,
            _ => throw new NotImplementedException()
        };
    }
}
