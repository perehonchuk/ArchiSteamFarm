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

using System.Collections.Generic;
using JetBrains.Annotations;

namespace ArchiSteamFarm.Plugins.Interfaces;

/// <summary>
///     Implementing this interface allows your plugin to declare its loading priority and dependencies on other plugins.
///     ASF will ensure plugins are loaded in the correct order based on their dependencies and priority levels.
/// </summary>
[PublicAPI]
public interface IPluginLoadOrder {
	/// <summary>
	///     Loading priority for this plugin. Lower values load first, higher values load later.
	///     Default priority is 100. System/core plugins should use 0-50, normal plugins 51-150, and addon plugins 151-255.
	/// </summary>
	/// <returns>Priority value between 0 and 255.</returns>
	public byte LoadPriority => 100;

	/// <summary>
	///     Names of other plugins that this plugin depends on. These plugins will be loaded before this plugin.
	///     Dependencies are resolved recursively. If a dependency cannot be found or a circular dependency is detected,
	///     the plugin will fail to load.
	/// </summary>
	/// <returns>Collection of plugin names this plugin depends on, or null/empty if no dependencies.</returns>
	public IReadOnlyCollection<string>? PluginDependencies => null;
}
