using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Integration.Actions
{
    public class RoomPickupItemAction : NeuroAction<string>
    {
        public static readonly string staticName = "pick_up_item";
        public override string Name => staticName;

        protected override string Description => "Pick up an item from the floor. You can only pick up items you can currently see.";

        protected override JsonSchema Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = { "name" },
            Properties =
            {
                ["name"] = QJS.Enum(GameManager.Instance.room.groundObjects
                    .Select(go => go.GetComponent<GroundFile>())
                    .Where(file => file != null && file.CanNeuroPickUp())
                    .Select(file => file.DisplayName)
                )
            }
        };

        protected override UniTask ExecuteAsync(string parsedData)
        {
            GameManager.Instance.player.NeuroPickUp(parsedData);
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData, out string parsedData)
        {
            parsedData = actionData?.Data?["name"]?.Value<string>();
            string name = parsedData;

            // Check for game mode
            if(GameManager.Instance.gameMode != GameMode.Room)
                return ExecutionResult.Failure("Someone tell Pasu4 there is a problem with his code.");

            // Check if the name is null
            if(name is null)
                return ExecutionResult.Failure("Action failed. Missing required parameter 'name'.");

            // Try to find a target
            GameObject target = GameManager.Instance.room.groundObjects.FirstOrDefault(go => go.TryGetComponent(out GroundFile f) && f.DisplayName == name);

            // Check if the target is in range
            float range = GameManager.Instance.neuroVisionRange;
            Vector2 playerPos = GameManager.Instance.player.transform.position;
            if(target == null || Vector2.Distance(target.transform.position, playerPos) > range)
                return ExecutionResult.Failure("Action failed. The specified item does not exist or is outside of your vision range.");
            GroundFile file = target.GetComponent<GroundFile>();

            // Check if Neuro has enough space for the item
            if(file.fileSize > GameManager.Instance.FreeStorage)
                return ExecutionResult.Failure("Action failed. You don't have enough storage space for this item.");

            // Check if the item is already in the inventory
            if(GameManager.Instance.deck.Any(c => c.filePath == file.displayPath))
                return ExecutionResult.Failure("Action failed. You already have this item in your inventory.");

            return ExecutionResult.Success();
        }
    }
}
