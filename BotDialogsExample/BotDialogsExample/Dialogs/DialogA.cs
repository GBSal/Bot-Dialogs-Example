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
    public class DialogA : IDialog<object>
    {
        private const string _thisDialogLabel = "DialogA";
        private const string _levelUpDialogLabel = "DialogB";
        private const string _levelDownDialogLabel = "RootDialog";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Confirm(
                context: context,
                resume: AfterContinuePromptAsync,
                prompt: $"[{_thisDialogLabel}]: Do you want to continue to {_levelUpDialogLabel}?",
                retry: $"[{_thisDialogLabel}]: Didn't get that! (were in {_thisDialogLabel})",
                promptStyle: PromptStyle.None);
        }

        public async Task AfterContinuePromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var goToNextDialog = await result;
            if (goToNextDialog)
            {
                await context.PostAsync($"[{_thisDialogLabel}]: Passing to {_levelUpDialogLabel}");
                await context.Forward(new DialogB(), ResumeAfterDialog, result, CancellationToken.None);
            }
            else
            {
                context.ConversationData.Clear();
                await context.PostAsync($"[{_thisDialogLabel}]: Closing {_thisDialogLabel}");
                context.Done("");
            }
        }

        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"[{_thisDialogLabel}]: We've now completed {_levelUpDialogLabel} we're in ResumeAfterDialog for {_thisDialogLabel}. Send any message to re-start {_thisDialogLabel}.");
            context.Wait(MessageReceivedAsync);
        }
    }
}