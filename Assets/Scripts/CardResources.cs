using Assets.Scripts.CardEffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class CardResources
    {
        private static readonly Dictionary<string, Card> cards = new()
        {
            ["null_pointer"] = new()
            {
                name = "Null Pointer",
                erase = true,
                fileSize = 5_000L,
                sprite = GameManager.Instance.GetFileSprite("null_pointer")
            },
            ["segmentation_fault"] = new()
            {
                name = "Segmentation Fault",
                erase = true,
                @volatile = true,
                fileSize = 5_000L,
                sprite = GameManager.Instance.GetFileSprite("segmentation_fault"),
                cardEffects = new[] { new SegfaultEffect() }
            }
        };
        public static Card GetCard(string id) => cards[id];
    }
}
