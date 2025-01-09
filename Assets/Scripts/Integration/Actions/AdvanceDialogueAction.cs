using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Integration.Actions
{
    public class AdvanceDialogueAction : NeuroAction
    {
        public static string staticName = "advance_dialogue";
        public override string Name => staticName;

        protected override string Description => "Advance the dialogue.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.dialogueUI.neuroAdvance = true;
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            // No data expected
            if(GameManager.Instance.gameMode == GameMode.Dialogue)
                return ExecutionResult.Success();
            else
                return ExecutionResult.Failure("No dialogue to advance.");
        }
    }
}
