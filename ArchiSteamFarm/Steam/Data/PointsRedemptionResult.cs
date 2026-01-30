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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SteamKit2;

namespace ArchiSteamFarm.Steam.Data;

public sealed class PointsRedemptionResult {
	[Required]
	public uint DefinitionID { get; }

	[Required]
	public EResult Result { get; }

	[Required]
	public byte AttemptNumber { get; }

	[Required]
	public DateTime Timestamp { get; }

	public uint? PointsCost { get; }

	public PointsRedemptionResult(uint definitionID, EResult result, byte attemptNumber, uint? pointsCost = null) {
		ArgumentOutOfRangeException.ThrowIfZero(definitionID);

		if (!Enum.IsDefined(result)) {
			throw new InvalidEnumArgumentException(nameof(result), (int) result, typeof(EResult));
		}

		ArgumentOutOfRangeException.ThrowIfZero(attemptNumber);

		DefinitionID = definitionID;
		Result = result;
		AttemptNumber = attemptNumber;
		Timestamp = DateTime.UtcNow;
		PointsCost = pointsCost;
	}
}
