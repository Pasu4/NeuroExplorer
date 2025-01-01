using System.Globalization;
using UnityEngine;

namespace Assets.Scripts.CardEffects
{
    public class RandEffect : CardEffect
    {
        long fileSize;

        public override string Description => $"{nameStyleOpen}rand:{nameStyleClose} Add a random {Utils.FileSizeString(fileSize)} card to the hand.";

        public RandEffect(long fileSize)
        {
            this.fileSize=fileSize;
        }

        public override void OnPlay(BattleContext ctx)
        {
            Card card = new Card(
                Random.value.ToString("F7", CultureInfo.InvariantCulture)[2..] + ".log",
                fileSize,
                GameManager.Instance.GetFileSprite(".log")
            );
            ctx.battleUI.CreateHandCard(card);
        }
    }
}
