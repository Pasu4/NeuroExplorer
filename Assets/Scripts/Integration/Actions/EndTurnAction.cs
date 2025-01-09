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
    public class EndTurnAction : NeuroAction
    {
        public static readonly string staticName = "end_turn";
        public override string Name => staticName;

        protected override string Description => "End your turn, allowing the enemies to act.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager.Instance.battleUI.playerTurn = false;
            GameManager.Instance.battleUI.waitingForAction = false;
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            // Check gamemode
            if(GameManager.Instance.gameMode != GameMode.Battle)
                return ExecutionResult.Failure("Someone tell Pasu4 there is a problem with his code.");

            // Check player turn status
            if(!GameManager.Instance.battleUI.playerTurn)
                return ExecutionResult.Failure("Someone tell Pasu4 there is a problem with his code.");

            return ExecutionResult.Success();
        }
    }
}
