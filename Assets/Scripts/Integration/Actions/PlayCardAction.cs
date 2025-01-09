using Assets.Scripts.CardEffects;
using Assets.Scripts.Integration.ActionData;
using Cysharp.Threading.Tasks;
using NeuroSdk.Actions;
using NeuroSdk.Json;
using NeuroSdk.Websocket;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Assets.Scripts.Integration.Actions
{
    public class PlayCardAction : NeuroAction<PlayCardActionData>
    {
        private BattleUI battleUI;

        public static readonly string staticName = "play_card";
        public override string Name => staticName;

        protected override string Description => "Play the card with the specified index. Only set a target if the card requires it.";

        protected override JsonSchema Schema => new()
        {
            Type = JsonSchemaType.Object,
            Required = { "index" },
            Properties =
            {
                ["index"] = new() {
                    Type = JsonSchemaType.Integer,
                    Minimum = 0,
                    Maximum = battleUI.handCards.Count - 1
                },
                ["target"] = QJS.Enum(battleUI.enemies.Select(e => e.enemy.name))
            }
        };

        public PlayCardAction(BattleUI battleUI)
        {
            this.battleUI = battleUI;
        }

        protected override async UniTask ExecuteAsync(PlayCardActionData parsedData)
        {
            // Click the card
            battleUI.handCards[parsedData.index].Click();

            if(parsedData.target is not null)
            {
                // Wait 0.5s
                await UniTask.WaitForSeconds(0.5f);

                // Click the enemy
                battleUI.enemies.First(e => e.enemy.name == parsedData.target).Click();
            }

            battleUI.waitingForAction = false;
        }

        protected override ExecutionResult Validate(ActionJData actionData, out PlayCardActionData parsedData)
        {
            parsedData = null;
            int? nIndex = actionData?.Data?["index"]?.Value<int>();
            string target = actionData?.Data?["target"]?.Value<string>();

            // Check if the card index is null
            if(nIndex is null)
                return ExecutionResult.Failure("Action failed. Missing required parameter 'index'.");
            int index = (int) nIndex;

            // Check if the card index is valid
            if(index < 0 || index >= battleUI.handCards.Count)
                return ExecutionResult.Failure("Action failed. Parameter 'index' is out of range.");
            BattleCardUI card = battleUI.handCards[index];

            // Check if Neuro has enough MP to play that card
            if(card.card.fileSize > GameManager.Instance.mp)
                return ExecutionResult.Failure("Action failed. You don't have enough available memory to play this card.");

            // Check if Neuro tried to play a different card while a mutex is active
            if(battleUI.handCards.Any(c => c.card.cardEffects.Any(e => e is MutexEffect)) && !card.card.cardEffects.Any(e => e is MutexEffect))
                return ExecutionResult.Failure("Action failed. A Mutex card in your hand is preventing you from playing this card.");

            // Check if Neuro provided a target when no target is required
            if(!card.card.requiresTarget && target is not null)
                return ExecutionResult.Failure("Action failed. This card does not require a target, but the 'target' parameter was set.");

            // Check if Neuro tried to play a card that requires a target without providing a target
            if(card.card.requiresTarget && target is null)
                return ExecutionResult.Failure("Action failed. Playing this card requires setting the 'target' parameter.");

            // Check if the target exists
            if(target is not null && !battleUI.enemies.Any(e => e.enemy.name == target))
                return ExecutionResult.Failure("Action failed. The specified target does not exist.");

            parsedData = new PlayCardActionData
            {
                index = index,
                target = target
            };
            return ExecutionResult.Success();
        }
    }
}
