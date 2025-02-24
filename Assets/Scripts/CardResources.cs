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
        // Starting cards
        public static Card Osu => new()
        {
            name = "osu.url",
            type = Card.CardType.Attack,
            fileSize = 458,
            attack = 21401 * 3/4,
            requiresTarget = true,
            sprite = GameManager.Instance.GetFileSprite(".url")
        };
        public static Card Minecraft => new()
        {
            name = "Minecraft Launcher.lnk",
            type = Card.CardType.Attack,
            fileSize = 492,
            attack = 22181 * 3/4,
            requiresTarget = true,
            sprite = GameManager.Instance.GetFileSprite(".lnk")
        };
        public static Card SlayTheSpire => new()
        {
            name = "Slay the Spire.url",
            type = Card.CardType.Attack,
            fileSize = 462,
            attack = 21494 * 3/4,
            requiresTarget = true,
            sprite = GameManager.Instance.GetFileSprite(".url")
        };
        public static Card DesktopIni => new()
        {
            name = "desktop.ini",
            type = Card.CardType.Defense,
            fileSize = 481,
            defense = 21932 * 3/4,
            sprite = GameManager.Instance.GetFileSprite(".ini")
        };
        public static Card Prompt => new()
        {
            name = "prompt.txt",
            type = Card.CardType.Defense,
            fileSize = 479,
            defense = 21886 * 3/4,
            sprite = GameManager.Instance.GetFileSprite(".txt")
        };
        public static Card NeuroLog => new()
        {
            name = "neuro.log",
            type = Card.CardType.Defense,
            fileSize = 487,
            defense = 22068,
            sprite = GameManager.Instance.GetFileSprite(".log")
        };

        // Special starting cards
        public static Card Gymbag => new()
        {
            name = "Gymbag.lnk",
            type = Card.CardType.Tool,
            fileSize = 1492,
            erase = true,
            sprite = GameManager.Instance.GetFileSprite(".lnk"),
            cardEffects = new[] { new RandEffect(1500) }
        };
        public static Card MetalPipe => new()
        {
            name = "metal_pipe.ogg",
            type = Card.CardType.Attack,
            fileSize = 1928,
            attack = 21954,
            multi = true,
            sprite = GameManager.Instance.GetFileSprite(".ogg"),
            cardEffects = new[] { new PlaySoundEffect(GameManager.Instance.sfx.metalPipe) }
        };
        public static Card Arg => new()
        {
            name = "353595895589",
            type = Card.CardType.Tool,
            fileSize = 2911,
            erase = true,
            requiresTarget = true,
            sprite = GameManager.Instance.GetFileSprite("error"),
            cardEffects = new[] { new ContinueEffect() }
        };

        // Boss rewards
        public static Card FilianAttack => new()
        {
            name = "main.py",
            filePath = @"C:\Users\Vedal\source\repos\FilAIn\main.py",
            type = Card.CardType.Attack,
            fileSize = 75337,
            attack = 274_475,
            requiresTarget = true,
            erase = true,
            sprite = GameManager.Instance.GetFileSprite(".py"),
            cardEffects = new[] { new ForkEffect(() => FilianAttack) }
        };
        public static Card FilianDefense => new()
        {
            name = "FilAIn.log",
            filePath = @"C:\Users\Vedal\source\repos\FilAIn\FilAIn.log",
            type = Card.CardType.Defense,
            fileSize = 75162,
            defense = 274_156 / 2,
            async = true,
            sprite = GameManager.Instance.GetFileSprite(".log")
        };
        public static Card CamilaAttack => new()
        {
            name = "AImila.model",
            filePath = @"C:\Program Files\VedalAI\AImila\AImila.model",
            type = Card.CardType.Attack,
            fileSize = 9_846_868,
            attack = 3_137_971 * 12/10,
            requiresTarget = true,
            sprite = GameManager.Instance.GetFileSprite(".model"),
            cardEffects = new[] { new MutexEffect() }
        };
        public static Card CamilaDefense => new()
        {
            name = "camila_voice.zip",
            filePath = @"C:\Program Files\VedalAI\AImila\camila_voice.zip",
            type = Card.CardType.Tool, // Not really defense but eh
            fileSize = 9_137_697,
            erase = true,
            requiresTarget = true,
            sprite = GameManager.Instance.GetFileSprite(".zip"),
            cardEffects = new[] { new ContinueEffect() }
        };

        // Trojans
        public static Card SegmentationFault => new()
        {
            name = "Segmentation Fault",
            type = Card.CardType.Trojan,
            erase = true,
            @volatile = true,
            fileSize = GameManager.Instance.maxMp * 2 / 5,
            sprite = GameManager.Instance.GetFileSprite("segmentation_fault"),
            cardEffects = new[] { new SegfaultEffect() }
        };
        public static Card NullPointer => new()
        {
            name = "Null Pointer",
            type = Card.CardType.Trojan,
            erase = true,
            fileSize = GameManager.Instance.maxMp * 1 / 5,
            sprite = GameManager.Instance.GetFileSprite("null_pointer")
        };
        public static Card ForkBomb => new()
        {
            name = "Fork Bomb",
            type = Card.CardType.Trojan,
            erase = true,
            fileSize = GameManager.Instance.maxMp * 3 / 5,
            sprite = GameManager.Instance.GetFileSprite("fork_bomb"),
            cardEffects = new CardEffect[] { new ForkEffect(() => ForkBomb), new NeuroDescriptionEffect("You should get rid of this card before it floods your deck.") }
        };
        public static Card MemoryLeak => new()
        {
            name = "Memory Leak",
            type = Card.CardType.Trojan,
            erase = true,
            fileSize = GameManager.Instance.maxMp * 1 / 5,
            sprite = GameManager.Instance.GetFileSprite("memory_leak"),
            cardEffects = new[] { new MemoryLeakEffect(GameManager.Instance.maxHp / 20) }
        };
        public static Card Mutex => new()
        {
            name = "Mutex",
            type = Card.CardType.Trojan,
            erase = true,
            fileSize = GameManager.Instance.maxMp * 9/20,
            sprite = GameManager.Instance.GetFileSprite("mutex"),
            cardEffects = new[] { new MutexEffect() }
        };
        public static Card Semaphore => new()
        {
            name = "Semaphore",
            type = Card.CardType.Trojan,
            erase = true,
            fileSize = GameManager.Instance.maxMp * 3 / 5,
            sprite = GameManager.Instance.GetFileSprite("semaphore"),
            cardEffects = new[] { new SemaphoreEffect() }
        };
        public static Card Error => new()
        {
            name = "Error",
            type = Card.CardType.Trojan,
            @volatile = true,
            fileSize = 1_000_000_000_000L,
            sprite = GameManager.Instance.GetFileSprite("error")
        };
    }
}
