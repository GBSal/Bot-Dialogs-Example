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
    public class RootDialog : IDialog<object>
    {
        private const string _thisDialogLabel = "RootDialog";
        private string _levelUpDialogLabel = "";

        public enum DialogPathOptions
        {
            Numbers,
            Letters
        }

        public async Task StartAsync(IDialogContext context)
        {

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Choice(
                           context: context,
                           resume: AfterChoicePromptAsync,
                           options: Enum.GetValues(typeof(DialogPathOptions)).Cast<DialogPathOptions>().ToArray(),
                           prompt: "Which path do you want to take:",
                           retry: "I didn't understand. Please try again.");
        }

        public async Task AfterChoicePromptAsync(IDialogContext context, IAwaitable<DialogPathOptions> result)
        {
            var pathChoice = await result;

            switch (pathChoice)
            {
                case DialogPathOptions.Letters:
                    _levelUpDialogLabel = "DialogA";
                    await context.PostAsync($"[{_thisDialogLabel}]: You chose {pathChoice}. Forwarding context on to {_levelUpDialogLabel}.");
                    await context.Forward(new DialogA(), ResumeAfterDialog, result, CancellationToken.None);
                    break;
                case DialogPathOptions.Numbers:
                    _levelUpDialogLabel = "Dialog1";
                    await context.PostAsync($"[{_thisDialogLabel}]: You chose {pathChoice}. Forwarding context on to {_levelUpDialogLabel}.");
                    await context.Forward(new Dialog1(), ResumeAfterDialog, result, CancellationToken.None);
                    break;
                default:
                    await context.PostAsync($"[{_thisDialogLabel}]: You did not give a recognised path choice. Send any message to re-start {_thisDialogLabel}.");
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