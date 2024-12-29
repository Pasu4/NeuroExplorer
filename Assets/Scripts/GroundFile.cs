﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts
{
    public class GroundFile : GroundObject
    {
        public long fileSize;

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

        protected override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        protected override void OnMouseDown()
        {
            base.OnMouseDown();

            if(GameManager.Instance.gameMode != GameMode.Room) return;

            GameManager gm = GameManager.Instance;
            Player player = gm.player;
            if(Vector2.Distance(player.transform.position, transform.position) > player.interactionRange)
                return;

            if(gm.deck.Any(c => c.filePath == displayPath))
            {
                gm.CreateTextEffect("Already copied", Color.red, transform.position);
                return;
            }

            if(gm.FreeStorage < card.fileSize)
            {
                gm.CreateTextEffect($"File too large\n({Utils.FileSizeString(card.fileSize)})", Color.red, transform.position);
                return;
            }

            gm.deck.Add(card);
            gm.CreateTextEffect("Copied", Color.green, transform.position);
        }

        public override void Init(Room room, string realPath)
        {
            base.Init(room, realPath);

            // Set sprite
            string extension = Path.GetExtension(realPath);
            IEnumerable<Sprite> possibleSprites = GameManager.Instance.fileSprites
                .Where(fs => fs.fileExtension == extension)
                .Select(fs => fs.sprite);

            if(possibleSprites.Count() == 0)
                possibleSprites = GameManager.Instance.defaultSprites;

            Sprite sprite = random.Choose(possibleSprites);
            GetComponent<SpriteRenderer>().sprite = sprite;

            // Set file size
            FileInfo info = new FileInfo(realPath);
            fileSize = info.Length;

            // Generate card
            card = new Card(displayPath, fileSize, sprite);
        }

        protected override void InitRandom()
        {
            random = GameManager.Instance.CreatePathRandom(DisplayName, "InitFile");
        }
    }
}