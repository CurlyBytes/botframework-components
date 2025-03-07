// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

'use strict';

const {
  BaseGenerator,
  platforms,
} = require('@microsoft/generator-bot-adaptive');

const packageReferences = {
  [platforms.dotnet]: [
    {
      name: 'Microsoft.Bot.Components.HelpAndCancel',
      version: '1.0.0-preview.20210324.6dfb4a1',
    },
  ],
  [platforms.js]: [
    { name: '@microsoft/bot-components-helpandcancel', version: 'latest' },
  ],
};

module.exports = class extends BaseGenerator {
  initializing() {
    this.composeWith(
      require.resolve('@microsoft/generator-bot-adaptive/generators/app'),
      Object.assign(this.options, {
        arguments: this.args,
        applicationSettingsDirectory: 'settings',
        packageReferences: packageReferences[this.options.platform],
      })
    );
  }

  writing() {
    this._copyBotTemplateFiles();
  }
};
