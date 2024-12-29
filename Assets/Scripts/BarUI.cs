using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class BarUI : MonoBehaviour
    {
        public float value;
        public float maxValue;
        public float maxWidth;
        public Gradient gradient;

        private RectTransform rt;
        private Image img;

        // Use this for initialization
        void Start()
        {
            rt = GetComponent<RectTransform>();
            img = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 sd = rt.sizeDelta;
            sd.x = maxWidth * value / maxValue;
            rt.sizeDelta = sd;
            img.color = gradient.Evaluate(value / maxValue);
        }
    }
}