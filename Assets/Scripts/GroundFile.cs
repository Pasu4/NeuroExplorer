using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class GroundFile : GroundObject
    {
        Card card;

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

        public override void Init(Room room, string realPath)
        {
            base.Init(room, realPath);

            // Generate card
            card = new Card(realPath);

            // Set sprite
            string extension = Path.GetExtension(realPath);
            IEnumerable<Sprite> possibleSprites = GameManager.Instance.fileSprites
                .Where(fs => fs.fileExtension == extension)
                .Select(fs => fs.sprite);

            if(possibleSprites.Count() == 0)
                possibleSprites = GameManager.Instance.defaultSprites;

            Sprite sprite = random.Choose(possibleSprites);
            GetComponent<SpriteRenderer>().sprite = sprite;
        }

        protected override void InitRandom()
        {
            random = GameManager.Instance.CreatePathRandom(DisplayName, "InitFile");
        }
    }
}