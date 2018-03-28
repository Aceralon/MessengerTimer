﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MessengerTimer.DataModels {
    public enum InputControlTypes { ToggleSwitch, Slider, ComboBox }
    public class SettingItem {
        //Timing: NeedObserving, StartDelay, DisplayMode
        //Scramble 
        //Statistics 
        //UI: TimerFormat, ShowScambleState

        //ToggleSwitch Slider ComboBox 
        private static AppSettings appSettings = App.MainPageInstance.appSettings;

        private static List<string> displayModeEnums = new List<string>
        { DisplayModeEnum.RealTime.ToString(), DisplayModeEnum.ToSecond.ToString(), DisplayModeEnum.OnlyOberving.ToString(), DisplayModeEnum.Hidden.ToString() };

        private static List<string> timerFormats = new List<string>
        { TimerFormat.MMSSFF.ToString(), TimerFormat.MMSSFFF.ToString(), TimerFormat.SSFF.ToString(), TimerFormat.SSFFF.ToString() };

        private static InputControlTypes[] ICTs = new InputControlTypes[] { InputControlTypes.ToggleSwitch, InputControlTypes.Slider, InputControlTypes.ComboBox };
        public string Title { get; set; }
        public Dictionary<InputControlTypes, Visibility> InputControlVisibility { get; set; }

        public bool IsToggleSwitchOn {
            get {
                switch (Title) {
                    case "NeedObserving: ":
                        return appSettings.NeedObserving;
                    case "ShowScambleState: ":
                        return appSettings.ShowScrambleState;
                    default:
                        return false;
                }
            }
            set {
                switch (Title) {
                    case "NeedObserving: ":
                        appSettings.NeedObserving = value;
                        break;
                    case "ShowScambleState: ":
                        appSettings.ShowScrambleState = value;
                        break;
                    default:
                        break;
                }
            }
        }

        public double SliderValue {
            get {
                switch (Title) {
                    case "StartDelay: ":
                        return appSettings.StartDelay / 10000000.0;
                    default:
                        return 0;
                }
            }
            set {
                switch (Title) {
                    case "StartDelay: ":
                        appSettings.StartDelay = (long)(value * 10000000);
                        break;
                    default:
                        break;
                }
            }
        }

        public object ComboBoxItemSource {
            get {
                switch (Title) {
                    case "DisplayMode: ":
                        return displayModeEnums;
                    case "TimerFormat: ":
                        return timerFormats;
                    default:
                        return null;
                }
            }
        }

        public int ComboBoxSelectedIndex {
            get {
                switch (Title) {
                    case "DisplayMode: ":
                        return displayModeEnums.IndexOf(appSettings.DisplayMode.ToString());
                    case "TimerFormat: ":
                        return timerFormats.IndexOf(appSettings.TimerFormat.ToString());
                    default:
                        return 0;
                }
            }
            set {
                switch (Title) {
                    case "DisplayMode: ":
                        appSettings.DisplayMode = (DisplayModeEnum)value;
                        break;
                    case "TimerFormat: ":
                        appSettings.TimerFormat = (TimerFormat)value;
                        break;
                    default:
                        break;
                }
            }
        }

        public SettingItem(string titile, InputControlTypes visibleControl) {
            Title = titile;
            InputControlVisibility = new Dictionary<InputControlTypes, Visibility>();
            foreach (InputControlTypes t in ICTs)
                InputControlVisibility.Add(t, Visibility.Collapsed);
            InputControlVisibility[visibleControl] = Visibility.Visible;
        }

        public Visibility GetControlVisibility(InputControlTypes visibleControl) => InputControlVisibility[visibleControl];
    }

    public class SettingItemGroup {
        public string Class { get; set; }

        public ObservableCollection<SettingItem> Items { get; set; }

        public static List<SettingItemGroup> Instance { get; private set; }

        static SettingItemGroup() {
            Instance = new List<SettingItemGroup>();

            var timingGroup = new SettingItemGroup { Class = "Timing", Items = new ObservableCollection<SettingItem>() };
            timingGroup.Items.Add(new SettingItem(titile: "NeedObserving: ", visibleControl: InputControlTypes.ToggleSwitch));
            timingGroup.Items.Add(new SettingItem(titile: "StartDelay: ", visibleControl: InputControlTypes.Slider));
            timingGroup.Items.Add(new SettingItem(titile: "DisplayMode: ", visibleControl: InputControlTypes.ComboBox));

            var scrambleGroup = new SettingItemGroup { Class = "Scramble", Items = new ObservableCollection<SettingItem>() };
            //scrambleGroup.Items.Add(new SettingItem(titile: "TestInput: ", visibleControl: InputControlTypes.ToggleSwitch));


            var statisticsGroup = new SettingItemGroup { Class = "Statistics", Items = new ObservableCollection<SettingItem>() };
            //statisticsGroup.Items.Add(new SettingItem(titile: "TestInput: ", visibleControl: InputControlTypes.Slider));


            var userInterfaceGroup = new SettingItemGroup { Class = "UserInterface", Items = new ObservableCollection<SettingItem>() };
            userInterfaceGroup.Items.Add(new SettingItem(titile: "TimerFormat: ", visibleControl: InputControlTypes.ComboBox));
            userInterfaceGroup.Items.Add(new SettingItem(titile: "ShowScambleState: ", visibleControl: InputControlTypes.ToggleSwitch));

            Instance.Add(timingGroup);
            Instance.Add(scrambleGroup);
            Instance.Add(statisticsGroup);
            Instance.Add(userInterfaceGroup);
        }
    }
}
