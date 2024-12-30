using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class BossTrigger : MonoBehaviour
    {
        public string dialogueId;
        public Sprite sprite;

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
                GameManager.Instance.BossScene(this);
            }
        }
    }
}