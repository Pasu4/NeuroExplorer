using Assets.Scripts.Integration.ActionData;
using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Integration.Actions
{
    public class SelectPlayerAction : NeuroAction<int>
    {
        public static string staticName = "select_player";

        private static readonly Dictionary<string, int> difficulties = new()
        {
            ["Neuro"] = 0,
            ["Evil"] = 1,
            ["???"] = 2
        };

        public override string Name => staticName;

        protected override string Description => "Select who you are going to play as.";

        protected override JsonSchema Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = { "player" },
            Properties =
            {
                ["player"] = QJS.Enum(new[] { "Neuro", "Evil", "???" })
            }
        };

        protected override UniTask ExecuteAsync(int parsedData)
        {
            GameManager.Instance.SetDifficulty(parsedData);
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData, out int parsedData)
        {
            string playerName = actionData.Data?["player"]?.Value<string>();

            if(difficulties.TryGetValue(playerName, out parsedData))
                return ExecutionResult.Success();
            else
                return ExecutionResult.Failure("Action failed. Invalid parameter 'player'.");
        }
    }
}
