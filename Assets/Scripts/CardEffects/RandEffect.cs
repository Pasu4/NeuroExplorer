using System.Globalization;
using Random = System.Random;
using UnityEngine;
using System.Linq;

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
            Random random = new();

            string ext = random.Choose(GameManager.Instance.fakeFilesByExt.Keys.ToArray());
            string fileName = random.Choose(GameManager.Instance.fakeFilesByExt[ext]);

            Card card = new Card(
                fileName + ext,
                fileSize,
                GameManager.Instance.GetFileSprite(ext)
            );
            ctx.battleUI.CreateHandCard(card);
        }
    }
}
