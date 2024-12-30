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
                cardEffects = new[] { new ForkEffect() }
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
                fileSize = GameManager.Instance.maxMp * 1 / 5,
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
