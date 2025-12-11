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

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace ArchiSteamFarm.Steam.Data;

[PublicAPI]
[SuppressMessage("ReSharper", "ClassCannotBeInstantiated")]
public sealed class Confirmation {
	[JsonInclude]
	[JsonPropertyName("type")]
	[JsonRequired]
	public EConfirmationType ConfirmationType { get; private init; }

	[JsonInclude]
	[JsonPropertyName("type_name")]
	public string? ConfirmationTypeName { get; private init; }

	[JsonInclude]
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
	[JsonPropertyName("creator_id")]
	[JsonRequired]
	public ulong CreatorID { get; private init; }

	[JsonInclude]
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
	[JsonPropertyName("id")]
	[JsonRequired]
	public ulong ID { get; private init; }

	[JsonInclude]
	[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
	[JsonPropertyName("nonce")]
	[JsonRequired]
	internal ulong Nonce { get; private init; }

	[JsonConstructor]
	private Confirmation() { }

	[UsedImplicitly]
	public static bool ShouldSerializeNonce() => false;

	[PublicAPI]
	public enum EConfirmationType : byte {
		Unknown,
		Generic,
		Trade,
		Market,
		PhoneNumberChange = 5,
		AccountRecovery = 6,
		ApiKeyRegistration = 9,
		FamilyJoin = 11,
		AccountSecurity = 12
	}

	internal static byte GetConfirmationTypePriority(EConfirmationType confirmationType) {
		// Lower value = higher priority
		// Critical security confirmations should be processed first
		return confirmationType switch {
			EConfirmationType.AccountSecurity => 1,  // Highest priority - account security changes
			EConfirmationType.AccountRecovery => 2,  // High priority - account recovery
			EConfirmationType.PhoneNumberChange => 3, // High priority - phone changes
			EConfirmationType.ApiKeyRegistration => 4, // Medium-high priority - API access
			EConfirmationType.FamilyJoin => 5,       // Medium priority - family sharing
			EConfirmationType.Trade => 10,           // Normal priority - trades
			EConfirmationType.Market => 11,          // Normal priority - market transactions
			EConfirmationType.Generic => 20,         // Low priority - generic confirmations
			EConfirmationType.Unknown => 99,         // Lowest priority - unknown types
			_ => 50                                  // Default medium-low priority
		};
	}
}
