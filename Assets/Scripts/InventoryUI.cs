using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class InventoryUI : MonoBehaviour
    {
        public RectTransform contentTransform;
        public GameObject cardPrefab;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Init()
        {
            List<Card> cards = GameManager.Instance.deck;
            foreach(Card card in cards)
            {
                GameObject go = Instantiate(cardPrefab, contentTransform);
                go.GetComponent<CardUI>().SetCard(card);
            }
        }
    }
}