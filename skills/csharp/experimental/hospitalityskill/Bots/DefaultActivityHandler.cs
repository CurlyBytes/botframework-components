﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HospitalitySkill.Responses.Main;
using HospitalitySkill.Utilities;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Solutions.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace HospitalitySkill.Bots
{
    public class DefaultActivityHandler<T> : TeamsActivityHandler
        where T : Dialog
    {
        private readonly Dialog _dialog;
        private readonly BotState _conversationState;
        private readonly BotState _userState;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateAccessor;
        private readonly LocaleTemplateManager _templateManager;

        public DefaultActivityHandler(IServiceProvider serviceProvider, T dialog)
        {
            _dialog = dialog;
            _dialog.TelemetryClient = serviceProvider.GetService<IBotTelemetryClient>();
            _conversationState = serviceProvider.GetService<ConversationState>();
            _userState = serviceProvider.GetService<UserState>();
            _dialogStateAccessor = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
            _templateManager = serviceProvider.GetService<LocaleTemplateManager>();
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Type == ActivityTypes.Message && turnContext.Activity.Text == null)
            {
                turnContext.Activity.Text = string.Empty;
            }

            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(_templateManager.GenerateActivity(MainResponses.WelcomeMessage), cancellationToken);
            await _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }

        protected override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // directline speech occasionally sends empty message activities that should be ignored
            var activity = turnContext.Activity;
            if (activity.ChannelId == Channels.DirectlineSpeech && activity.Type == ActivityTypes.Message && string.IsNullOrEmpty(activity.Text))
            {
                return Task.CompletedTask;
            }

            return _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }

        protected override Task OnTeamsSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            return _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }

        protected override Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            return _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }

        protected override Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            return _dialog.RunAsync(turnContext, _dialogStateAccessor, cancellationToken);
        }
    }
}
