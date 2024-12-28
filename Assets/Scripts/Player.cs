using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        InputAction moveAction;
        [SerializeField] float speed = 1f;

        // Use this for initialization
        void Start()
        {
            moveAction = InputSystem.actions.FindAction("Move");
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            if(moveInput != Vector2.zero)
            {
                transform.Translate(speed * Time.deltaTime * moveInput);
            }
        }
    }
}