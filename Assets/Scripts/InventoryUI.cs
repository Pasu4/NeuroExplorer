using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
            // Delete old cards
            foreach(Transform child in contentTransform)
            {
                Destroy(child.gameObject);
            }

            // Spawn new cards
            List<Card> cards = GameManager.Instance.deck;
            foreach(Card card in cards)
            {
                GameObject go = Instantiate(cardPrefab, contentTransform);
                go.GetComponentInChildren<CardUI>().SetCard(card);
                go.GetComponentInChildren<Button>().onClick.AddListener(() =>
                {
                    GameManager.Instance.deck.Remove(card);
                    Destroy(go);
                });
            }
        }
    }
}