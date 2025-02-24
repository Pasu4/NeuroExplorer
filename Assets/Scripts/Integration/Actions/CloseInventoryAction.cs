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
    public class CloseInventoryAction : NeuroAction
    {
        public override string Name => "close_inventory";

        protected override string Description => "Close your inventory.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager gm = GameManager.Instance;
            gm.gameMode = GameMode.Room;
            gm.inventoryUI.gameObject.SetActive(false);
            gm.NeuroRoomStart();
            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            if(GameManager.Instance.gameMode != GameMode.Inventory)
                return ExecutionResult.Success("You cannot close your inventory if it is not open.");
            return ExecutionResult.Success();
        }
    }
}
