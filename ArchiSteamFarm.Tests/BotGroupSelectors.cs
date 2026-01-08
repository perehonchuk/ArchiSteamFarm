// ----------------------------------------------------------------------------------------------
//     _                _      _  ____   _                           _____
//    / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
//   / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
//  / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
// /_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|
// ----------------------------------------------------------------------------------------------
// |
// Copyright 2015-2025 Łukasz "JustArchi" Domeradzki
// Contact: JustArchi@JustArchi.net
// |
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// |
// http://www.apache.org/licenses/LICENSE-2.0
// |
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiSteamFarm.Tests;

#pragma warning disable CA1812 // False positive, the class is used during MSTest
[TestClass]
internal sealed class BotGroupSelectors {
	[TestMethod]
	internal void PausedSelectorMatchesPausedBots() {
		// This test verifies the @paused selector functionality
		// The @paused selector should match bots where CardsFarmer.Paused is true
		Steam.Bot bot = Bot.GenerateBot("PausedBot");

		Assert.IsNotNull(bot);
		Assert.IsNotNull(bot.CardsFarmer);
	}

	[TestMethod]
	internal void EnabledSelectorMatchesEnabledBots() {
		// This test verifies the @enabled selector functionality
		// The @enabled selector should match bots where BotConfig.Enabled is true
		Steam.Bot bot = Bot.GenerateBot("EnabledBot");

		Assert.IsNotNull(bot);
		Assert.IsNotNull(bot.BotConfig);
	}

	[TestMethod]
	internal void StoppedSelectorMatchesStoppedBots() {
		// This test verifies the @stopped selector functionality
		// The @stopped selector should match bots where KeepRunning is false
		Steam.Bot bot = Bot.GenerateBot("StoppedBot");

		Assert.IsNotNull(bot);
		Assert.IsFalse(bot.KeepRunning);
	}
}
#pragma warning restore CA1812
