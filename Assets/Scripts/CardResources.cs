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
            // Starting cards
            "osu" => new() {
                name = "osu.url",
                type = Card.CardType.Attack,
                fileSize = 917,
                attack = 30282,
                requiresTarget = true,
                sprite = GameManager.Instance.GetFileSprite(".url")
            },
            "minecraft" => new() {
                name = "Minecraft Launcher.lnk",
                type = Card.CardType.Attack,
                fileSize = 984,
                attack = 31369,
                requiresTarget = true,
                sprite = GameManager.Instance.GetFileSprite(".lnk")
            },
            "slay_the_spire" => new() {
                name = "Slay the Spire.url",
                type = Card.CardType.Attack,
                fileSize = 925,
                attack = 30414,
                requiresTarget = true,
                sprite = GameManager.Instance.GetFileSprite(".url")
            },
            "desktop_ini" => new()
            {
                name = "desktop.ini",
                type = Card.CardType.Defense,
                fileSize = 963,
                defense = 31032,
                sprite = GameManager.Instance.GetFileSprite(".ini")
            },
            "prompt" => new()
            {
                name = "prompt.txt",
                type = Card.CardType.Defense,
                fileSize = 958,
                defense = 30952,
                sprite = GameManager.Instance.GetFileSprite(".txt")
            },
            "neuro_log" => new()
            {
                name = "neuro.log",
                type = Card.CardType.Defense,
                fileSize = 975,
                defense = 31225,
                sprite = GameManager.Instance.GetFileSprite(".log")
            },

            // Special starting cards
            "gymbag" => new() {
                name = "Gymbag.lnk",
                type = Card.CardType.Tool,
                fileSize = 1492,
                erase = true,
                sprite = GameManager.Instance.GetFileSprite(".lnk"),
                cardEffects = new[] { new RandEffect(1500) }
            },
            "metal_pipe" => new()
            {
                name = "metal_pipe.ogg",
                type = Card.CardType.Attack,
                fileSize = 1928,
                attack = 21954,
                multi = true,
                sprite = GameManager.Instance.GetFileSprite(".ogg"),
                cardEffects = new[] { new PlaySoundEffect(GameManager.Instance.sfx.metalPipe) }
            },
            "hiyori" => new()
            {
                name = "353595895589",
                type = Card.CardType.Tool,
                fileSize = 2911,
                erase = true,
                requiresTarget = true,
                sprite = GameManager.Instance.GetFileSprite("error"),
                cardEffects = new[] { new ContinueEffect() }
            },

            // Boss rewards
            "filian_attack" => new() {
                name = "main.py",
                filePath = @"C:\Users\Vedal\source\repos\FilAIn\main.py",
                type = Card.CardType.Attack,
                fileSize = 75337,
                attack = 274_475 * 3/4,
                erase = true,
                sprite = GameManager.Instance.GetFileSprite(".py"),
                cardEffects = new[] { new ForkEffect("filian_attack") }
            },
            "filian_defense" => new() {
                name = "FilAIn.log",
                filePath = @"C:\Users\Vedal\source\repos\FilAIn\FilAIn.log",
                type = Card.CardType.Defense,
                fileSize = 75162,
                defense = 274_156 * 3/4,
                erase = true,
                sprite = GameManager.Instance.GetFileSprite(".log"),
                cardEffects = new[] { new ForkEffect("filian_defense") }
            },
            "camila_attack" => new() {
                name = "AImila.model",
                filePath = @"C:\Program Files\VedalAI\AImila\AImila.model",
                type = Card.CardType.Attack,
                fileSize = 9_846_868,
                attack = 3_137_971 * 12/10,
                sprite = GameManager.Instance.GetFileSprite(".model"),
                cardEffects = new[] { new MutexEffect() }
            },
            "camila_defense" => new() {
                name = "camila_voice.zip",
                filePath = @"C:\Program Files\VedalAI\AImila\camila_voice.zip",
                type = Card.CardType.Defense,
                fileSize = 9_137_697,
                defense = 3_022_862 * 12/10,
                sprite = GameManager.Instance.GetFileSprite(".zip"),
                cardEffects = new[] { new MutexEffect() }
            },

            // Trojans
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
            "fork_bomb" => new()
            {
                name = "Fork Bomb",
                type = Card.CardType.Trojan,
                erase = true,
                fileSize = GameManager.Instance.maxMp * 3 / 5,
                sprite = GameManager.Instance.GetFileSprite("fork_bomb"),
                cardEffects = new[] { new ForkEffect("fork_bomb") }
            },
            "memory_leak" => new()
            {
                name = "Memory Leak",
                type = Card.CardType.Trojan,
                erase = true,
                fileSize = GameManager.Instance.maxMp * 1 / 5,
                sprite = GameManager.Instance.GetFileSprite("memory_leak"),
                cardEffects = new[] { new MemoryLeakEffect(GameManager.Instance.maxHp / 20) }
            },
            "mutex" => new()
            {
                name = "Mutex",
                type = Card.CardType.Trojan,
                erase = true,
                fileSize = GameManager.Instance.maxMp * 9/20,
                sprite = GameManager.Instance.GetFileSprite("mutex"),
                cardEffects = new[] { new MutexEffect() }
            },
            "semaphore" => new()
            {
                name = "Semaphore",
                type = Card.CardType.Trojan,
                erase = true,
                fileSize = GameManager.Instance.maxMp * 3 / 5,
                sprite = GameManager.Instance.GetFileSprite("semaphore"),
                cardEffects = new[] { new SemaphoreEffect() }
            },
            _ => new()
            {
                name = "Error",
                type = Card.CardType.Trojan,
                @volatile = true,
                fileSize = 1_000_000_000_000L,
                sprite = GameManager.Instance.GetFileSprite("error")
            }
        };
    }
}
