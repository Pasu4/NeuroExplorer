using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EnemyUI : MonoBehaviour
    {
        public TextMeshProUGUI hpText;
        public BarUI hpBar;
        public Enemy enemy;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            hpBar.maxValue = enemy.maxHp;
            hpBar.value = enemy.hp;
        }

        public void SetEnemy(Enemy enemy)
        {
            this.enemy = enemy;

            GetComponent<Image>().sprite = enemy.sprite;
            hpBar.maxValue = enemy.maxHp;
            hpBar.value = enemy.hp;
        }
    }
}