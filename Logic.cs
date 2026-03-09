using JHelper.Common.ProcessInterop;
using LiveSplit.Model;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.SonicFrontiers;

internal partial class SonicFrontiersComponent : LogicComponent
{
    /// <summary>
    /// Name of the component as it appears in the LiveSplit UI.
    /// </summary>
    public override string ComponentName => "Sonic X Shadow Generations - Autosplitter";

    private readonly Task autosplitterTask;
    private readonly CancellationTokenSource cancelToken = new();

    /// <summary>
    /// Main task method for the autosplitter that monitors the game process and updates the LiveSplit timer based on in-game events.
    /// </summary>
    /// <param name="state">Current state of the LiveSplit timer.</param>
    /// <param name="canceltoken">Token for cancelling the task.</param>
    private void AutosplitterLogic(LiveSplitState state, CancellationToken canceltoken)
    {
        // Array of target game process names to hook into
        string[] gameProcessNames = ["SonicFrontiers.exe"];

        // Interval for autosplitter updates (about 60 times per second)
        TimeSpan updateInterval = TimeSpan.FromMilliseconds(1000d / 60d);

        // Timer model connected to LiveSplit state, allowing timer control
        TimerModel timer = new() { CurrentState = state };

        // Stopwatch to track time for each update cycle
        Stopwatch clock = new();

        // Main loop that will continue running until cancellation is requested
        while (!canceltoken.IsCancellationRequested)
        {
            // Try to hook into the game process
            using ProcessMemory? process = ProcessHook(gameProcessNames);

            // If the process is not available, wait and retry
            if (process is null)
            {
                Task.Delay(1500, canceltoken).Wait(canceltoken);
                continue;
            }

            // Attempt to locate necessary memory addresses in the hooked process
            // This loop will retry if memory scanning fails, catching any exceptions
            Memory? memory = null;
            while (!canceltoken.IsCancellationRequested && process.IsOpen && memory is null)
            {
                try
                {
                    memory = new Memory();
                }
                catch
                {
                    Task.Delay(1500, canceltoken).Wait(canceltoken);
                }
            }

            // If memory is still null, exit the loop to retry from the start
            if (memory is null)
                continue;

            // Primary loop for updating and checking game memory while the process remains open
            while (!canceltoken.IsCancellationRequested && process.IsOpen)
            {
                clock.Start();

                // Memory update cycle for the current game process and settings
                memory.Update(process, Settings);

                // Timer logic to manage game-time, loading status, and potential resets or splits
                if (timer.CurrentState.CurrentPhase == TimerPhase.Running || timer.CurrentState.CurrentPhase == TimerPhase.Paused)
                {
                    // Check if the game is in a loading state and pause timer if so
                    bool? isLoading = memory.IsLoading(Settings);
                    if (isLoading is not null)
                        state.IsGameTimePaused = isLoading.Value;

                    // Update in-game time if available
                    TimeSpan? gameTime = memory.GameTime(Settings);
                    if (gameTime is not null)
                        timer.CurrentState.SetGameTime(gameTime.Value);

                    // Check if the game conditions require a timer reset or split
                    if (memory.Reset(Settings))
                        timer.Reset();
                    else if (memory.Split(Settings))
                        timer.Split();
                }

                // Start the timer if game start conditions are met and the timer is not running
                if (timer.CurrentState.CurrentPhase == TimerPhase.NotRunning && memory.Start(Settings))
                {
                    timer.Start();
                    state.IsGameTimePaused = true;

                    // Re-check if the game is loading to update the paused status
                    bool? isLoading = memory.IsLoading(Settings);
                    if (isLoading is not null)
                        state.IsGameTimePaused = isLoading.Value;
                }

                // Calculate elapsed time for this update cycle
                TimeSpan elapsed = clock.Elapsed;
                clock.Reset();

                // Wait if the cycle completed faster than the desired update interval
                if (elapsed < updateInterval)
                    canceltoken.WaitHandle.WaitOne(updateInterval - elapsed);
            }

        }
    }

    /// <summary>
    /// Attempts to hook into one of the target game processes specified in <paramref name="exeNames"/>.
    /// </summary>
    /// <param name="exeNames">Array of executable names to try hooking into.</param>
    /// <returns>A <see cref="ProcessMemory"/> object representing the hooked process, or null if no process could be hooked.</returns>
    private static ProcessMemory? ProcessHook(string[] exeNames)
    {
        try
        {
            // Attempt to hook into each process in exeNames and return the first successful match
            return exeNames
                .Select(ProcessMemory.HookProcess)
                .FirstOrDefault(p => p is not null);
        }
        catch
        {
            // Return null if any errors occur during hooking
            return null;
        }
    }
}
