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

using JetBrains.Annotations;

namespace ArchiSteamFarm.Plugins.Interfaces;

/// <summary>
///     Implementing this interface allows plugin to specify its loading phase and priority.
///     Plugins are loaded in phases (Early, Normal, Late) and within each phase by priority value.
/// </summary>
[PublicAPI]
public interface IPluginLoadPriority {
	/// <summary>
	///     Defines the loading phase for this plugin.
	///     Early phase plugins load first (useful for infrastructure plugins).
	///     Normal phase plugins load second (default for most plugins).
	///     Late phase plugins load last (useful for plugins that depend on others).
	/// </summary>
	public ELoadPhase LoadPhase { get; }

	/// <summary>
	///     Defines the priority within the loading phase (lower values load first).
	///     Default priority is 100. Range is 0-255.
	/// </summary>
	public byte Priority { get; }

	/// <summary>
	///     Enum defining plugin loading phases.
	/// </summary>
	public enum ELoadPhase : byte {
		/// <summary>
		///     Loads before other plugins - for infrastructure and foundational plugins.
		/// </summary>
		Early = 0,

		/// <summary>
		///     Default loading phase - for standard plugins.
		/// </summary>
		Normal = 1,

		/// <summary>
		///     Loads after other plugins - for plugins with dependencies on other plugins.
		/// </summary>
		Late = 2
	}
}
