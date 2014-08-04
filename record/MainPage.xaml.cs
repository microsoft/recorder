using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using record.Resources;
using Lumia.Sense;
using Lumia.Sense.Testing;

namespace record
{
    public partial class MainPage : PhoneApplicationPage
    {
        public enum SenseType
        {
            Activity,
            Places,
            Steps,
            Route
        };

        private Recording ActivityRecording = new Recording();
        private Recording PlacesRecording = new Recording();
        private Recording StepsRecording = new Recording();
        private Recording RouteRecording = new Recording();

        private DispatcherTimer recordingTimer = new DispatcherTimer();

        private PlaceMonitor pMonitor;
        private StepCounter sCounter;
        private TrackPointMonitor rTracker;
        private ActivityMonitor aMonitor;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            PhoneApplicationService.Current.ApplicationIdleDetectionMode = IdleDetectionMode.Disabled;
            
            this.Loaded += (sender, args) =>
            {
                recordingTimer.Interval = TimeSpan.FromSeconds(1);
                recordingTimer.Tick += recordingTimer_Tick;
                recordingTimer.Start();
            };  
        }

        void recordingTimer_Tick(object sender, object e)
        {
            if (ActivityRecording.ItemState == Status.Recording)
            {
                ActivityClock.Text = (DateTime.Now - ActivityRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (PlacesRecording.ItemState == Status.Recording)
            {
                PlaceClock.Text = (DateTime.Now - PlacesRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (StepsRecording.ItemState == Status.Recording)
            {
                StepClock.Text = (DateTime.Now - StepsRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
            if (RouteRecording.ItemState == Status.Recording)
            {
                RouteClock.Text = (DateTime.Now - RouteRecording.StarTime).ToString(@"dd\:hh\:mm\:ss");
            }
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            if (ActivityRecording.ItemState == Status.Recording)
            {
                ActivityRecording.Recorder.StopAsync();
            }
            if (PlacesRecording.ItemState == Status.Recording)
            {
                PlacesRecording.Recorder.StopAsync();
            }
            if (StepsRecording.ItemState == Status.Recording)
            {
                StepsRecording.Recorder.StopAsync();
            }
            if (RouteRecording.ItemState == Status.Recording)
            {
                RouteRecording.Recorder.StopAsync();
            }

            ActivityClock.Text = "00:00:00:00";
            PlaceClock.Text = "00:00:00:00";
            RouteClock.Text = "00:00:00:00";
            StepClock.Text = "00:00:00:00";

            ActivityRecording.ItemState = Status.Empty;
            PlacesRecording.ItemState = Status.Empty;
            StepsRecording.ItemState = Status.Empty;
            RouteRecording.ItemState = Status.Empty;

            ActivityButton.Content = "record";
            PlacesButton.Content = "record";
            StepsButton.Content = "record";
            RouteButton.Content = "record";

            ChangeMarkerColor(ActivityMarker, ActivityRecording.ItemState);
            ChangeMarkerColor(PlacesMarker, PlacesRecording.ItemState);
            ChangeMarkerColor(StepMarker, StepsRecording.ItemState);
            ChangeMarkerColor(RouteMarker, RouteRecording.ItemState);
        }

        private void OnHelpClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

        private void OnAboutClicked(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative));
        }

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

        private async void RouteButton_Click(object sender, RoutedEventArgs e)
        {
            HandleStateChange(sender, RouteRecording, RouteMarker);

            await HandleSensorActivity(RouteRecording, SenseType.Route);
        }

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
                        case SenseType.Route:
                            rTracker = await TrackPointMonitor.GetDefaultAsync();
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
                        case SenseType.Route:
                            rec.Recorder = new SenseRecorder(rTracker);
                            break;
                        case SenseType.Steps:
                            rec.Recorder = new SenseRecorder(sCounter);
                            break;
                    }
                }
                else
                {
                    //Application.Current.Exit();
                }
            }

            if (rec.Recorder == null)
                return;

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
                        MessageBoxResult res = MessageBox.Show(
                            "Location has been disabled. Do you want to open Location settings now?",
                            "Information",
                            MessageBoxButton.OKCancel
                            );
                        if (res == MessageBoxResult.OK)
                        {
                            await SenseHelper.LaunchLocationSettingsAsync();
                        }

                        return false;

                    case SenseError.SenseDisabled:

                        MessageBoxResult res2 = MessageBox.Show(
                            "Motion data has been disabled. Do you want to open Motion data settings now?",
                            "Information",
                            MessageBoxButton.OKCancel
                            );

                        if (res2 == MessageBoxResult.OK)
                        {
                            await SenseHelper.LaunchSenseSettingsAsync();
                        }

                        return false;


                    default:
                        MessageBoxResult res3 = MessageBox.Show(
                              "Error:" + SenseHelper.GetSenseError(failure.HResult),
                              "Information",
                              MessageBoxButton.OK);

                        return false;
                }
            }

            return true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (recordingTimer != null && recordingTimer.IsEnabled == false)
                recordingTimer.Start();
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (recordingTimer != null)
                recordingTimer.Stop();

            if (pMonitor != null)
                await pMonitor.DeactivateAsync();
            if (sCounter != null)
                await sCounter.DeactivateAsync();
            if (rTracker != null)
                await rTracker.DeactivateAsync();
            if (aMonitor != null)
                await aMonitor.DeactivateAsync();

            ActivityRecording.Recorder = null;
            PlacesRecording.Recorder = null;
            StepsRecording.Recorder = null;
            RouteRecording.Recorder = null;
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}