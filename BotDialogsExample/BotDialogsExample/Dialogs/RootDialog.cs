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
        private const string _levelUpDialogLabel = "Dialog1";
        private const string _levelDownDialogLabel = "";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            PromptDialog.Confirm(
                context,
                AfterContinuePromptAsync,
                $"[{_thisDialogLabel}]: Do you want to continue to {_levelUpDialogLabel}?",
                $"[{_thisDialogLabel}]: Didn't get that! (were in {_thisDialogLabel})",
                promptStyle: PromptStyle.None);
        }

        public async Task AfterContinuePromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var goToNextDialog = await result;
            if (goToNextDialog)
            {
                await context.PostAsync($"[{_thisDialogLabel}]: Passing to {_levelUpDialogLabel}");
                await context.Forward(new Dialog1(), ResumeAfterDialog, result, CancellationToken.None);
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
            await context.PostAsync($"[{_thisDialogLabel}]: We've now completed {_levelUpDialogLabel} and are in the ResumeAfterDialog method of {_thisDialogLabel}. You can send any message to re-start {_thisDialogLabel}.");
            context.Wait(MessageReceivedAsync);
        }

    }
}