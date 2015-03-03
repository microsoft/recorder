/*
 * The MIT License (MIT)
 * Copyright (c) 2015 Microsoft
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.  
 */
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Lumia.Sense;
using Lumia.Sense.Testing;

/// <summary>
/// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
/// </summar
namespace record
{
    /// <summary>
    /// Main page of the application
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Enum to declare the Sense type
        /// </summary>
        public enum SenseType
        {
            Activity,
            Places,
            Steps,
            Route
        };

        #region Private members
        /// <summary>
        /// Recording instance for activity recording 
        /// </summary>
        private Recording _activityRecording = new Recording();

        /// <summary>
        /// Recording instance for places recording 
        /// </summary>
        private Recording _placesRecording = new Recording();

        /// <summary>
        /// Recording instance for steps recording 
        /// </summary>
        private Recording _stepsRecording = new Recording();

        /// <summary>
        /// Recording instance for route recording 
        /// </summary>
        private Recording _routeRecording = new Recording();

        /// <summary>
        /// Dispatcher Timer instance
        /// </summary>
        private DispatcherTimer _recordingTimer = new DispatcherTimer();

        /// <summary>
        /// Place Monitor instance
        /// </summary>
        private PlaceMonitor _pMonitor;

        /// <summary>
        /// Step Counter instance
        /// </summary>
        private StepCounter _sCounter;

        /// <summary>
        /// Track Point Monitor instance
        /// </summary>
        private TrackPointMonitor _rTracker;

