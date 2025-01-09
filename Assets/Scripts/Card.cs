using Assets.Scripts.CardEffects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Card
    {
        public enum CardType
        {
            Attack, Defense, Tool, Resource, Trojan
        }

        private System.Random random;

        public string filePath;
        public string name;
        public long fileSize = 0;
        public Sprite sprite;
        public CardType type;
        public long attack = 0;
        public long defense = 0;
        public CardEffect[] cardEffects = Array.Empty<CardEffect>();
        public bool requiresTarget = false;
        public bool erase = false;
        public bool @volatile = false;
        public bool keep = false;
        public bool multi = false;
        public bool async = false;

        // For object initializer
        public Card()
        {
            random = new System.Random();
        }

        public Card(string path, long fileSize, Sprite sprite)
        {
            random = GameManager.Instance.CreatePathRandom(path, "CardStats");
            filePath = path;
            this.fileSize = fileSize;
            this.sprite = sprite;
            name = Path.GetFileName(filePath);

            long mainValue = (long) Mathf.Sqrt(this.fileSize) * 1000L; // Main attack or defense value
            int points = Mathf.CeilToInt(Mathf.Log10(this.fileSize) - 4); // Points to spend on increasing the main value or effects
            
            // (15%) Add [Erase] and 2 points
            if(random.NextDouble() < 0.15f)
            {
                erase = true;
                points += 2;
            }
            
            // (10%) Add [Volatile] and 2 points
            if(random.NextDouble() < 0.10f)
            {
                @volatile = true;
                points += 2;
            }

            type = random.NextDouble() switch
            {
                <.35 => CardType.Attack,  // (35%) Attack card
                <.70 => CardType.Defense, // (35%) Defense card
                <.95 => CardType.Tool,    // (25%) Tool card
                _ => CardType.Resource    // ( 5%) Resource card
            };

            // Based on file extension
            switch(Path.GetExtension(path))
            {
                case ".exe": // Weapon (Always attack, +25% attack)
                    type = CardType.Attack;
                    mainValue += mainValue / 4;
                    break;

                case ".zip": // Shield (Always defense, +25% defense)
                case ".tar":
                case ".gz":
                case ".7z":
                    type = CardType.Defense;
                    mainValue += mainValue / 4;
                    break;

                case ".dll": // Spell (Always tool, +2 pts)
                    type = CardType.Tool;
                    points += 2;
                    break;

                default: break;
            }
            
            // Init
            switch(type)
            {
                case CardType.Attack:
                    InitAttack(mainValue, points);
                    break;

                case CardType.Defense:
                    InitDefense(mainValue, points);
                    break;

                case CardType.Tool:
                    InitTool(points);
                    break;

                case CardType.Resource:
                    //InitResource(points);
                    InitTool(points); // TODO
                    break;
            }
        }

        private void InitAttack(long mainValue, int points)
        {
            requiresTarget = true;
            mainValue += (long) (mainValue * random.Range(-0.10f, 0.10f)); // 10% Variance
            attack = mainValue;

            List<CardEffect> effectList = new();

            // Apply effects to 50% of cards
            if(random.Next() % 2 == 0)
            {
                int rolls = points;
                while(points > 0 && rolls > 0)
                {
                    points = ChooseEffect(points, ref attack, effectList);
                    rolls--;
                }
            }

            cardEffects = effectList.ToArray();
        }

        private void InitDefense(long mainValue, int points)
        {
            mainValue += (long) (mainValue * random.Range(-0.10f, 0.10f)); // 10% Variance
            defense = mainValue;

            List<CardEffect> effectList = new();

            // Apply effects to 50% of cards
            if(random.Next() % 2 == 0)
            {
                int rolls = points;
                while(points > 0 & rolls > 0)
                {
                    points = ChooseEffect(points, ref defense, effectList);
                    rolls--;
                }
            }

            cardEffects = effectList.ToArray();
        }

        private void InitTool(int points)
        {
            points = Math.Max(points, 1); // At least one point

            List<CardEffect> effectList = new();
            int rolls = points;

            while(points > 0 && rolls > 0)
            {
                long __ = 0;
                points = ChooseEffect(points, ref __, effectList);
                rolls--;
            }

            cardEffects = effectList.ToArray();
        }

        private void InitResource(int points)
        {
            points += 5;

            if(erase)
            {
                points -= 2;
                erase = false;
            }
        }

        private int ChooseEffect(int points, ref long mainValue, List<CardEffect> effectList)
        {
            long localMainValue = mainValue;

            Utils.ChooseWeighted(random,
                (100, new Action(() =>
                {
                    if(type != CardType.Attack || multi) return;

                    multi = true;
                    if(!effectList.Any(e => e is ContinueEffect))
                        requiresTarget = false;
                    
                    points -= 2;
                    localMainValue /= 2; // 50%
                })),
                (100, new Action(() =>
                {
                    if(effectList.Any(e => e is MallocEffect)) return;

                    int count = type == CardType.Tool ? random.Next(points) + 1 : 1;
                    effectList.Add(new MallocEffect(count));
                    
                    points -= count;
                    localMainValue = localMainValue * 8/10; // 80%
                })),
                (100, new Action(() =>
                {
                    if(type != CardType.Tool || effectList.Any(e => e is ClearerrEffect)) return;

                    if(!erase)
                    {
                        points += 2;
                        erase = true;
                    }

                    int count = random.Next(points) + 1;
                    effectList.Add(new ClearerrEffect(count));

                    points -= count;
                })),
                (100, new Action(() =>
                {
                    if(effectList.Any(e => e is MemmoveEffect) || points < 2) return;

                    effectList.Add(new MemmoveEffect());

                    points -= 2;
                    localMainValue = localMainValue * 7/10; // 70%
                })),
                (100, new Action(() =>
                {
                    if(type != CardType.Tool || effectList.Any(e => e is QsortEffect) || points < 4) return;

                    effectList.Add(new QsortEffect());
                    points -= 4;
                })),
                (100, new Action(() =>
                {
                    if(type != CardType.Tool || points < 2) return;

                    effectList.Add(new RandEffect((long) (fileSize * random.Range(0.5f, 1.5f))));

                    points -= 2;
                })),
                (100, new Action(() =>
                {
                    if(type != CardType.Tool || effectList.Any(e => e is ReallocEffect) || points < 2) return;

                    effectList.Add(new ReallocEffect());
                    points -= 2;
                }))
                // Too OP
                /*
                (100, new Action(() =>
                {
                    if((type != CardType.Tool && type != CardType.Attack) || effectList.Any(e => e is ContinueEffect) || points < 2) return;

                    requiresTarget = true;
                    erase = true;
                    effectList.Add(new ContinueEffect());
                    points -= 2;
                }))*/
            )(); // Immediately execute

            mainValue = localMainValue;
            return points;
        }

        public void OnPlay(BattleContext ctx)
        {
            foreach(CardEffect effect in cardEffects)
                effect.OnPlay(ctx);
        }

        public void OnDiscard(BattleContext ctx)
        {
            foreach(CardEffect effect in cardEffects)
                effect.OnDiscard(ctx);
        }

        public void OnErase(BattleContext ctx)
        {
            foreach(CardEffect effect in cardEffects)
                effect.OnErase(ctx);
        }

        public void OnEnterHand(BattleContext ctx)
        {
            foreach(CardEffect effect in cardEffects)
                effect.OnEnterHand(ctx);
        }

        public void OnTurnEnd(BattleContext ctx)
        {
            foreach(CardEffect effect in cardEffects)
                effect.OnTurnEnd(ctx);
        }

        public string GetDescription(bool forNeuro = false)
        {
            StringBuilder sb = new();

            if(attack > 0)
                sb.AppendLine($"Deals <color=#66d>{Utils.FileSizeString(attack)}</color> of damage{(multi ? " to all enemies" : "")}.");
            if(defense > 0)
                sb.AppendLine($"Blocks <color=#66d>{Utils.FileSizeString(defense)}</color> of damage.");

            if(forNeuro)
            {
                if(erase)
                    sb.AppendLine("Erase: If this card is played, it is removed from your deck for the rest of the battle.");
                if(@volatile)
                    sb.AppendLine("Volatile: If this card is in your hand at the end of the turn, it is removed from your deck for the rest of the battle.");
                if(keep)
                    sb.AppendLine("Static: This card stays in your hand at the end of the turn instead of being discarded.");
                if(async)
                    sb.AppendLine("Async: This card is not discarded when you play it.");
            }
            else
            {
                if(erase)
                    sb.AppendLine("<color=#d00>[Erase]</color>");
                if(@volatile)
                    sb.AppendLine("<color=#d00>[Volatile]</color>");
                if(keep)
                    sb.AppendLine("<color=#d00>[Static]</color>");
                if(async)
                    sb.AppendLine("<color=#d00>[Async]</color>");
            }

            foreach(CardEffect effect in cardEffects)
            {
                string desc = effect.Description;
                if(!string.IsNullOrEmpty(desc))
                    sb.AppendLine(desc);
                if(forNeuro && effect is NeuroDescriptionEffect nde)
                    sb.AppendLine(nde.neuroDescription);
            }

            return sb.ToString();
        }
    }
}
