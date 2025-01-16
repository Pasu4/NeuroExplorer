using Assets.Scripts.Enemies;
using UnityEngine;

namespace Assets.Scripts
{
    public static class EnemyResources
    {
        public static Enemy Drone => new SimpleEnemy
        {
            name = "Drone",
            maxHp = (long) (60000 * GameManager.Instance.enemyHpScale * Random.Range(0.8f, 1.2f)),
            sprite = GameManager.Instance.enemySprites.drone,
            attackFactor = 1f,
            defendFactor = 1f,
            nextActionProvider = (ctx, self) =>
                Utils.ChooseWeightedRandom<EnemyAction>(
                    (150, new AttackAction(self.Attack)),
                    (100, new DefendAction(self.Defense))
                )
        };
        public static Enemy GymbagDrone => new SimpleEnemy
        {
            name = "Gymbag Drone",
            maxHp = (long) (90000 * GameManager.Instance.enemyHpScale * Random.Range(0.8f, 1.2f)),
            sprite = GameManager.Instance.enemySprites.gymbagDrone,
            attackFactor = 1f,
            defendFactor = 1f,
            nextActionProvider = (ctx, self) =>
                Utils.ChooseWeightedRandom<EnemyAction>(
                    (100, new AttackAction(self.Attack)),
                    (100, new DefendAction(self.Defense)),
                    (50, new TrojanAction(CardResources.NullPointer, 4)),
                    (100, new BuffAction(1.3f, 1.0f))
                )
        };
        public static Enemy NeuroYukkuri => new SimpleEnemy
        {
            name = "Neuro Yukkuri",
            maxHp = (long) (55000 * GameManager.Instance.enemyHpScale * Random.Range(0.8f, 1.2f)),
            sprite = GameManager.Instance.enemySprites.neuroYukkuri,
            attackFactor = 0.5f,
            defendFactor = 2.0f,
            nextActionProvider = (ctx, self) =>
                Utils.ChooseWeightedRandom<EnemyAction>(
                    (50, new AttackAction(self.Attack)),
                    (100, new DefendAction(self.Defense)),
                    (150, new TrojanAction(CardResources.MemoryLeak, 2)),
                    (100, new DoNothingAction())
                )
        };
    }
}
