using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class GroundChest : GroundObject
    {
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void InitRandom()
        {
            random = GameManager.Instance.CreatePathRandom(displayPath, "InitChest");
        }
    }
}