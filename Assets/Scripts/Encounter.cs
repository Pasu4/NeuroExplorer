using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(Collider2D))]
    public class Encounter : MonoBehaviour
    {
        public Enemy[] enemies;
        public string encounterId;
        public float detectionRange;
        public float speed;

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if(GameManager.Instance.gameMode != GameMode.Room)
                return;

            if(Vector2.Distance(GameManager.Instance.player.transform.position, transform.position) < detectionRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, GameManager.Instance.player.transform.position, speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("Player"))
            {
                GameManager.Instance.StartBattle(enemies, encounterId, false, GameManager.Instance.battleClip);
                Destroy(gameObject);
            }
        }

        public void Init(string id, long strength)
        {
            encounterId = id;

            System.Random random = GameManager.Instance.CreatePathRandom(encounterId, "EncounterInit");

            speed = random.Range(2, 3);
            detectionRange = random.Range(2, 5);

            int enemyCount = random.Next(1, 4 + GameManager.Instance.difficulty);
            enemies = random.ChooseMany(GameManager.Instance.enemies, enemyCount).Select(e => e.Copy()).ToArray();
            foreach(Enemy enemy in enemies)
            {
                enemy.strength = strength;
            }
            GetComponent<SpriteRenderer>().sprite = new System.Random().Choose(enemies.Select(e => e.sprite));
        }
    }
}