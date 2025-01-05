using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Heal : MonoBehaviour
    {
        private void OnMouseDown()
        {
            if(GameManager.Instance.gameMode != GameMode.Room) return;

            GameManager.Instance.sfxSource.PlayOneShot(GameManager.Instance.sfx.heal);
            GameManager.Instance.CreateTextEffect("Fully healed", Color.green, GameManager.Instance.player.transform.position);
            GameManager.Instance.hp = GameManager.Instance.maxHp;
        }
    }
}
