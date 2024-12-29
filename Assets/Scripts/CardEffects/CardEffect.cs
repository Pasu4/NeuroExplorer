using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public abstract class CardEffect
    {
        public static readonly string nameStyleOpen = "<color=#d00><b>";
        public static readonly string nameStyleClose = "</b></color>";
        public abstract string Description { get; }

        public virtual void OnDeckChanged(BattleContext ctx) { }
        public virtual void OnTurnStart(BattleContext ctx) { }
        public virtual void OnTurnEnd(BattleContext ctx) { }
        public virtual void OnEnterHand(BattleContext ctx) { }
        public virtual void OnDiscard(BattleContext ctx) { }
        public virtual void OnErase(BattleContext ctx) { }
        public virtual void OnPlay(BattleContext ctx) { }
    }
}
