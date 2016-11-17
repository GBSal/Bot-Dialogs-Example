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
            PromptDialog.Confirm(
                context,
                AfterContinuePromptAsync,
                $"[{_thisDialogLabel}]: Do you want to go back down to to {_levelDownDialogLabel}?",
                $"[{_thisDialogLabel}]: Didn't get that! (were in {_thisDialogLabel})",
                promptStyle: PromptStyle.None);
        }

        public async Task AfterContinuePromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var goToNextDialogDown = await result;
            if (goToNextDialogDown)
            {
                context.ConversationData.Clear();
                await context.PostAsync($"[{_thisDialogLabel}]: Closing {_thisDialogLabel}");
                context.Done("");
            }
            else
            {
                await context.PostAsync($"[{_thisDialogLabel}]: OK, we'll stay in {_thisDialogLabel}. You can send any message to re-start {_thisDialogLabel}.");
                context.Wait(MessageReceivedAsync);
            }
        }
    }
}