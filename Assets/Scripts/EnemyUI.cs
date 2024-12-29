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
        public Image nextActionImage;
        public TextMeshProUGUI nextActionText;
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
            hpText.text = Utils.FileSizeString(enemy.hp);

            if(enemy.nextAction is AttackAction a)
            {
                nextActionImage.sprite = GameManager.Instance.enemyActionAttackSprite;
                nextActionText.text = (a.times > 1 ? a.times + "x " : "") + Utils.FileSizeString(a.damage);
            }
            else if(enemy.nextAction is DefendAction d)
            {
                nextActionImage.sprite = GameManager.Instance.enemyActionDefendSprite;
                nextActionText.text = Utils.FileSizeString(d.block);
            }
            else if(enemy.nextAction is TrojanAction)
            {
                nextActionImage.sprite = GameManager.Instance.enemyActionTrojanSprite;
                nextActionText.text = "";
            }
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