using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts
{
    public class MoveObject : MonoBehaviour
    {
        public Vector2 target;
        public AnimationCurve moveCurve;
        public float moveTime = 1f;
        public bool targetValid = false;

        Coroutine moveCoro = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetTarget(Vector2 newTarget)
        {
            // If already near position
            if(Vector2.SqrMagnitude(newTarget - (Vector2) transform.localPosition) < 0.01f && targetValid)
            {
                transform.localPosition = newTarget;
                target = newTarget;
                return;
            }

            // If same target
            if(newTarget == target && targetValid)
            {
                return;
            }

            // Move
            if(moveCoro != null)
            {
                StopCoroutine(moveCoro);
            }
            targetValid = true;
            target = newTarget;
            moveCoro = StartCoroutine(CMoveTo());
        }

        private IEnumerator CMoveTo()
        {
            Vector2 start = transform.localPosition;
            for(float t = 0f; t < moveTime; t += Time.deltaTime)
            {
                transform.localPosition = Vector2.Lerp(start, target, moveCurve.Evaluate(t / moveTime));
                yield return null;
            }
        }
    }
}