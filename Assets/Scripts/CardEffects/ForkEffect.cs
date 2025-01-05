using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardEffects
{
    public class ForkEffect : CardEffect
    {
        public string createdCardId;
        public override string Description => "If not played, adds another copy to the discard pile";

        public ForkEffect(string createdCardId)
        {
            this.createdCardId = createdCardId;
        }

        public override void OnTurnEnd(BattleContext ctx)
        {
            ctx.battleUI.CreateDiscardedCard(CardResources.GetCard(createdCardId), (Vector2) ctx.battleUI.discardPile.transform.position + Vector2.up * 10);
        }
    }
}
