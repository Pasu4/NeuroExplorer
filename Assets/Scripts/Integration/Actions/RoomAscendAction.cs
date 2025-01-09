using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using System.Linq;

namespace Assets.Scripts.Integration.Actions
{
    public class RoomAscendAction : NeuroAction
    {
        public static readonly string staticName = "room_ascend";
        public override string Name => staticName;

        protected override string Description => "Climb up the ladder to the room above.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.player.NeuroAscend();
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            // Check if there is a ladder up
            if(!GameManager.Instance.room.groundObjects.Any(go => go.TryGetComponent(out GroundDir dir) && dir.isUpDir))
                return ExecutionResult.Failure("Action failed. You cannot go up from the current room.");

            // Check if the game mode is correct
            if(GameManager.Instance.gameMode != GameMode.Room)
                return ExecutionResult.Failure("Someone tell Pasu4 there is a problem with his code.");

            return ExecutionResult.Success();
        }
    }
}
