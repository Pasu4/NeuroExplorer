using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Integration.Actions
{
    public class EngageBossAction : NeuroAction
    {
        public static readonly string staticName = "engage_boss";
        public override string Name => staticName;

        protected override string Description => "Engage the boss and commence battle.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.player.NeuroMoveTo(GameObject.FindFirstObjectByType<BossTrigger>().transform.position);
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            // Check if there is a boss in the room
            if(GameObject.FindFirstObjectByType<BossTrigger>() == null)
                return ExecutionResult.Failure("Someone tell Pasu4 there is a problem with his code.");

            return ExecutionResult.Success();
        }
    }
}
