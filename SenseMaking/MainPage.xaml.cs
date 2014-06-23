//*********************************************************************
// Component:   SensorCore Recorder
//
// Environment: Windows Phone 8.1
//
// Copyright (c) 2014 Nokia Corporation. All rights reserved. 
//
// This component and the accompanying materials are copyrighted by 
// Nokia and/or its subsidiaries. 
//
// This software, including documentation, is protected by copyright
// controlled by Nokia Corporation. All rights are reserved. Copying,
// including reproducing, storing, adapting or translating, any or all
// of this material requires the prior written consent of Nokia Corporation.
// This material also contains confidential information which may not be
// disclosed to others without the prior written consent of Nokia.
//*********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641
using Windows.UI.Xaml.Shapes;
using Lumia.Sense;
using Lumia.Sense.Testing;

namespace SenseMaking
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public enum SenseType
        {
            Activity,
            Places,
            Steps,
            TrackPoint
        };

        private Recording ActivityRecording = new Recording();
        private Recording PlacesRecording = new Recording();
        private Recording StepsRecording = new Recording();
        private Recording TrackPointRecording = new Recording();

        private DispatcherTimer recordingTimer = new DispatcherTimer();

        private PlaceMonitor pMonitor;
        private StepCounter sCounter;
        private TrackPointMonitor tpTracker;
        private ActivityMonitor aMonitor;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.Loaded += (sender, args) =>
            {
                recordingTimer.Interval = TimeSpan.FromSeconds(1);
                recordingTimer.Tick += recordingTimer_Tick;
                recordingTimer.Start();
            };
        }

        void recordingTimer_Tick(object sender, object e)
        {
            if (ActivityRecording.ItemState == State.Recording)
            {
                ActivityClock.Text = (DateTime.Now - ActivityRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (PlacesRecording.ItemState == State.Recording)
            {
                PlaceClock.Text = (DateTime.Now - PlacesRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (StepsRecording.ItemState == State.Recording)
            {
                StepClock.Text = (DateTime.Now - StepsRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (TrackPointRecording.ItemState == State.Recording)
            {
                TrackPointClock.Text = (DateTime.Now - TrackPointRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private async void OnResetClicked(object sender, RoutedEventArgs e)
        {
            if (ActivityRecording.ItemState == State.Recording)
            {
                await ActivityRecording.Recorder.StopAsync();
            }
            if (PlacesRecording.ItemState == State.Recording)
            {
                await PlacesRecording.Recorder.StopAsync();
            }
            if (StepsRecording.ItemState == State.Recording)
            {
                await StepsRecording.Recorder.StopAsync();
            }
            if (TrackPointRecording.ItemState == State.Recording)
            {
                await TrackPointRecording.Recorder.StopAsync();
            }

            ActivityClock.Text = "00:00:00:00";
            PlaceClock.Text = "00:00:00:00";
            TrackPointClock.Text = "00:00:00:00";
            StepClock.Text = "00:00:00:00";

            ActivityRecording.ItemState = State.Empty;
            PlacesRecording.ItemState = State.Empty;
            StepsRecording.ItemState = State.Empty;
            TrackPointRecording.ItemState = State.Empty;

            ActivityButton.Content = "record";
            PlacesButton.Content = "record";
            StepsButton.Content = "record";
            TrackPointButton.Content = "record";

            ChangeMarkerColor(ActivityMarker, ActivityRecording.ItemState);
            ChangeMarkerColor(PlacesMarker, PlacesRecording.ItemState);
            ChangeMarkerColor(StepMarker, StepsRecording.ItemState);
            ChangeMarkerColor(TrackPointMarker, TrackPointRecording.ItemState);
        }

        private void OnHelpClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(HelpPage));
        }

        private void OnAboutClicked(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage));
        }

        private void ChangeMarkerColor(Ellipse marker, State state)
        {
            switch (state)
            {
                case State.Empty:
                    marker.Fill = new SolidColorBrush(Color.FromArgb(255, 0x64, 0x76, 0x87));
                    break;
                case State.Recording:
                    marker.Fill = new SolidColorBrush(Color.FromArgb(255, 0xe5, 0x14, 0x00));
                    break;
                case State.Stopped:
                    marker.Fill = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x50, 0xef));
                    break;

            }
        }

        private void ChangeButtonState(Button button, State state)
        {
            switch (state)
            {
                case State.Empty:
                    button.Content = "record";
                    break;
                case State.Recording:
                    button.Content = "stop";
                    break;
                case State.Stopped:
                    button.Content = "save";
                    break;
            }

        }

        private async void ActivityButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, ActivityRecording, ActivityMarker);

            await HandleSensorActivity(ActivityRecording, SenseType.Activity);
        }
        
        private async void PlacesButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, PlacesRecording, PlacesMarker);

            await HandleSensorActivity(PlacesRecording, SenseType.Places);
        }

        private async void StepsButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, StepsRecording, StepMarker);

            await HandleSensorActivity(StepsRecording, SenseType.Steps);
        }

        private async void TrackPointButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, TrackPointRecording, TrackPointMarker);

            await HandleSensorActivity(TrackPointRecording, SenseType.TrackPoint);
        }

        private void HandleStateChange(object sender, Recording rec, Ellipse marker)
        {
            if (rec.ItemState == State.Empty)
            {
                // Start recording
                rec.ItemState = State.Recording;
                rec.StarTime = DateTime.Now;
            }
            else if (rec.ItemState == State.Recording)
            {
                // Stop recording
                rec.ItemState = State.Stopped;
            }
            else if (rec.ItemState == State.Stopped)
            {
                // Stop recording
                rec.ItemState = State.Empty;
            }

            ChangeButtonState(sender as Button, rec.ItemState);
            ChangeMarkerColor(marker, rec.ItemState);
        }

        private async Task HandleSensorActivity(Recording rec, SenseType type)
        {
            if (rec.Recorder == null)
            {
                if (await CallSensorcoreApiAsync(async () =>
                {
                    switch (type)
                    {
                        case SenseType.Activity:
                            aMonitor = await ActivityMonitor.GetDefaultAsync();
                            break;
                        case SenseType.Places:
                            pMonitor = await PlaceMonitor.GetDefaultAsync();
                            break;
                        case SenseType.TrackPoint:
                            tpTracker = await TrackPointMonitor.GetDefaultAsync();
                            break;
                        case SenseType.Steps:
                            sCounter = await StepCounter.GetDefaultAsync();
                            break;
                    }
                }))
                {
                    Debug.WriteLine("Recorder initialized.");
                    switch (type)
                    {
                        case SenseType.Activity:
                            rec.Recorder = new SenseRecorder(aMonitor);
                            break;
                        case SenseType.Places:
                            rec.Recorder = new SenseRecorder(pMonitor);
                            break;
                        case SenseType.TrackPoint:
                            rec.Recorder = new SenseRecorder(tpTracker);
                            break;
                        case SenseType.Steps:
                            rec.Recorder = new SenseRecorder(sCounter);
                            break;
                    }
                }
                else
                {
                    Application.Current.Exit();
                }
            }

            if (rec.Recorder == null)
                return;

            switch (rec.ItemState)
            {
                case State.Recording:
                    await rec.Recorder.StartAsync();
                    break;
                case State.Stopped:
                    await rec.Recorder.StopAsync();
                    break;
                case State.Empty:
                    await rec.Recorder.GetRecording().SaveAsync();
                    break;
            }
        }

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
                        MessageDialog dialog = new MessageDialog("Location has been disabled. Do you want to open Location settings now?", "Information");
                        dialog.Commands.Add(new UICommand("Yes", async cmd => await SenseHelper.LaunchLocationSettingsAsync()));
                        dialog.Commands.Add(new UICommand("No"));
                        await dialog.ShowAsync();
                        new System.Threading.ManualResetEvent(false).WaitOne(500);
                        return false;

                    case SenseError.SenseDisabled:
                        dialog = new MessageDialog("Motion data has been disabled. Do you want to open Motion data settings now?", "Information");
                        dialog.Commands.Add(new UICommand("Yes", async cmd => await SenseHelper.LaunchSenseSettingsAsync()));
                        dialog.Commands.Add(new UICommand("No"));
                        await dialog.ShowAsync();
                        new System.Threading.ManualResetEvent(false).WaitOne(500);
                        return false;

                    default:
                        dialog = new MessageDialog("Failure: " + SenseHelper.GetSenseError(failure.HResult), "");
                        await dialog.ShowAsync();
                        return false;
                }
            }

            return true;
        }
    }
}
