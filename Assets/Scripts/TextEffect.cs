using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class TextEffect : MonoBehaviour
    {
        [Min(0.001f)]
        public float disappearTime;
        public float disappearHeight;

        TextMeshProUGUI tmp;
        Color originalColor;
        Color targetColor;
        float t;

        // Use this for initialization
        void Start()
        {
            tmp = GetComponentInChildren<TextMeshProUGUI>();
            originalColor = tmp.color;
            targetColor = originalColor;
            targetColor.a = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if(t < disappearTime)
            {
                transform.Translate(disappearHeight * Time.deltaTime * Vector2.up / disappearTime);
                tmp.color = Color.Lerp(originalColor, targetColor, t / disappearTime);
                t += Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}