        /// <summary>
        /// Activity Monitor instance
        /// </summary>
        private ActivityMonitor _aMonitor;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            this.Loaded += (sender, args) =>
            {
                _recordingTimer.Interval = TimeSpan.FromSeconds(1);
                _recordingTimer.Tick += RecordingTimer_Tick;
                _recordingTimer.Start();
            };
        }

        /// <summary>
        /// Occurs when the timer has elapsed.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments.</param>
        void RecordingTimer_Tick(object sender, object e)
        {
            if (_activityRecording.ItemState == Status.Recording)
            {
                ActivityClock.Text = (DateTime.Now - _activityRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (_placesRecording.ItemState == Status.Recording)
            {
                PlaceClock.Text = (DateTime.Now - _placesRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (_stepsRecording.ItemState == Status.Recording)
            {
                StepClock.Text = (DateTime.Now - _stepsRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (_routeRecording.ItemState == Status.Recording)
            {
                RouteClock.Text = (DateTime.Now - _routeRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
        }

        /// <summary>
        /// Occurs when reset button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void OnResetClicked(object sender, EventArgs e)
        {
            if (_activityRecording.ItemState == Status.Recording)
            {
                await _activityRecording.Recorder.StopAsync();
            }
            if (_placesRecording.ItemState == Status.Recording)
            {
                await _placesRecording.Recorder.StopAsync();
            }
            if (_stepsRecording.ItemState == Status.Recording)
            {
                await _stepsRecording.Recorder.StopAsync();
            }
            if (_routeRecording.ItemState == Status.Recording)
            {
                await _routeRecording.Recorder.StopAsync();
            }
            ActivityClock.Text = "00:00:00:00";
            PlaceClock.Text = "00:00:00:00";
            RouteClock.Text = "00:00:00:00";
            StepClock.Text = "00:00:00:00";
            _activityRecording.ItemState = Status.Empty;
            _placesRecording.ItemState = Status.Empty;
            _stepsRecording.ItemState = Status.Empty;
            _routeRecording.ItemState = Status.Empty;
            ActivityButton.Content = "record";
            PlacesButton.Content = "record";
            StepsButton.Content = "record";
            RouteButton.Content = "record";
            ChangeMarkerColor(ActivityMarker, _activityRecording.ItemState);
            ChangeMarkerColor(PlacesMarker, _placesRecording.ItemState);
            ChangeMarkerColor(StepMarker, _stepsRecording.ItemState);
            ChangeMarkerColor(RouteMarker, _routeRecording.ItemState);
        }

        /// <summary>
        /// Occurs when help button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments.</param>
        private void OnHelpClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Occurs when about button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void OnAboutClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Changes the ellipse color based on the status of the recording
        /// </summary>
        /// <param name="marker">Ellipse instance</param>
        /// <param name="state">The status of recording</param>
        private void ChangeMarkerColor(Ellipse marker, Status state)
        {
            switch (state)
            {
                case Status.Empty:
                    marker.Fill = new SolidColorBrush(Color.FromArgb(255, 0x64, 0x76, 0x87));
                    break;
                case Status.Recording:
                    marker.Fill = new SolidColorBrush(Color.FromArgb(255, 0xe5, 0x14, 0x00));
                    break;
                case Status.Stopped:
                    marker.Fill = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x50, 0xef));
                    break;
            }
        }

        /// <summary>
        /// Changes the content of the button based on the status of the recording
        /// </summary>
        /// <param name="button">Button control which content will change</param>
        /// <param name="state">The status of recording</param>
        private void ChangeButtonState(Button button, Status state)
        {
            switch (state)
            {
                case Status.Empty:
                    button.Content = "record";
                    break;
                case Status.Recording:
                    button.Content = "stop";
                    break;
                case Status.Stopped:
                    button.Content = "save";
                    break;
            }
        }

        /// <summary>
        /// Occurs when activity monitor button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void ActivityButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, _activityRecording, ActivityMarker);
            await HandleSensorActivity(_activityRecording, SenseType.Activity);
        }

        /// <summary>
        /// Occurs when places monitor button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void PlacesButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, _placesRecording, PlacesMarker);
            await HandleSensorActivity(_placesRecording, SenseType.Places);
        }

        /// <summary>
        /// Occurs when steps counter button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void StepsButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, _stepsRecording, StepMarker);
            await HandleSensorActivity(_stepsRecording, SenseType.Steps);
        }

        /// <summary>
        /// Occurs when route tracker button is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private async void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, _routeRecording, RouteMarker);
            await HandleSensorActivity(_routeRecording, SenseType.Route);
        }

        /// <summary>
        /// Handles a state change of the recording instance
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="rec">Recording instance</param>
        /// <param name="marker">Ellipse instance</param>
        private void HandleStateChange(object sender, Recording rec, Ellipse marker)
        {
            if (rec.ItemState == Status.Empty)
            {
                // Start recording
                rec.ItemState = Status.Recording;
                rec.StarTime = DateTime.Now;
            }
            else if (rec.ItemState == Status.Recording)
            {
                // Stop recording
                rec.ItemState = Status.Stopped;
            }
            else if (rec.ItemState == Status.Stopped)
            {
                // Stop recording
                rec.ItemState = Status.Empty;
            }
            ChangeButtonState(sender as Button, rec.ItemState);
            ChangeMarkerColor(marker, rec.ItemState);
        }

        /// <summary>
        /// Check motion data settings
        /// </summary>
        private async void CheckMotionDataSettings()
        {
            if (!(await TrackPointMonitor.IsSupportedAsync()) || !(await PlaceMonitor.IsSupportedAsync()) || !(await StepCounter.IsSupportedAsync()) || !(await ActivityMonitor.IsSupportedAsync()))
            {
                MessageBoxResult dlg = MessageBox.Show("Unfortunately this device does not support SensorCore service");
                Application.Current.Terminate();
            }
            else
            {
                uint apiSet = await SenseHelper.GetSupportedApiSetAsync();
                MotionDataSettings settings = await SenseHelper.GetSettingsAsync();
                // Devices with old location settings
                if (!settings.LocationEnabled)
                {
                    MessageBoxResult dlg = MessageBox.Show("In order to recognize activities and view visited places you need to enable location in system settings. Do you want to open settings now? if no, applicatoin will exit", "Information", MessageBoxButton.OKCancel);
                    if (dlg == MessageBoxResult.OK)
                        await SenseHelper.LaunchLocationSettingsAsync();
                }
                if (!settings.PlacesVisited)
                {
                    MessageBoxResult dlg = new MessageBoxResult();
                    if (settings.Version < 2)
                    {
                        //device which has old motion data settings.
                        //this is equal to motion data settings on/off in old system settings(SDK1.0 based)
                        dlg = MessageBox.Show("In order to count steps you need to enable Motion data collection in Motion data settings. Do you want to open settings now?", "Information", MessageBoxButton.OKCancel);
                        if (dlg == MessageBoxResult.Cancel)
                            Application.Current.Terminate();
                    }
                    else
                    {
                        dlg = MessageBox.Show("In order to recognize activities you need to 'enable Places visited' and 'DataQuality to detailed' in Motion data settings. Do you want to open settings now? ", "Information", MessageBoxButton.OKCancel);
                    }
                    if (dlg == MessageBoxResult.OK)
                        await SenseHelper.LaunchSenseSettingsAsync();
                }
                else if (apiSet >= 3 && settings.DataQuality == DataCollectionQuality.Basic)
                {
                    MessageBoxResult dlg = MessageBox.Show("In order to recognize biking activity you need to enable detailed data collection in Motion data settings. Do you want to open settings now?", "Information", MessageBoxButton.OKCancel);
                    if (dlg == MessageBoxResult.OK)
                        await SenseHelper.LaunchSenseSettingsAsync();
                }
            }
        }

        /// <summary>
        /// Initialize SensorCore 
        /// </summary>
        /// <param name="rec">Recording instance</param>
        /// <param name="type">Sense type</param>
        /// <returns>Asynchronous task</returns>
        private async Task HandleSensorActivity(Recording rec, SenseType type)
        {           
            if (rec.Recorder == null)
            {
                if (await CallSensorcoreApiAsync(async () =>
                {
                    switch (type)
                    {
                        case SenseType.Activity:
                            _aMonitor = await ActivityMonitor.GetDefaultAsync();
                            break;
                        case SenseType.Places:
                            _pMonitor = await PlaceMonitor.GetDefaultAsync();
                            break;
                        case SenseType.Route:
                            _rTracker = await TrackPointMonitor.GetDefaultAsync();
                            break;
                        case SenseType.Steps:
                            _sCounter = await StepCounter.GetDefaultAsync();
                            break;
                    }
                }))
                {
                    Debug.WriteLine("Recorder initialized.");
                    switch (type)
                    {
                        case SenseType.Activity:
                            rec.Recorder = new SenseRecorder(_aMonitor);
                            break;
                        case SenseType.Places:
                            rec.Recorder = new SenseRecorder(_pMonitor);
                            break;
                        case SenseType.Route:
                            rec.Recorder = new SenseRecorder(_rTracker);
                            break;
                        case SenseType.Steps:
                            rec.Recorder = new SenseRecorder(_sCounter);
                            break;
                    }
                }
                else return;
            }
            if (rec.Recorder == null)
                return;
            else
            {
                await ActivateAsync();
                switch (rec.ItemState)
                {
                    case Status.Recording:
                        await rec.Recorder.StartAsync();
                        break;
                    case Status.Stopped:
                        await rec.Recorder.StopAsync();
                        break;
                    case Status.Empty:
                        await rec.Recorder.GetRecording().SaveAsync();
                        break;
                }
            }
        }

        /// <summary>
        /// Re-establishes the communication channel with underlying sensor, if it doesn't
        /// already exist.  Connection needs to be re-established when the application
        /// is brought to foreground.
        /// </summary>
        /// <returns>Returns an IAsyncAction object that is used to control the asynchronous operation.</returns>
        public async Task ActivateAsync()
        {
            if (_aMonitor != null)
            {
                await _aMonitor.ActivateAsync();
            }
            if (_sCounter != null)
            {
                await _sCounter.ActivateAsync();
            }
            if (_rTracker != null)
            {
                await _rTracker.ActivateAsync();
            }
            if (_pMonitor != null)
            {
                await _pMonitor.ActivateAsync();
            }
        }

        /// <summary>
        /// Performs asynchronous Sensorcore SDK operation and handles any exceptions
        /// </summary>
        /// <param name="action">The function delegate to execute asynchronously when one task in the tasks completes.</param>
        /// <returns><c>true</c> if call was successful, <c>false</c> otherwise</returns>
        private async Task<bool> CallSensorcoreApiAsync(Func<Task> action)
        {
            Exception failure = null;
            try
            {
                await action();
            }
            catch (Exception e)
            {
                failure = e;
            }
            if (failure != null)
            {
                switch (SenseHelper.GetSenseError(failure.HResult))
                {
                    case SenseError.LocationDisabled:
                        MessageBoxResult res = MessageBox.Show("Location has been disabled. Do you want to open Location settings now?", "Information", MessageBoxButton.OKCancel);
                        if (res == MessageBoxResult.OK)
                        {
                            await SenseHelper.LaunchLocationSettingsAsync();
                        }
                        return false;
                    case SenseError.SenseDisabled:
                        MessageBoxResult res2 = MessageBox.Show("Motion data has been disabled. Do you want to open Motion data settings now?", "Information", MessageBoxButton.OKCancel);
                        if (res2 == MessageBoxResult.OK)
                        {
                            await SenseHelper.LaunchSenseSettingsAsync();
                        }
                        return false;
                    default:
                        MessageBoxResult res3 = MessageBox.Show("Error:" + SenseHelper.GetSenseError(failure.HResult), "Information", MessageBoxButton.OK);
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Called when a page becomes the active page in a frame.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_recordingTimer != null && _recordingTimer.IsEnabled == false)
                _recordingTimer.Start();
            CheckMotionDataSettings();
        }

        /// <summary>
        /// Called when a page is no longer the active page in a frame.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected async override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            if (_recordingTimer != null)
                _recordingTimer.Stop();
            if (_pMonitor != null)
                await _pMonitor.DeactivateAsync();
            if (_sCounter != null)
                await _sCounter.DeactivateAsync();
            if (_rTracker != null)
                await _rTracker.DeactivateAsync();
            if (_aMonitor != null)
                await _aMonitor.DeactivateAsync();
            _activityRecording.Recorder = null;
            _placesRecording.Recorder = null;
            _stepsRecording.Recorder = null;
            _routeRecording.Recorder = null;
        }         
    }
}