using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class Encounter : MonoBehaviour
    {
        public Enemy[] enemies;
        public string encounterId;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                GameManager.Instance.StartBattle(enemies, encounterId);
            }
        }
    }
}