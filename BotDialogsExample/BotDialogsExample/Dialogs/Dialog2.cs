using BotDialogsExample.Enums;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotDialogsExample.Dialogs
{
    [Serializable]
    public class Dialog2 : IDialog<object>
    {
        private const string _thisDialogLabel = "Dialog2";
        private const string _levelUpDialogLabel = "";
        private const string _levelDownDialogLabel = "Dialog1";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(
               context: context,
               resume: AfterChoicePromptAsync,
               options: Enum.GetValues(typeof(Options.NavigationDirectionOptions)).Cast<Options.NavigationDirectionOptions>().ToArray(),
               prompt: $"[{_thisDialogLabel}]: Which direction do you want to go?",
               retry: $"[{_thisDialogLabel}]: I didn't understand. Please try again.");
        }

        public async Task AfterChoicePromptAsync(IDialogContext context, IAwaitable<Options.NavigationDirectionOptions> result)
        {
            var pathChoice = await result;

            switch (pathChoice)
            {
                case Options.NavigationDirectionOptions.Up:
                    await context.PostAsync($"[{_thisDialogLabel}]: We can't go further Up. Send any message to re-start {_thisDialogLabel}.");
                    context.Wait(MessageReceivedAsync);
                    break;
                case Options.NavigationDirectionOptions.Down:
                    context.ConversationData.Clear();
                    await context.PostAsync($"[{_thisDialogLabel}]: Closing {_thisDialogLabel}");
                    context.Done("");
                    break;
                default:
                    await context.PostAsync($"[{_thisDialogLabel}]: You did not give a choice. Send any message to re-start {_thisDialogLabel}.");
                    context.Wait(MessageReceivedAsync);
                    break;
            }
        }
    }
}