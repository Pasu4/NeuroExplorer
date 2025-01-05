using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.CardEffects
{
    public class PlaySoundEffect : CardEffect
    {
        public AudioClip audioClip;

        public override string Description => "";

        public PlaySoundEffect(AudioClip audioClip)
        {
            this.audioClip = audioClip;
        }

        public override void OnPlay(BattleContext ctx)
        {
            GameManager.Instance.sfxSource.PlayOneShot(audioClip);
        }
    }
}
