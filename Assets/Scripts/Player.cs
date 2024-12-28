using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        InputAction moveAction;
        Animator animator;
        [SerializeField] float speed = 1f;

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
            if(GameManager.Instance.gameMode != GameMode.Room) return;

            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            if(moveInput != Vector2.zero)
            {
                transform.Translate(speed * Time.deltaTime * moveInput);

                // Animation
                animator.SetBool("Moving", true);

                animator.SetBool("MoveUp",    moveInput.y > Mathf.Abs(moveInput.x));
                animator.SetBool("MoveDown", -moveInput.y > Mathf.Abs(moveInput.x));
                animator.SetBool("MoveRight", moveInput.x > Mathf.Abs(moveInput.y));
                animator.SetBool("MoveLeft", -moveInput.x > Mathf.Abs(moveInput.y));
            }
            else
            {
                animator.SetBool("Moving", false);
            }
        }
    }
}