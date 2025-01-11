using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(fileName = "EnemyResources", menuName = "Resources/Enemy Sprite Resources")]
    public class EnemySpriteResources : ScriptableObject
    {
        public Sprite drone;
        public Sprite gymbagDrone;
        public Sprite neuroYukkuri;
    }
}
