using NeuroSdk.Messages.Outgoing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;

using URandom = UnityEngine.Random;
namespace Assets.Scripts
{
    public abstract class EnemyAction
    {
        public virtual string Text => string.Empty;
        public abstract string NeuroDescription { get; }
        public abstract Sprite Sprite { get; }

        public abstract void Execute(BattleContext ctx);
    }

    public class AttackAction : EnemyAction
    {
        long damage;
        int times;

        public override string Text => (times > 1 ? times + "x " : "") + Utils.FileSizeString(damage);

        public override string NeuroDescription => $"The enemy intends to deal {Utils.FileSizeString(damage)} of damage to you.";

        public override Sprite Sprite => GameManager.Instance.enemyActionAttackSprite;

        public AttackAction(long damage, int times = 1, float variance = 0.15f)
        {
            this.damage = (long) (damage * URandom.Range(1f - variance, 1f + variance));
            this.times = times;
        }

        public override void Execute(BattleContext ctx)
        {
            GameManager.Instance.CreateTextEffect("Attack", Color.blue, ctx.activeEnemy.transform.position);
            for(int i = 0; i < times; i++)
            {
                ctx.battleUI.AttackPlayer(damage);
                Context.Send($"{ctx.activeEnemy.enemy.name} deals {Utils.FileSizeString(damage)} of damage to you.");
            }
        }
    }

    public class DefendAction : EnemyAction
    {
        long block;

        public override string Text => Utils.FileSizeString(block);

        public override string NeuroDescription => $"The enemy intends to block {Utils.FileSizeString(block)} of damage next turn.";

        public override Sprite Sprite => GameManager.Instance.enemyActionDefendSprite;

        public DefendAction(long block, float variance = 0.15f)
        {
            this.block = (long) (block * URandom.Range(1f - variance, 1f + variance));
        }

        public override void Execute(BattleContext ctx)
        {
            GameManager.Instance.CreateTextEffect("Defend", Color.blue, ctx.activeEnemy.transform.position);
            ctx.activeEnemy.enemy.block = block;
            Context.Send($"{ctx.activeEnemy.enemy.name} prepares to block {Utils.FileSizeString(block)} of damage.");
        }
    }

    public class TrojanAction : EnemyAction
    {
        Card[] cards;
        bool targetDrawPile;

        public override string NeuroDescription => "The enemy intends to add one or more negative cards to your deck.";

        public override Sprite Sprite => GameManager.Instance.enemyActionTrojanSprite;

        public TrojanAction(bool targetDrawPile, params Card[] cards)
        {
            this.targetDrawPile = targetDrawPile;
            this.cards = cards;
        }

        public TrojanAction(params Card[] cards) : this(false, cards) { }
        public TrojanAction(Card card, int count, bool targetDrawPile = false)
        {
            this.targetDrawPile = targetDrawPile;
            cards = new Card[count];
            Array.Fill(cards, card);
        }

        public override void Execute(BattleContext ctx)
        {
            Enemy enemy = ctx.activeEnemy.enemy;
            GameManager.Instance.CreateTextEffect("Trojan", Color.black, ctx.activeEnemy.transform.position);

            foreach(Card card in cards)
            {
                if(targetDrawPile)
                    ctx.battleUI.CreateDrawCard(card, ctx.activeEnemy.transform.position);
                else
                    ctx.battleUI.CreateDiscardedCard(card, ctx.activeEnemy.transform.position);
            }
            string s = cards.Length == 1 ? "" : "s";
            Context.Send($"{enemy.name} creates {cards.Length} trojan card{s} in your {(targetDrawPile ? "draw pile" : "discard pile")}.");
        }
    }

    public class DoNothingAction : EnemyAction
    {
        public override string NeuroDescription => "The enemy intends to do nothing this turn.";

        public override Sprite Sprite => GameManager.Instance.enemyActionDoNothingSprite;

        public override void Execute(BattleContext ctx)
        {
            GameManager.Instance.CreateTextEffect("Pass", Color.gray, ctx.activeEnemy.transform.position);
        }
    }

    public class SummonAction : EnemyAction
    {
        Enemy summonedEnemy;

        public override string NeuroDescription => "The enemy intends to summon an ally.";

        public override Sprite Sprite => GameManager.Instance.enemyActionSummonSprite;

        public SummonAction(Enemy summonedEnemy)
        {
            this.summonedEnemy = summonedEnemy;
        }

        public override void Execute(BattleContext ctx)
        {
            GameManager.Instance.CreateTextEffect("Summon", new Color(1.0f, 0.0f, 0.5f), ctx.activeEnemy.transform.position);

            EnemyUI spawned = ctx.battleUI.CreateEnemy(summonedEnemy, ctx.activeEnemy.transform.position);
            spawned.enemy.nextAction = new DoNothingAction(); // Prevent enemy from immediately acting

            Context.Send($"{ctx.activeEnemy.enemy.name} summons a {summonedEnemy.name}.");
        }
    }

    public class BuffAction : EnemyAction
    {
        float attackBuff = 1.0f;
        float defenseBuff = 1.0f;

        public override string NeuroDescription => "The enemy intends to buff itself.";

        public override Sprite Sprite => GameManager.Instance.enemyActionBuffSprite;

        public BuffAction(float attackBuff, float defenseBuff)
        {
            this.attackBuff = attackBuff;
            this.defenseBuff = defenseBuff;
        }

        public override void Execute(BattleContext ctx)
        {
            GameManager.Instance.CreateTextEffect("Buff", new Color(1.0f, 0.5f, 0.0f), ctx.activeEnemy.transform.position);

            Enemy e = ctx.activeEnemy.enemy;

            e.attackFactor *= attackBuff;
            e.defendFactor *= defenseBuff;

            Context.Send($"{e.name} buffed their stats.");
        }
    }

    public class HealAction : EnemyAction
    {
        long absoluteAmount;
        float relativeAmount;

        public override string NeuroDescription => "The enemy intends to recover its health.";

        public override Sprite Sprite => GameManager.Instance.enemyActionHealSprite;

        public override void Execute(BattleContext ctx)
        {
            Enemy e = ctx.activeEnemy.enemy;
            long amount = (long) (e.maxHp * relativeAmount) + absoluteAmount;
            if(amount > e.maxHp - e.hp) amount = e.maxHp - e.hp;

            GameManager.Instance.CreateTextEffect("+" + Utils.FileSizeString(amount), Color.green, ctx.activeEnemy.transform.position);
            e.hp += amount;
            Context.Send($"{e.name} recovers {Utils.FileSizeString(amount)} of health.");
        }
    }
}
