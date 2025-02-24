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
    public class RemoveCardAction : NeuroAction<string>
    {
        public override string Name => "remove_card";

        protected override string Description => "Permanently remove a card from your deck.";

        protected override JsonSchema Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = { "name" },
            Properties =
            {
                ["name"] = QJS.Enum(GameManager.Instance.deck.Select(c => c.name))
            }
        };

        protected override UniTask ExecuteAsync(string parsedData)
        {
            GameManager gm = GameManager.Instance;
            if(gm.deck.Any(c => c.name == parsedData))
                gm.inventoryUI.NeuroRemove(parsedData);

            if(gm.gameMode == GameMode.Inventory)
                gm.inventoryUI.InitActionWindow();
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData, out string parsedData)
        {
            parsedData = actionData?.Data?["name"]?.Value<string>();
            string name = parsedData;

            GameManager gm = GameManager.Instance;

            if(gm.gameMode != GameMode.Inventory)
                return ExecutionResult.Success("Someone tell Pasu4 there is a problem with his code.");

            if(name is null)
                return ExecutionResult.Failure("Action failed. Missing required parameter 'name'.");

            if(!gm.deck.Any(c => c.name == name))
                return ExecutionResult.Failure($"Action failed. You don't have a card called \"{name}\"");

            return ExecutionResult.Success($"Removed card \"{name}\" from your deck.");
        }
    }
}
