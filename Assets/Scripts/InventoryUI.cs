using Assets.Scripts.Integration;
using Assets.Scripts.Integration.Actions;
using NeuroSdk.Actions;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public void InitActionWindow()
        {
            GameManager gm = GameManager.Instance;
            ActionWindow.Create(gameObject)
                .AddAction(new CloseInventoryAction())
                .AddActionIf(new RemoveCardAction(), gm.deck.Count > 0)
                .SetForce(0, "Remove a card or close your inventory.", GetInventoryState(), false)
                .SetEnd(() => gm.gameMode != GameMode.Inventory)
                .Register();
        }

        public void NeuroRemove(string cardName)
        {
            GameManager gm = GameManager.Instance;
            Card card = gm.deck.FirstOrDefault(x => x.name == cardName);
            if(card is null)
            {
                Debug.LogError($"Could not find a card with the name '{cardName}' to remove.");
                return;
            }
            gm.deck.Remove(card);

            CardUI cardUI = GetComponentsInChildren<CardUI>().FirstOrDefault(c => c.title.text == cardName);
            if(cardUI != null)
                Destroy(cardUI.transform.parent.gameObject);
            else
                Debug.LogError("Could not find a CardUI to remove.");
                
        }

        private string GetInventoryState()
        {
            var state = GameManager.Instance.deck.Select(card => new
            {
                card.name,
                type = card.type.ToString(),
                mpCost = Utils.FileSizeString(card.fileSize),
                description = card.GetDescription().TrimRTF(),
            }).ToList();
            return JsonConvert.SerializeObject(state, Formatting.None);
        }
    }
}