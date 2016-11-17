using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BotDialogsExample.Dialogs
{
    [LuisModel("6bae5450-214a-42bc-90c2-c1bc228558ea", "d004b0b064694dd1bec537e3629863fb")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        private const string _thisDialogLabel = "RootLuisDialog";
        private string _levelUpDialogLabel = "";

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await base.MessageReceived(context, result);
        }

        [LuisIntent("None")]
        public async Task NoneAsync(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"[{_thisDialogLabel}]: You did not give a recognised path choice. Type 'Numbers' or 'Letters' to re-start {_thisDialogLabel}.");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Numbers")]
        public async Task NumbersAsync(IDialogContext context, LuisResult result)
        {
            _levelUpDialogLabel = "Dialog1";
            await context.PostAsync($"[{_thisDialogLabel}]: We think you chose numbers. Forwarding context on to {_levelUpDialogLabel}.");
            await context.Forward(new Dialog1(), ResumeAfterDialog, result, CancellationToken.None);
        }

        [LuisIntent("Letters")]
        public async Task LettersAsync(IDialogContext context, LuisResult result)
        {
            _levelUpDialogLabel = "DialogA";
            await context.PostAsync($"[{_thisDialogLabel}]: We think you chose letters. Forwarding context on to {_levelUpDialogLabel}.");
            await context.Forward(new DialogA(), ResumeAfterDialog, result, CancellationToken.None);
        }

        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"[{_thisDialogLabel}]: We've now completed {_levelUpDialogLabel} we're in ResumeAfterDialog for {_thisDialogLabel}. Send any message to re-start {_thisDialogLabel}.");
            context.Wait(MessageReceived);
        }
    }
}