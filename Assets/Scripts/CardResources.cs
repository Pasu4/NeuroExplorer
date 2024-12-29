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
        public static Card GetCard(string id) => id switch
        {
            "segmentation_fault" => new()
            {
                name = "Segmentation Fault",
                type = Card.CardType.Trojan,
                erase = true,
                @volatile = true,
                fileSize = GameManager.Instance.maxMp * 2 / 5,
                sprite = GameManager.Instance.GetFileSprite("segmentation_fault"),
                cardEffects = new[] { new SegfaultEffect() }
            },
            "null_pointer" => new()
            {
                name = "Null Pointer",
                type = Card.CardType.Trojan,
                erase = true,
                fileSize = GameManager.Instance.maxMp * 1 / 5,
                sprite = GameManager.Instance.GetFileSprite("null_pointer")
            },
            _ => new()
            {
                name = "Error",
                type = Card.CardType.Trojan,
                @volatile = true,
                fileSize = 1_000_000_000_000L,
                sprite = GameManager.Instance.GetFileSprite("null_pointer")
            }
        };
    }
}
