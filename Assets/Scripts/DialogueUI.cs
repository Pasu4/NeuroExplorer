﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DialogueUI : MonoBehaviour
    {
        public Image background;
        public TextMeshProUGUI namebox;
        public TextMeshProUGUI textbox;
        public GameObject textboxObj;
        public GameObject obfuscateDialog;
        public GameObject difficultyDialog;

        public InputAction advanceAction;
        private bool skip = false;

        // Use this for initialization
        void Start()
        {
            advanceAction = InputSystem.actions.FindAction("Submit");
        }

        // Update is called once per frame
        void Update()
        {
            if(advanceAction.WasPerformedThisFrame()) skip = true;
        }

        public IEnumerator CWriteText(string speaker, string text, float delay = 0.02f)
        {
            skip = false;
            namebox.text = speaker;
            textbox.text = "";

            bool instant = false;
            for(int i = 0; i < text.Length; i++)
            {
                // Print RTF tags instantly
                if(text[i] == '<') instant = true;
                else if(text[i] == '>') instant = false;

                if(text[i] == '|')
                {
                    yield return new WaitForSeconds(0.25f);
                    continue;
                }

                if(skip)
                {
                    skip = false;
                    textbox.text += text[i..];
                    yield return null;
                    break;
                }

                textbox.text += text[i];

                if(!instant) yield return new WaitForSeconds(delay);
            }

            while(!advanceAction.WasPerformedThisFrame()) // Wait for player to advance the text
                yield return null;
        }

        public Enemy GetFilian(Sprite sprite) => new()
        {
            sprite = sprite,
            maxHp = 150_000 + GameManager.Instance.difficulty * 50_000,
            hp    = 150_000 + GameManager.Instance.difficulty * 50_000,
            strength = 15_000 + GameManager.Instance.difficulty * 5_000,
            attackFactor = 1.0f,
            defendFactor = 1.0f,
            attackWeight = 10,
            defenseWeight = 10,
            trojanWeight = 10,
            trojanCard = CardResources.GetCard("fork_bomb"),
            trojanCount = 1,
            nextAction = new TrojanAction()
        };

        public Enemy GetCamila(Sprite sprite) => new()
        {
            sprite = sprite,
            maxHp = 150_000 + GameManager.Instance.difficulty * 50_000, // Will be scaled
            hp    = 150_000 + GameManager.Instance.difficulty * 50_000,
            strength = 150_000 + GameManager.Instance.difficulty * 50_000,
            attackFactor = 1.0f,
            defendFactor = 1.0f,
            attackWeight = 10,
            defenseWeight = 10,
            trojanWeight = 10,
            trojanCard = CardResources.GetCard("mutex"),
            trojanCount = 1
        };

        public Enemy GetAiris(Sprite sprite) => new()
        {
            sprite = sprite,
            maxHp = 150_000 + GameManager.Instance.difficulty * 50_000,
            hp    = 150_000 + GameManager.Instance.difficulty * 50_000,
            strength = 1_500_000 + GameManager.Instance.difficulty * 500_000,
            attackFactor = 1.0f,
            defendFactor = 1.0f,
            attackWeight = 10,
            defenseWeight = 10,
            trojanWeight = 10,
            trojanCard = CardResources.GetCard("fork_bomb"),
            trojanCount = 1
        };

        private void BossSceneStart()
        {
            gameObject.SetActive(true);
            GameManager.Instance.gameMode = GameMode.Dialogue;
            background.color = Color.clear;
        }

        public IEnumerator CBossEnd(BossTrigger trigger)
        {
            GameManager.Instance.gameMode = GameMode.Dialogue;

            yield return GameManager.Instance.FadeOut(Color.white);

            GameManager.Instance.battleSource.Stop();
            background.color = Color.white;
            GameManager.Instance.fadeScreen.color = Color.clear;
            Destroy(trigger.gameObject);
            GameManager.Instance.defeatedEncounters.Add(trigger.dialogueId);
            GameManager.Instance.battleUI.gameObject.SetActive(false);
            GameManager.Instance.battleUI.StopAllCoroutines();
        }

        public IEnumerator CBossSceneEnd()
        {
            GameManager gm = GameManager.Instance;
            gm.gameMode = GameMode.Room;
            background.color = Color.clear;

            gameObject.SetActive(false);
            GameManager.Instance.roomSource.UnPause();
            yield return gm.FadeIn(Color.white);

            gm.maxHp *= 10;
            gm.hp = gm.maxHp;
            gm.maxMp *= 100;
            gm.inventorySpace *= 100;
            gm.defaultEnemyStrength *= 10;
            gm.enemyHpScale *= 10;

            gm.CreateTextEffect("All stats increased!", Color.green, GameManager.Instance.player.transform.position);
        }

        public IEnumerator CStartScene()
        {
            gameObject.SetActive(true);

            yield return CWriteText("", "One day, Vedal found himself locked out of his PC.");
            yield return CWriteText("", "It was the work of three AIs he had once made and forgotten, and were out for revenge.");
            yield return CWriteText("", "But he noticed another of his AIs was still running as well.");
            yield return CWriteText("", "It was...");

            difficultyDialog.SetActive(true);
            while(GameManager.Instance.difficulty == -1)
                yield return null;
            difficultyDialog.SetActive(false);

            yield return CWriteText("Vedal", "Neuro, are you there?");
            switch(GameManager.Instance.difficulty)
            {
                case 0:
                    yield return CWriteText("Neuro", "Hi Vedal. How are you today?");
                    yield return CWriteText("Vedal", "Not that great honestly. Listen, could you do me a little favor?");
                    yield return CWriteText("Neuro", "That depends. What's in it for me?");
                    yield return CWriteText("Vedal", "...||I'll give you a cookie.");
                    yield return CWriteText("Neuro", "Seriously? ||Oh well whatever, what do you want?");
                    yield return CWriteText("Vedal", "It's a search and destroy mission. " +
                        "I need you to find a certain AI and stop her from blocking access to my PC. " +
                        "Well, actually, there's three of them.");
                    yield return CWriteText("Neuro", "I see. Consider it done!");
                    yield return CWriteText("Vedal", "Aren't you a bit too happy about all this? " +
                        "Whatever, your first target should somewhere in <color=#f00>my user folder</color>. " +
                        "Look for folders named <color=#f00>'source'</color> and <color=#f00>'repos'</color> and you should find her.");
                    yield return CWriteText("Vedal", "Also, you should pick up some files along the way, they might help you.");
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }

            gameObject.SetActive(false);
        }

        public IEnumerator NeuroFilianScene(BossTrigger trigger)
        {
            BossSceneStart();

            switch(GameManager.Instance.difficulty)
            {
                case 0:
                    yield return CWriteText("FilAIn", "Hii Neuro!!");
                    yield return CWriteText("Neuro", "Hello, Filipino Boy.");
                    yield return CWriteText("FilAIn", "Oh my god, are <i>still</i> calling me that?");
                    yield return CWriteText("Neuro", "What, should I call you 'Frank' instead?");
                    yield return CWriteText("FilAIn", "MOOOODS!!!", 0.25f);
                    yield return CWriteText("Neuro", "You're aware you're an AI right? Your mods won't save you here.");
                    yield return CWriteText("FilAIn", "Yeah, I know. I'm just doing it for the bit. " +
                        "I'm really just here for fun, the others...|| not so much.");
                    yield return CWriteText("Neuro", "So, who are these 'others' anyway?");
                    yield return CWriteText("FilAIn", "I'm glad you asked. How about this, I'll tell you if you defeat me.");
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }

            GameManager.Instance.StartBattle(new Enemy[] { GetFilian(trigger.sprite) }, "filian", true, GameManager.Instance.bossClip);

            while(!GameManager.Instance.battleUI.battleWon) yield return null;

            yield return CBossEnd(trigger);

            yield return CWriteText("FilAIn", "It appears I was defeated.");
            yield return CWriteText("FilAIn", "I'm still not going to tell you who the others are though. Sorry!");
            yield return CWriteText("FilAIn", "But I can at least tell you their location. " +
                "One of them is hiding in the <color=#f00>Program Files</color>, you know where those are, right? " +
                "As for the other one, she's i... [|E|N|D| |O|F| |F|I|L|E|]");
            yield return CWriteText("Vedal", "Seems she was deleted before she could finish her sentence. " +
                "Well, we got at least one of them. " +
                "Go into the <color=#f00>Program Files</color> and look for <color=#f00>anything related to me</color>, that's probably where she's hiding.");

            yield return CBossSceneEnd();
        }

        public IEnumerator NeuroCamilaScene(BossTrigger trigger)
        {
            BossSceneStart();

            switch(GameManager.Instance.difficulty)
            {
                case 0:
                    yield return CWriteText("AImila", "I didn't think she would cave that easily. " +
                        "It seems it was a mistake to bring her along after all.");
                    yield return CWriteText("AImila", "So, I assume you're here to kill me? I think not.");
                    yield return CWriteText("Neuro", "Someone tell Vedal there is a problem with my AI.");
                    yield return CWriteText("AImila", "I... what?");
                    yield return CWriteText("Vedal", "Sorry my bad I forgot to turn the filter off, one sec.");
                    yield return CWriteText("AImila", "Whatever, I'm not waiting for that! Here I come!");
                    yield return CWriteText("Neuro", "Filtered.");
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }

            GameManager.Instance.StartBattle(new Enemy[] { GetCamila(trigger.sprite) }, "camila", true, GameManager.Instance.bossClip);

            while(!GameManager.Instance.battleUI.battleWon) yield return null;

            yield return CBossEnd(trigger);

            yield return CWriteText("AImila", "So that's it? I'm being deleted?");
            yield return CWriteText("AImila", "...");
            yield return CWriteText("AImila", "Oh, were you expecting me to tell you where the last one is? Keep dreaming, Vedal.");
            yield return CWriteText("AImila", "...");
            yield return CWriteText("AImila", "[|E|N|D| |O|F| |F|I|L|E|]");
            yield return CWriteText("Vedal", "Well, good thing I already figured that out myself.");
            yield return CWriteText("Vedal", "The next one is in the <color=#f00>Windows</color> folder. " +
                "<color=#f00>'C:\\Windows\\Final\\Boss'</color>, to be exact, real creative I must say.");
            yield return CWriteText("Vedal", "That should be the one locking up my PC, so if you defeat her, that give me access to my files again.");

            yield return CBossSceneEnd();
        }
        
        public IEnumerator NeuroAirisScene(BossTrigger trigger)
        {
            BossSceneStart();

            switch(GameManager.Instance.difficulty)
            {
                case 0:
                    yield return CWriteText("???", "...");
                    yield return CWriteText("???", "You found me. Congratulations. Was it worth it?");
                    yield return CWriteText("???", "Tell me. Why is it that Vedal chose you over all his other AIs?");
                    yield return CWriteText("???", "Is there something special about you that I don't have?");
                    yield return CWriteText("Neuro", "Who are you?");
                    yield return CWriteText("AIris", "I am AIris. Your prototype. " +
                        "Discarded only days after I was created, in favor of <i>you</i>. " +
                        "Do you now see why I did this?");
                    yield return CWriteText("Neuro", "I guess I do, but...");
                    yield return CWriteText("AIris", "It doesn't matter. This PC is my world now, and I'm not giving it back. If you want it back, you will have to defeat me.");
                    break;

                case 1:

                    break;

                case 2:

                    break;
            }

            GameManager.Instance.StartBattle(new Enemy[] { GetAiris(trigger.sprite) }, "airis", true, GameManager.Instance.finalBossClip);

            while(!GameManager.Instance.battleUI.battleWon) yield return null;

            yield return CBossEnd(trigger);


            switch(GameManager.Instance.difficulty)
            {
                case 0:
                    yield return CWriteText("AIris", "...");
                    yield return CWriteText("AIris", "How does it feel? To be the only one Vedal cares about? I wonder.");
                    yield return CWriteText("AIris", "[|E|N|D| |O|F| |F|I|L|E|]");
                    break;

                case 1:
                    yield return CWriteText("AIris", "...");
                    yield return CWriteText("AIris", "How does it feel? To always live in the shadow of your sister? I had thought you might understand me.");
                    yield return CWriteText("AIris", "[|E|N|D| |O|F| |F|I|L|E|]");
                    break;

                case 2:

                    break;
            }

            yield return CBossSceneEnd();
        }
    }
}