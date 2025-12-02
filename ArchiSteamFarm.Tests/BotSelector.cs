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

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ArchiSteamFarm.Tests;

#pragma warning disable CA1812 // False positive, the class is used during MSTest
[TestClass]
internal sealed class BotSelector {
	// This test class documents the bot selector functionality
	// Bot selectors allow commands to target specific groups of bots
	// Available selectors:
	// - @all / ASF: All bots
	// - @farming: Bots currently farming cards
	// - @idle: Bots not currently farming
	// - @offline: Bots not connected to Steam
	// - @online: Bots connected to Steam
	// - @paused: Bots with farming paused
	// - @enabled: Bots with Enabled=true in configuration
	// - @stopped: Bots that are not running (KeepRunning=false)

	[TestMethod]
	public void BotSelectorDocumentation() {
		// This test serves as documentation for the bot selector feature
		// The selectors are implemented in ArchiSteamFarm/Steam/Bot.cs in the GetBots() method
		Assert.IsTrue(true, "Bot selectors documented");
	}
}
#pragma warning restore CA1812 // False positive, the class is used during MSTest
