using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.Integration.Actions
{
    public class RoomDescendAction : NeuroAction<string>
    {
        public static readonly string staticName = "room_descend";
        public override string Name => staticName;

        protected override string Description => "Descend a ladder to a room below. You can only go to rooms you can currently see.";

        protected override JsonSchema Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = { "name" },
            Properties =
            {
                ["name"] = QJS.Enum(GameManager.Instance.room.groundObjects
                    .Select(go => go.GetComponent<GroundDir>())
                    .Where(dir => dir != null && !dir.locked && Vector2.Distance(dir.transform.position, GameManager.Instance.player.transform.position) < GameManager.Instance.neuroVisionRange)
                    .Select(dir => dir.DisplayName)
                )
            }
        };

        protected override UniTask ExecuteAsync(string parsedData)
        {
            GameManager.Instance.player.NeuroDescend(parsedData);
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
            GameObject target = GameManager.Instance.room.groundObjects.FirstOrDefault(go => go.TryGetComponent(out GroundDir dir) && dir.DisplayName == name);
            
            // Check if a target exists an is in range
            float range = GameManager.Instance.neuroVisionRange;
            Vector2 playerPos = GameManager.Instance.player.transform.position;
            if(target == null || Vector2.Distance((Vector2) target.transform.position, playerPos) > range)
                return ExecutionResult.Failure("Action failed. The specified room does not exist or is outside of your vision range.");

            return ExecutionResult.Success();
        }
    }
}
