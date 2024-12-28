using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class GroundDir : GroundObject
    {

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        protected override void InitRandom()
        {
            random = GameManager.Instance.CreatePathRandom(DisplayName, "InitDir");
        }
    }
}