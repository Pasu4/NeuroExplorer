using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "Sound Effect Resources")]
    public class SoundEffectsResources : ScriptableObject
    {
        public AudioClip hit;
        public AudioClip heal;
        public AudioClip pickup;
        public AudioClip ladder;
        public AudioClip metalPipe;
    }
}
