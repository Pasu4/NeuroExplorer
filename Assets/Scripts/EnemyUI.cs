﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class EnemyUI : MonoBehaviour, IPointerDownHandler
    {
        public TextMeshProUGUI hpText;
        public BarUI hpBar;
        public Image nextActionImage;
        public TextMeshProUGUI nextActionText;
        public Enemy enemy;
        public BattleUI battleUI;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            hpBar.maxValue = enemy.maxHp;
            hpBar.value = enemy.hp;
            hpText.text = enemy.block > 0 ? $"{Utils.FileSizeString(enemy.hp)}\n<color=#66d>+ {Utils.FileSizeString(enemy.block)}</color>" : Utils.FileSizeString(enemy.hp);

            nextActionImage.sprite = enemy.nextAction.Sprite;
            nextActionText.text = enemy.nextAction.Text;
        }

        public void Click()
        {
            if(battleUI.selectedCard != null)
            {
                battleUI.targetEnemy = this;
                battleUI.PlayCard(battleUI.selectedCard);
            }
        }

        public void OnPointerDown(PointerEventData ev)
        {
            Click();
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