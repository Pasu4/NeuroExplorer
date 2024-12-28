﻿using Assets.Scripts.CardEffects;
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
        private System.Random random;

        public long FileSize { get; private set; } = 0;
        public long Attack { get; private set; } = 0;
        public long Defense { get; private set; } = 0;
        public CardEffect[] CardEffects { get; private set; } = Array.Empty<CardEffect>();
        public bool RequiresTarget { get; private set; } = false;
        public bool Erase { get; private set; } = false;
        public bool Volatile { get; private set; } = false;

        public Card(string path, long fileSize)
        {
            random = GameManager.Instance.CreatePathRandom(path, "CardStats");
            FileSize = fileSize;

            long mainValue = (long) Mathf.Sqrt(FileSize); // Main attack or defense value
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

            switch(random.NextDouble())
            {
                case <.30: // (30%) Attack card
                    InitAttack(mainValue, points);
                    break;

                case <.60: // (30%) Defense card
                    InitDefense(mainValue, points);
                    break;

                case <.90: // (30%) Tool card
                    InitTool(points);
                    break;

                default: // (10%) Resource card
                    InitResource(points);
                    break;
            }
        }

        private void InitAttack(long mainValue, int points)
        {
            RequiresTarget = true;
            mainValue += (long) (mainValue * random.Range(-0.10f, 0.10f)); // 10% Variance
            Attack = mainValue;

            List<CardEffect> effectList = new();

            while(points > 0)
            {
                if(random.NextDouble() < 0.5f) // (50%) Increase damage by 10% for 1 point
                {
                    Attack += Attack / 10;
                }
                else // (50%) Add a random effect
                {
                    Utils.ChooseWeighted(random,
                        (100, new Action(() => {
                            if(effectList.Any(e => e is MallocEffect)) return;

                            int count = random.Next(3) + 1;
                            effectList.Add(new MallocEffect(count));
                            points -= count;
                        }))
                    )();
                }
            }

            CardEffects = effectList.ToArray();
        }

        private void InitDefense(long mainValue, int points)
        {
            mainValue += (long) (mainValue * random.Range(-0.10f, 0.10f)); // 10% Variance
            Defense = mainValue;

            List<CardEffect> effectList = new();

            while(points > 0)
            {
                if(random.NextDouble() < 0.5f) // (50%) Increase defense by 10% for 1 point
                {
                    Defense += Defense / 10;
                }
                else // (50%) Add a random effect
                {
                    Utils.ChooseWeighted(random,
                        (100, new Action(() => {
                            if(effectList.Any(e => e is MallocEffect)) return;

                            int count = random.Next(3) + 1;
                            effectList.Add(new MallocEffect(count));
                            points -= count;
                        }))
                    )();
                }
            }

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

                        int count = random.Next(3) + 1;
                        effectList.Add(new MallocEffect(count));
                        points -= count;
                    }))
                )();
                failsafe--;
            }

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
                sb.AppendLine($"Deals <color=#fd0>{Utils.FileSizeString(Attack)}</color> of damage.");
            if(Defense > 0)
                sb.AppendLine($"Allocates {Utils.FileSizeString(Defense)} of swap.");

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
