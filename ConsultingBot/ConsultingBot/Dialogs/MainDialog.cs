// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio CoreBot v4.3.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace ConsultingBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogger Logger;

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new BookingDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                DispatchStepAsync,
                //ActStepAsync,
                //FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> DispatchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(Configuration["LuisAppId"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file."), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }
            else
            {
                //var message = "You are not in a Team";
                //var channelData = stepContext.Context.Activity.GetChannelData<TeamsChannelData>();
                //if (channelData != null && channelData.Channel != null && channelData.Channel.Id != null)
                //{
                //    message = $"You are in channel {channelData.Channel.Id}";
                //}

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Echo: {stepContext.Context.Activity.Text}"), cancellationToken);

                //        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What can I help you with today?\nSay something like \"Book a flight from Paris to Berlin on March 22, 2020\"") }, cancellationToken);
                //    }
                //}

                //private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
                //{
                // Call LUIS and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
                var projectIntentDetails = stepContext.Context.Activity.Text != null
                        ?
                    await LuisConsultingProjectRecognizer.ExecuteQuery(Configuration, Logger, stepContext.Context, cancellationToken)
                        :
                    new ProjectIntentDetails();

                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Got intent of {projectIntentDetails.intent}\nProject: {projectIntentDetails.projectName}\nPerson: {projectIntentDetails.personName}\nMinutes: {projectIntentDetails.taskDurationMinutes}\nWhen: {projectIntentDetails.deliveryDate}"), cancellationToken);

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
                //var bookingDetails = stepContext.Result != null
                //    ?
                //await LuisBookingRecognizer.ExecuteQuery(Configuration, Logger, stepContext.Context, cancellationToken)
                //    :
                //new BookingDetails();

                // In this sample we only have a single Intent we are concerned with. However, typically a scenario
                // will have multiple different Intents each corresponding to starting a different child Dialog.

                // Run the BookingDialog giving it whatever details we have from the LUIS call, it will fill out the remainder.
                //return await stepContext.BeginDialogAsync(nameof(BookingDialog), bookingDetails, cancellationToken);
        }

        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    // If the child dialog ("BookingDialog") was cancelled or the user failed to confirm, the Result here will be null.
        //    if (stepContext.Result != null)
        //    {
        //        var result = (BookingDetails)stepContext.Result;

        //        // Now we have all the booking details call the booking service.

        //        // If the call to the booking service was successful tell the user.

        //        var timeProperty = new TimexProperty(result.TravelDate);
        //        var travelDateMsg = timeProperty.ToNaturalLanguage(DateTime.Now);
        //        var msg = $"I have you booked to {result.Destination} from {result.Origin} on {travelDateMsg}";
        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);
        //    }
        //    else
        //    {
        //        await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you."), cancellationToken);
        //    }
        //    return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        //}
    }
}
