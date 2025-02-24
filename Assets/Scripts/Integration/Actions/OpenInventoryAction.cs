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
    public class OpenInventoryAction : NeuroAction
    {
        public override string Name => "open_inventory";

        protected override string Description => "Open your inventory to manage your cards.";

        protected override JsonSchema Schema => new();

        protected override UniTask ExecuteAsync()
        {
            GameManager gm = GameManager.Instance;
            if(gm.gameMode != GameMode.Room) return UniTask.CompletedTask;

            gm.gameMode = GameMode.Inventory;
            gm.inventoryUI.gameObject.SetActive(true);
            gm.inventoryUI.Init();
            gm.inventoryUI.InitActionWindow();

            return UniTask.CompletedTask;
        }

        protected override ExecutionResult Validate(ActionJData actionData)
        {
            GameManager gm = GameManager.Instance;

            if(gm.gameMode != GameMode.Room)
                return ExecutionResult.Success("Cannot perform this action right now.");

            return ExecutionResult.Success();
        }
    }
}
