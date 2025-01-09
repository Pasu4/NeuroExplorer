using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using System;
using UnityEngine;

namespace Assets.Scripts.Integration.Actions
{
    public class HealAction : NeuroAction
    {
        public static readonly string staticName = "heal";
        public override string Name => staticName;

        protected override string Description => "Use the healing point in this room to recover all data integrity (health).";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.player.NeuroHeal();
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            // Check if the room contains a healing item
            Heal heal = GameObject.FindFirstObjectByType<Heal>();

            if(heal == null)
                return ExecutionResult.Failure("Someone tell Pasu4 there is a problem with his code.");

            return ExecutionResult.Success();
        }
    }
}
