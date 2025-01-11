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
        public override string Description => "If not played, adds another copy to the discard pile";
        /// <summary>
        /// Must be a getter to prevent infinite loop.
        /// </summary>
        public Func<Card> cardGetter = () => CardResources.Error;

        public ForkEffect(Func<Card> cardGetter)
        {
            this.cardGetter = cardGetter;
        }

        public override void OnTurnEnd(BattleContext ctx)
        {
            ctx.battleUI.CreateDiscardedCard(cardGetter(), (Vector2) ctx.battleUI.discardPile.transform.position + Vector2.up * 10);
        }
    }
}
