using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        InputAction moveAction;
        Animator animator;

        public float speed = 1f;
        public float interactionRange = 4.0f;

        private Coroutine currentMoveCoro;
        private Vector2 neuroInput;

        // Use this for initialization
        void Start()
        {
            GameManager.Instance.player = this;

            moveAction = InputSystem.actions.FindAction("Move");
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(GameManager.Instance.gameMode != GameMode.Room)
            {
                animator.SetBool("Moving", false);
                return;
            }

            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            if(moveInput == Vector2.zero) // Only let Neuro move if a human isn't moving
                moveInput = neuroInput;

            if(moveInput != Vector2.zero)
            {
                transform.Translate(speed * Time.deltaTime * moveInput);

                // Animation
                animator.SetBool("Moving", true);

                animator.SetBool("MoveUp",    moveInput.y >= Mathf.Abs(moveInput.x));
                animator.SetBool("MoveDown", -moveInput.y >= Mathf.Abs(moveInput.x));
                animator.SetBool("MoveRight", moveInput.x >  Mathf.Abs(moveInput.y));
                animator.SetBool("MoveLeft", -moveInput.x >  Mathf.Abs(moveInput.y));
            }
            else
            {
                animator.SetBool("Moving", false);
            }
        }

        public void NeuroExploreRoom()
        {
            Rect rect = GameManager.Instance.room.walkableRect;
            Vector2 position = new(Random.Range(rect.xMin, rect.xMax), Random.Range(rect.yMin, rect.yMax));
            StartCoroutine(CNeuroExplore(position));
        }

        public void NeuroAscend()
        {
            GroundDir upDir = GameManager.Instance.room.groundObjects
                .Select(o => o.GetComponent<GroundDir>())
                .FirstOrDefault(d => d != null && d.isUpDir);

            if(upDir == null) return;

            if(currentMoveCoro != null)
                StopCoroutine(currentMoveCoro);
            currentMoveCoro = StartCoroutine(CNeuroClick(upDir, false));
        }

        public void NeuroDescend(string dirName)
        {
            GroundDir dir = GameManager.Instance.room.groundObjects
                .Select(o => o.GetComponent<GroundDir>())
                .FirstOrDefault(d => d != null && d.DisplayName == dirName);

            if(dir == null) return;

            if(currentMoveCoro != null)
                StopCoroutine(currentMoveCoro);
            currentMoveCoro = StartCoroutine(CNeuroClick(dir, false));
        }

        public void NeuroPickUp(string fileName)
        {
            GroundFile file = GameManager.Instance.room.groundObjects
                .Select(o => o.GetComponent<GroundFile>())
                .FirstOrDefault(f => f != null && f.DisplayName == fileName);

            if(file == null) return;

            if(currentMoveCoro != null)
                StopCoroutine(currentMoveCoro);
            currentMoveCoro = StartCoroutine(CNeuroClick(file, true));
        }

        public void NeuroHeal()
        {
            Heal heal = FindFirstObjectByType<Heal>();

            if(heal == null) return;

            if(currentMoveCoro != null)
                StopCoroutine(currentMoveCoro);
            currentMoveCoro = StartCoroutine(CNeuroHeal(heal));
        }

        public void NeuroMoveTo(Vector2 position)
        {
            if(currentMoveCoro != null)
                StopCoroutine(currentMoveCoro);
            currentMoveCoro = StartCoroutine(CNeuroMoveTo(position));
        }

        private IEnumerator CNeuroMoveTo(Vector2 targetPos)
        {
            while(Vector2.Distance(transform.position, targetPos) > Time.deltaTime * speed && GameManager.Instance.gameMode == GameMode.Room)
            {
                neuroInput = (targetPos - (Vector2) transform.position).normalized;
                yield return null;
            }

            neuroInput = Vector2.zero;
        }

        private IEnumerator CNeuroExplore(Vector2 target)
        {
            yield return CNeuroMoveTo(target);
            GameManager.Instance.NeuroRoomStart();
        }

        private IEnumerator CNeuroClick(GroundObject obj, bool register)
        {
            yield return CNeuroMoveTo(obj.transform.position);
            obj.Click();

            if(register)
                GameManager.Instance.NeuroRoomStart();
        }

        private IEnumerator CNeuroHeal(Heal heal)
        {
            yield return CNeuroMoveTo(heal.transform.position);
            heal.Click();
            GameManager.Instance.NeuroRoomStart();
        }
    }
}