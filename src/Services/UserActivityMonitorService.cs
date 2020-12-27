﻿//-------------------------------------------------------------------
// Copyright © 2019 Kindel Systems, LLC
// http://www.kindel.com
// charlie@kindel.com
// 
// Published under the MIT License.
// Source on GitHub: https://github.com/tig/mcec  
//-------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MCEControl {

    // Uses a global Windows hook to detect keyboard or mouse activity
    // Based on this post: https://www.codeproject.com/Articles/7294/Processing-Global-Mouse-and-Keyboard-Hooks-in-C
#pragma warning disable CA1724
    public sealed class UserActivityMonitorService : IDisposable {
        private System.DateTime LastTime;

        private static readonly Lazy<UserActivityMonitorService> lazy = new Lazy<UserActivityMonitorService>(() => new UserActivityMonitorService());
        private UserActivityMonitorService() {
        }

        private static Gma.UserActivityMonitor.GlobalEventProvider _userActivityMonitor = null;
        private Timer _timer = null;

        public static UserActivityMonitorService Instance => lazy.Value;
        public bool LogActivity { get; set; }
        public string ActivityCmd { get; set; } = "activity";
        public int DebounceTime { get; set; } = 5;
        public bool UnlockDetection { get; set; }
        public bool InputDetection { get; set; }
        /// <summary>
        /// Starts the Activity Monitor. 
        /// </summary>
        /// <param name="debounceTime">Specifies the maximum frequency at which activity messages will be sent in seconds.</param>
        public void Start() {
            LastTime = DateTime.Now;

            if (InputDetection) {
                Debug.Assert(_userActivityMonitor == null);
                _userActivityMonitor = new Gma.UserActivityMonitor.GlobalEventProvider();
                _userActivityMonitor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseMove);
                _userActivityMonitor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseClick);
                _userActivityMonitor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HookManager_KeyPress);
                _userActivityMonitor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HookManager_KeyDown);
                _userActivityMonitor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseDown);
                _userActivityMonitor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseUp);
                _userActivityMonitor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.HookManager_KeyUp);
                _userActivityMonitor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.HookManager_MouseDoubleClick);
            }

            if (UnlockDetection) {
                Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
                StartSessionUnlockedTimer();
            }

            // BUGBUG: If app is started with the session unlocked (which will be most of the time), the Session Unlocked Timer
            // does not start until a user input is detected (See Activity() below).
            // There is no documented way to detect whehter a session is unlocked.
            // This is not a big deal, but is a bug.
            Logger.Instance.Log4.Info($"ActivityMonitor: Start");
            Logger.Instance.Log4.Info($"ActivityMonitor: Keyboard/mouse input detection: {InputDetection}");
            Logger.Instance.Log4.Info($"ActivityMonitor: Desktop unlock detection: {UnlockDetection}");
            Logger.Instance.Log4.Info($"ActivityMonitor: Command: {ActivityCmd}");
            Logger.Instance.Log4.Info($"ActivityMonitor: Debounce Time: {DebounceTime} seconds");

            // TELEMETRY: 
            // what: when activity montioring is turned off
            // why: to understand how user trun activity monitoring on and off
            // how is PII protected: whether activity monitoring is on or off is not PII
            TelemetryService.Instance.TrackEvent("ActivityMonitor Start");
        }

        /// <summary>
        /// 
        /// </summary>
        private void StartSessionUnlockedTimer() {
            Debug.Assert(_timer == null);
            _timer = new Timer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = this.DebounceTime * 1000;
        }

        private void StopSessionUnlockTimer() {
            Debug.Assert(_timer != null);
            _timer.Stop();
            _timer.Dispose();
            _timer = null;
        }

        public void Stop() {
            Logger.Instance.Log4.Info($"ActivityMonitor: Stop");
            // TELEMETRY: 
            // what: when activity montioring is turned off
            // why: to understand how user trun activity monitoring on and off
            // how is PII protected: whether activity monitoring is on or off is not PII
            TelemetryService.Instance.TrackEvent("ActivityMonitor Stop");

            if (_userActivityMonitor != null) {
                _userActivityMonitor.Dispose();
                _userActivityMonitor = null;
            }

            if (UnlockDetection) {
                StopSessionUnlockTimer();
                Microsoft.Win32.SystemEvents.SessionSwitch -= new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);
            }
        }

        private void SystemEvents_SessionSwitch(object sender, Microsoft.Win32.SessionSwitchEventArgs e) {
            if (!UnlockDetection) return;

            if (e.Reason == SessionSwitchReason.SessionLock) {
                // Desktop has been locked - Pretty good signal there's not going to be any activity
                // Stop the timer
                Logger.Instance.Log4.Info($"ActivityMonitor: Session Locked");
                Debug.Assert(_timer != null);
                _timer.Enabled = false;
                StopSessionUnlockTimer();            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock) {
                // Desktop has been Unlocked - this is a signal there's activity. 
                // Start a repeating timer (using the same duration as debounce + 1 second) 
                Logger.Instance.Log4.Info($"ActivityMonitor: Session Unlocked");
                StartSessionUnlockedTimer();
                _timer.Enabled = true;
                Timer_Tick(null, null);
            }
        }

        private void Timer_Tick(object sender, EventArgs e) {
            //Logger.Instance.Log4.Info($"ActivityMonitor: Tick");
            this.Activity("Session is unlocked");
        }

        private void Activity(string source, string moreInfo = "") {
            if (_userActivityMonitor == null) {
                return;
            }

            // Enable desktop-locked/unlocked timer (desktop is clearly unlocked!)
            if (UnlockDetection && _timer != null) {
                _timer.Enabled = true;
            }

            if (LastTime.AddSeconds(DebounceTime) <= DateTime.Now) {
                // Only log/trigger if outside of debounce time
                Logger.Instance.Log4.Info($@"ActivityMonitor: Activity detected: {source} {moreInfo}");

                // TELEMETRY: 
                // what: the count of activity dectected
                // why: to understand how frequently activity is detected
                // how is PII protected: the frequency of activity is not PII
                TelemetryService.Instance.TelemetryClient.GetMetric($"activity Sent").TrackValue(1);

                MainWindow.Instance.SendLine(ActivityCmd);

                LastTime = DateTime.Now;
            }
        }

        private void HookManager_KeyDown(object sender, KeyEventArgs e) {
            Activity("KeyDown", LogActivity ? $"{e.KeyCode}" : "");
        }

        private void HookManager_KeyUp(object sender, KeyEventArgs e) {
            Activity("KeyUp", LogActivity ? $"{e.KeyCode}" : "");
        }


        private void HookManager_KeyPress(object sender, KeyPressEventArgs e) {
            Activity("KeyPress", LogActivity ? $"{e.KeyChar}" : "");
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e) {
            Activity($"MouseMove", LogActivity ? $"x={e.X:0000}; y={e.Y:0000}" : "");
        }

        private void HookManager_MouseClick(object sender, MouseEventArgs e) {
            Activity($"MouseClick", LogActivity ? $"{e.Button}" : "");
        }

        private void HookManager_MouseUp(object sender, MouseEventArgs e) {
            Activity($"MouseUp", LogActivity ? $"{e.Button}" : "");
        }

        private void HookManager_MouseDown(object sender, MouseEventArgs e) {
            Activity($"MouseDown", LogActivity ? $"{e.Button}" : "");
        }

        private void HookManager_MouseDoubleClick(object sender, MouseEventArgs e) {
            Activity($"MouseDoubleClick", LogActivity ? $"{e.Button}" : "");
        }

        private void HookManager_MouseWheel(object sender, MouseEventArgs e) {
            Activity($"MouseWheel", LogActivity ? $"{e.Delta:000}" : "");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    _timer?.Dispose();
                    _timer = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
