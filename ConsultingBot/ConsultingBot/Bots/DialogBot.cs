// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConsultingBot.TeamsActivityHandlers;
using ConsultingBot.TeamsManifest;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.MessagingExtensionBot.Engine;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsultingBot.Bots
{
    public class DialogBot<T> : TeamsActivityHandler where T : Dialog
    {
        protected readonly BotState conversationState;
        protected readonly BotState userState;
        protected readonly Dialog dialog;
        protected readonly ILogger logger;
        protected readonly ProjectMessagingExtension projectMessagingExtension;
        protected readonly TestMessagingExtension testMessagingExtension;

        public DialogBot(
            ProjectMessagingExtension projectMessagingExtension,
            TestMessagingExtension testMessagingExtension,
            ConversationState conversationState,
            UserState userState,
            T dialog,
            ILogger<DialogBot<T>> logger)
        {
            this.projectMessagingExtension = projectMessagingExtension;
            this.testMessagingExtension = testMessagingExtension;

            this.conversationState = conversationState;
            this.userState = userState;
            this.dialog = dialog;
            this.logger = logger;
        }

    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
    {
        await base.OnTurnAsync(turnContext, cancellationToken);

        // Save any state changes that might have occured during the turn.
        await conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        await userState.SaveChangesAsync(turnContext, false, cancellationToken);
    }


    protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await dialog.Run(turnContext, conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }

        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            if (query.CommandId == ManifestConstants.ComposeExtensions.ProjectQuery.Id)
            {
                return await this.projectMessagingExtension.HandleMessagingExtensionQueryAsync(turnContext, query);
            }
            else
            {
                return await this.testMessagingExtension.HandleMessagingExtensionQueryAsync(turnContext, query);
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            if (action.CommandId == ManifestConstants.ComposeExtensions.SampleCard.Id)
            {
                return await this.projectMessagingExtension.HandleMessagingExtensionFetchTaskAsync(turnContext, action);
            }
            else
            {
                return await this.testMessagingExtension.HandleMessagingExtensionFetchTaskAsync(turnContext, action);
            }
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            if (action.CommandId == ManifestConstants.ComposeExtensions.SampleCard.Id)
            {
                return await this.projectMessagingExtension.HandleMessagingExtensionSubmitActionAsync(turnContext, action);
            }
            else
            {
                return await this.testMessagingExtension.HandleMessagingExtensionSubmitActionAsync(turnContext, action);
            }
        }

        protected override async Task<MessagingExtensionResponse> OnTeamsAppBasedLinkQueryAsync(ITurnContext<IInvokeActivity> turnContext, AppBasedLinkQuery query, CancellationToken cancellationToken)
        {
            var projectLinkQuery = new TestLinkQuery();
            return await projectLinkQuery.HandleInvokeActivityAsync(turnContext, query).ConfigureAwait(false);
        }

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var taskModule = new TestTaskModule();
            return await taskModule.HandleTaskModuleFetchAsync(turnContext, taskModuleRequest);
        }

        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            var taskModule = new TestTaskModule();
            return await taskModule.HandleTaskModuleSubmitAsync(turnContext, taskModuleRequest);
        }

        protected override Task<InvokeResponse> OnTeamsCardActionInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return base.OnTeamsCardActionInvokeAsync(turnContext, cancellationToken);
        }

    }
}
