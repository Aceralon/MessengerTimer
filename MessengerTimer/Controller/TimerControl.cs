﻿using MessengerTimer.DataModels;
using System;
using System.Threading;
using System.Timers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MessengerTimer {
    public sealed partial class MainPage : Page {
        private void TimerControlSpaceKeyDown(CoreWindow sender, KeyEventArgs args) {
            if (args.VirtualKey == VirtualKey.Space) {
                switch (TimerStatus) {
                    case TimerStatus.Waiting:
                        if (!appSettings.NeedObserving)
                            StartHoldingTick();
                        else
                            TimerTextBlock.Foreground = YellowBrush;
                        break;
                    case TimerStatus.Observing:
                        StartHoldingTick();
                        break;
                    case TimerStatus.Timing:
                        TimerStatus = TimerStatus.Display;
                        StopTimer();

                        TimerTextBlock.Foreground = RedBrush;
                        break;
                    case TimerStatus.Display:
                        TimerTextBlock.Foreground = RedBrush;
                        break;
                    default:
                        if (!(TimerStatus == TimerStatus.Holding))
                            TimerTextBlock.Foreground = YellowBrush;
                        break;
                }
                滴汤Button.Focus(FocusState.Keyboard);
            }
        }

        private void TimerControlSpaceKeyUp(CoreWindow sender, KeyEventArgs args) {
            if (args.VirtualKey == VirtualKey.Space) {
                switch (TimerStatus) {
                    case TimerStatus.Waiting:
                        if (appSettings.NeedObserving) {
                            TimerStatus = TimerStatus.Observing;

                            StartTime = DateTime.Now;
                            RefreshTimeTimer.Start();
                        }
                        else
                            IsHolding = false;
                        break;
                    case TimerStatus.Holding:
                        TimerStatus = TimerStatus.Timing;

                        RefreshTimeTimer.Stop();
                        CurrentResultPunishment = GetCurrentPunishment();

                        StartTimer();
                        break;
                    case TimerStatus.Observing:
                        IsHolding = false;
                        break;
                    case TimerStatus.Display:
                        TimerStatus = TimerStatus.Waiting;
                        break;
                    default:
                        break;
                }
                TimerTextBlock.Foreground = BlackBrush;
            }
        }

        private Punishment GetCurrentPunishment() {
            if (TimerTextBlock.Text == "DNF")
                return Punishment.DNF;
            if (TimerTextBlock.Text == "+2")
                return Punishment.PlusTwo;
            return Punishment.None;
        }

        private void RefreshTimeTimer_Tick(object sender, object e) {
            EndTime = DateTime.Now;
            if (TimerStatus == TimerStatus.Observing || TimerStatus == TimerStatus.Holding) {
                switch (appSettings.DisplayMode) {
                    case DisplayModeEnum.Hidden:
                        DisplayTime(TimerStatus.ToString());
                        break;
                    default:
                        var timeLeft = 15 - ((int)(EndTime - StartTime).TotalSeconds);
                        if (timeLeft <= -2)
                            DisplayTime("DNF");
                        else if (timeLeft <= 0)
                            DisplayTime("+2");
                        else
                            DisplayTime(timeLeft.ToString());
                        break;
                }
            }
            else if (TimerStatus == TimerStatus.Timing) {
                switch (appSettings.DisplayMode) {
                    case DisplayModeEnum.RealTime:
                        DisplayTime(Result.GetFormattedString((EndTime - StartTime).TotalSeconds));
                        break;
                    case DisplayModeEnum.ToSecond:
                        DisplayTime(((int)(EndTime - StartTime).TotalSeconds).ToString());
                        break;
                    default:
                        DisplayTime("Solving");
                        break;
                }
            }
        }

        private void HoldingCheck() {
            if (IsHolding) {
                TimerStatus = TimerStatus.Holding;
                TimerTextBlock.Foreground = GreenBrush;
            }
            IsHolding = false;
        }

        private void DisplayTime(string time) => TimerTextBlock.Text = time;

        private void DisplayTime(Result result) => TimerTextBlock.Text = result.ResultString;

        private void StopTimer() {
            EndTime = DateTime.Now;
            RefreshTimeTimer.Stop();

            Result result = new Result(
                (EndTime - StartTime).TotalSeconds, Results.Count + 2,
                appSettings.NeedObserving ? CurrentResultPunishment : Punishment.None,
                ScrambleTextBlock.Text);

            DisplayTime(result);
            UpdateResult(result);

            NextScramble(true);
        }

        private void StartHoldingTick() {
            if (appSettings.StartDelay > 0) {
                IsHolding = true;
                TimerTextBlock.Foreground = YellowBrush;

                var holdingStartDateTime = DateTime.Now;
                new Thread(async () => {
                    while ((DateTime.Now - holdingStartDateTime).Ticks < appSettings.StartDelay)
                        Thread.Sleep(50);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => { HoldingCheck(); });
                }).Start();
            }
            else {
                IsHolding = true;
                HoldingCheck();
            }
        }

        private void StartTimer() {
            StartTime = DateTime.Now;
            RefreshTimeTimer.Start();
        }
    }
}
