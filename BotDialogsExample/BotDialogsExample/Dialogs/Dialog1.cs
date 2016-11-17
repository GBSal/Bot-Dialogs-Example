using BotDialogsExample.Enums;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BotDialogsExample.Dialogs
{
    [Serializable]
    public class Dialog1 : IDialog<object>
    {
        private const string _thisDialogLabel = "Dialog1";
        private const string _levelUpDialogLabel = "Dialog2";
        private const string _levelDownDialogLabel = "RootDialog";

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
                    await context.PostAsync($"[{_thisDialogLabel}]: Passing to {_levelUpDialogLabel}");
                    await context.Forward(new Dialog2(), ResumeAfterDialog, result, CancellationToken.None);
                    break;
                case Options.NavigationDirectionOptions.Down:
                    context.ConversationData.Clear();
                    await context.PostAsync($"[{_thisDialogLabel}]: Closing {_thisDialogLabel}");
                    context.Done("");
                    break;
                default:
                    await context.PostAsync($"[{_thisDialogLabel}]: You did not give a choice. Send any message to re-start {_thisDialogLabel}.");
                    context.Done("");
                    break;
            }
        }

        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"[{_thisDialogLabel}]: We've now completed {_levelUpDialogLabel} we're in ResumeAfterDialog for {_thisDialogLabel}. Send any message to re-start {_thisDialogLabel}.");
            context.Wait(MessageReceivedAsync);
        }
    }
}