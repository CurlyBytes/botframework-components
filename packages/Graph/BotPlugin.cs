// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Components.Graph
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Runtime.Plugins;

    public class BotPlugin : IBotPlugin
    {
        /// <inheritdoc/>
        public void Load(IBotPluginLoadContext context)
        {
            ComponentRegistration.Add(new GraphComponentRegistration());
        }
    }
}