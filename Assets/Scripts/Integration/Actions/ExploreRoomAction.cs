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
    public class ExploreRoomAction : NeuroAction
    {
        public static readonly string staticName = "explore_room";

        public override string Name => staticName;

        protected override string Description => "Explore the current room.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.player.NeuroExploreRoom();
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            if(GameManager.Instance.gameMode != GameMode.Room)
            {
                return ExecutionResult.Failure("Someone tell Pasu4 there's a problem with his code.");
            }

            return ExecutionResult.Success();
        }
    }
}
