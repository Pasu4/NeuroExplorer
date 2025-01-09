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
    public class SetObfuscateAction : NeuroAction
    {
        public static string staticName = "activate_streamer_mode";
        public override string Name => staticName;

        protected override string Description => "Activates streamer mode, which obfuscates the names of files and folders for the purpose of privacy.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.SetObfuscate(true);
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            if(GameManager.Instance.obfuscateSet)
                return ExecutionResult.Success("Streamer mode has already been set.");
            else
                return ExecutionResult.Success("Activated streamer mode.");
        }
    }
}
