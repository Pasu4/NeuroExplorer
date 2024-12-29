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

        public string FilePath { get; private set; }
        public string Name => Path.GetFileName(FilePath);
        public long FileSize { get; private set; } = 0;
        public Sprite Sprite { get; private set; }
        public CardType Type { get; private set; }
        public long Attack { get; private set; } = 0;
        public long Defense { get; private set; } = 0;
        public CardEffect[] CardEffects { get; private set; } = Array.Empty<CardEffect>();
        public bool RequiresTarget { get; private set; } = false;
        public bool Erase { get; private set; } = false;
        public bool Volatile { get; private set; } = false;

        public Card(string path, long fileSize, Sprite sprite)
        {
            random = GameManager.Instance.CreatePathRandom(path, "CardStats");
            FilePath = path;
            FileSize = fileSize;
            Sprite = sprite;

            long mainValue = (long) Mathf.Sqrt(FileSize) * 1000L; // Main attack or defense value
            int points = Mathf.CeilToInt(Mathf.Log10(FileSize) - 3); // Points to spend on increasing the main value or effects
            
            // (15%) Add [Erase] and 2 points
            if(random.NextDouble() < 0.15f)
            {
                Erase = true;
                points += 2;
            }
            
            // (10%) Add [Volatile] and 2 points
            if(random.NextDouble() < 0.10f)
            {
                Volatile = true;
                points += 2;
            }

            CardType type = random.NextDouble() switch
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
                    InitResource(points);
                    break;
            }

            Type = type;
        }

        private void InitAttack(long mainValue, int points)
        {
            RequiresTarget = true;
            mainValue += (long) (mainValue * random.Range(-0.10f, 0.10f)); // 10% Variance
            Attack = mainValue;

            List<CardEffect> effectList = new();

            int failsafe = 100;
            while(points > 0 && failsafe > 0)
            {
                if(random.NextDouble() < 0.5f) // (50%) Increase damage by 10% for 1 point
                {
                    Attack += Attack / 10;
                    points--;
                }
                else // (50%) Add a random effect
                {
                    Utils.ChooseWeighted(random,
                        (100, new Action(() => {
                            if(effectList.Any(e => e is MallocEffect)) return;

                            int count = random.Next(points) + 1;
                            effectList.Add(new MallocEffect(count));
                            points -= count;
                        }))
                    )();
                }
                failsafe--;
            }
            if(failsafe <= 0) Debug.LogWarning("Failsafe hit");

            CardEffects = effectList.ToArray();
        }

        private void InitDefense(long mainValue, int points)
        {
            mainValue += (long) (mainValue * random.Range(-0.10f, 0.10f)); // 10% Variance
            Defense = mainValue;

            List<CardEffect> effectList = new();

            int failsafe = 100;
            while(points > 0 & failsafe > 0)
            {
                if(random.NextDouble() < 0.5f) // (50%) Increase defense by 10% for 1 point
                {
                    Defense += Defense / 10;
                    points--;
                }
                else // (50%) Add a random effect
                {
                    Utils.ChooseWeighted(random,
                        (100, new Action(() => {
                            if(effectList.Any(e => e is MallocEffect)) return;

                            int count = random.Next(points) + 1;
                            effectList.Add(new MallocEffect(count));
                            points -= count;
                        }))
                    )();
                }
                failsafe--;
            }
            if(failsafe <= 0) Debug.LogWarning("Failsafe hit");

            CardEffects = effectList.ToArray();
        }

        private void InitTool(int points)
        {
            points = Math.Max(points, 1); // At least one point

            List<CardEffect> effectList = new();
            int failsafe = 100;

            while(points > 0 && failsafe > 0)
            {
                Utils.ChooseWeighted(random,
                    (100, new Action(() => {
                        if(effectList.Any(e => e is MallocEffect)) return;

                        int count = random.Next(points) + 1;
                        effectList.Add(new MallocEffect(count));
                        points -= count;
                    }))
                )();
                failsafe--;
            }

            if(failsafe <= 0) Debug.LogWarning("Failsafe hit");

            CardEffects = effectList.ToArray();
        }

        private void InitResource(int points)
        {
            points += 5;

            if(Erase)
            {
                points -= 2;
                Erase = false;
            }
        }

        public void OnErase(BattleContext ctx)
        {
            foreach(CardEffect effect in CardEffects)
                effect.OnErase(ctx);
        }

        public void OnEnterHand(BattleContext ctx)
        {
            foreach(CardEffect effect in CardEffects)
                effect.OnEnterHand(ctx);
        }

        public string GetDescription()
        {
            StringBuilder sb = new();

            if(Attack > 0)
                sb.AppendLine($"Deals <color=#66d>{Utils.FileSizeString(Attack)}</color> of damage.");
            if(Defense > 0)
                sb.AppendLine($"Blocks <color=#66d>{Utils.FileSizeString(Defense)}</color> of damage.");

            if(Erase)
                sb.AppendLine("<color=#d00>[Erase]</color>");
            if(Volatile)
                sb.AppendLine("<color=#d00>[Volatile]</color>");

            foreach(CardEffect effect in CardEffects)
            {
                string desc = effect.Description;
                if(!string.IsNullOrEmpty(desc))
                    sb.AppendLine(desc);
            }

            return sb.ToString();
        }
    }
}
