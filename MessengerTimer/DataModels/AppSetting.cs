﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace MessengerTimer.DataModels {
    public class AppSettings : INotifyPropertyChanged {
        //Config Var
        public bool NeedObserving {
            get {
                return ReadSettings(nameof(NeedObserving), true);
            }
            set {
                SaveSettings(nameof(NeedObserving), value);
                NotifyPropertyChanged();
            }
        }

        public long StartDelay {
            get {
                return ReadSettings(nameof(StartDelay), 3000000);
            }
            set {
                SaveSettings(nameof(StartDelay), value);
                NotifyPropertyChanged();
            }
        }

        public TimerFormat TimerFormat {
            get {
                return ReadSettings(nameof(TimerFormat), TimerFormat.SSFFF);
            }
            set {
                SaveSettings(nameof(TimerFormat), value);
                NotifyPropertyChanged();
            }
        }

        public DisplayModeEnum DisplayMode {
            get {
                return (DisplayModeEnum)ReadSettings(nameof(DisplayMode), 0);
            }
            set {
                SaveSettings(nameof(DisplayModeEnum), value);
                NotifyPropertyChanged();
            }
        }

        public int CurrentDataGroupIndex {
            get {
                return ReadSettings(nameof(CurrentDataGroupIndex), 0);
            }
            set {
                SaveSettings(nameof(CurrentDataGroupIndex), value);
                NotifyPropertyChanged();
            }
        }

        public DateTime LastUpdateImageTime {
            get {
                return ReadSettings(nameof(LastUpdateImageTime), DateTimeOffset.MinValue).DateTime;
            }
            set {
                SaveSettings(nameof(LastUpdateImageTime), (DateTimeOffset)value);
            }
        }

        public bool ShowScrambleState {
            get {
                return ReadSettings(nameof(ShowScrambleState), true);
            }
            set {
                SaveSettings(nameof(ShowScrambleState), value);
            }
        }

        public ApplicationDataContainer LocalSettings { get; set; }

        public AppSettings() {
            LocalSettings = ApplicationData.Current.LocalSettings;
        }

        private void SaveSettings(string key, object value) {
            LocalSettings.Values[key] = value;
        }

        private T ReadSettings<T>(string key, T defaultValue) {
            if (LocalSettings.Values.ContainsKey(key)) {
                return (T)LocalSettings.Values[key];
            }
            if (null != defaultValue) {
                return defaultValue;
            }
            return default(T);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName]string propName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
