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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArchiSteamFarm.Localization;
using ArchiSteamFarm.Steam;
using JetBrains.Annotations;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using SteamKit2;

namespace ArchiSteamFarm.NLog.Targets;

[Target("Steam")]
internal sealed class SteamTarget : AsyncTaskTarget {
	private const ushort DefaultBatchSize = 10;
	private const ushort DefaultFlushTimeoutSeconds = 30;

	// This is NLog config property, it must have public get() and set() capabilities
	[UsedImplicitly]
	public Layout? BotName { get; set; }

	// This is NLog config property, it must have public get() and set() capabilities
	[UsedImplicitly]
	public ulong ChatGroupID { get; set; }

	// This is NLog config property, it must have public get() and set() capabilities
	[UsedImplicitly]
	public ulong SteamID { get; set; }

	// This is NLog config property, it must have public get() and set() capabilities
	[UsedImplicitly]
	public ushort BatchSize { get; set; } = DefaultBatchSize;

	// This is NLog config property, it must have public get() and set() capabilities
	[UsedImplicitly]
	public ushort FlushTimeoutSeconds { get; set; } = DefaultFlushTimeoutSeconds;

	private readonly List<string> MessageBuffer = new();
	private readonly SemaphoreSlim BufferSemaphore = new(1, 1);
	private readonly CancellationTokenSource FlushCancellation = new();
	private DateTime LastFlushTime = DateTime.UtcNow;

	// This parameter-less constructor is intentionally public, as NLog uses it for creating targets
	// It must stay like this as we want to have our targets defined in our NLog.config
	// Keeping date in default layout also doesn't make much sense (Steam offers that), so we remove it by default
	[UsedImplicitly]
	public SteamTarget() {
		Layout = "${level:uppercase=true}|${logger}|${message}";
		StartFlushTimer();
	}

	protected override void InitializeTarget() {
		base.InitializeTarget();

		if ((SteamID == 0) || ((ChatGroupID == 0) && !new SteamID(SteamID).IsIndividualAccount)) {
			throw new NLogConfigurationException(Strings.FormatErrorIsInvalid(nameof(SteamID)));
		}

		if (BatchSize == 0) {
			BatchSize = DefaultBatchSize;
		}

		if (FlushTimeoutSeconds == 0) {
			FlushTimeoutSeconds = DefaultFlushTimeoutSeconds;
		}
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			FlushCancellation.Cancel();
			FlushCancellation.Dispose();
			BufferSemaphore.Dispose();
		}

		base.Dispose(disposing);
	}

	protected override async Task WriteAsyncTask(LogEventInfo logEvent, CancellationToken cancellationToken) {
		ArgumentNullException.ThrowIfNull(logEvent);

		Write(logEvent);

		if ((Bot.Bots == null) || Bot.Bots.IsEmpty) {
			return;
		}

		string message = Layout.Render(logEvent);

		if (string.IsNullOrEmpty(message)) {
			return;
		}

		await BufferSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

		try {
			MessageBuffer.Add(message);

			// Check if we should flush the buffer
			if (MessageBuffer.Count >= BatchSize) {
				await FlushBuffer().ConfigureAwait(false);
			}
		} finally {
			BufferSemaphore.Release();
		}
	}

	private async Task FlushBuffer() {
		if (MessageBuffer.Count == 0) {
			return;
		}

		StringBuilder batchedMessage = new();

		foreach (string msg in MessageBuffer) {
			if (batchedMessage.Length > 0) {
				batchedMessage.Append('\n');
			}

			batchedMessage.Append(msg);
		}

		MessageBuffer.Clear();
		LastFlushTime = DateTime.UtcNow;

		string fullMessage = batchedMessage.ToString();

		Bot? bot = null;

		if (!string.IsNullOrEmpty(BotName?.ToString())) {
			bot = Bot.GetBot(BotName.ToString());

			if (bot?.IsConnectedAndLoggedOn != true) {
				return;
			}
		}

		if (ChatGroupID != 0) {
			await SendGroupMessage(fullMessage, bot).ConfigureAwait(false);
		} else if ((bot == null) || (bot.SteamID != SteamID)) {
			await SendPrivateMessage(fullMessage, bot).ConfigureAwait(false);
		}
	}

	private async void StartFlushTimer() {
		while (!FlushCancellation.Token.IsCancellationRequested) {
			try {
				await Task.Delay(TimeSpan.FromSeconds(FlushTimeoutSeconds), FlushCancellation.Token).ConfigureAwait(false);

				await BufferSemaphore.WaitAsync(FlushCancellation.Token).ConfigureAwait(false);

				try {
					TimeSpan timeSinceLastFlush = DateTime.UtcNow - LastFlushTime;

					// Only flush if we have messages and enough time has passed
					if ((MessageBuffer.Count > 0) && (timeSinceLastFlush.TotalSeconds >= FlushTimeoutSeconds)) {
						await FlushBuffer().ConfigureAwait(false);
					}
				} finally {
					BufferSemaphore.Release();
				}
			} catch (OperationCanceledException) {
				// Expected when disposing
				break;
			}
		}
	}

	private async Task SendGroupMessage(string message, Bot? bot = null) {
		ArgumentException.ThrowIfNullOrEmpty(message);

		if (bot == null) {
			bot = Bot.Bots?.Values.FirstOrDefault(static targetBot => targetBot.IsConnectedAndLoggedOn);

			if (bot == null) {
				return;
			}
		}

		if (!await bot.SendMessage(ChatGroupID, SteamID, message).ConfigureAwait(false)) {
			bot.ArchiLogger.LogGenericTrace(Strings.FormatWarningFailedWithError(nameof(Bot.SendMessage)));
		}
	}

	private async Task SendPrivateMessage(string message, Bot? bot = null) {
		ArgumentException.ThrowIfNullOrEmpty(message);

		if (bot == null) {
			bot = Bot.Bots?.Values.FirstOrDefault(targetBot => targetBot.IsConnectedAndLoggedOn && (targetBot.SteamID != SteamID));

			if (bot == null) {
				return;
			}
		}

		if (!await bot.SendMessage(SteamID, message).ConfigureAwait(false)) {
			bot.ArchiLogger.LogGenericTrace(Strings.FormatWarningFailedWithError(nameof(Bot.SendMessage)));
		}
	}
}
