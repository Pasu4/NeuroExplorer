using System.Collections;
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

        public IEnumerator CStartScene()
        {
            gameObject.SetActive(true);

            yield return CWriteText("Pasu4", "Intro text aaaaaaaa bbbbbbbbbbb cccccccccccc ddddddddddd eeeeeef ffff g");

            difficultyDialog.SetActive(true);
            while(GameManager.Instance.difficulty == -1)
                yield return null;
            difficultyDialog.SetActive(false);

            yield return CWriteText("Pasu4", "More intro AAAAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHHHHHHHHH");

            gameObject.SetActive(false);
        }
    }
}