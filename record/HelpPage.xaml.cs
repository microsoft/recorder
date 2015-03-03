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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using record.Resources;

/// <summary>
/// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556
/// </summary>
namespace record
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HelpPage : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HelpPage()
        {
            this.InitializeComponent();
            var version = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            var versionRun = new Run()
            {
                Text = String.Format(AppResources.AboutPage_VersionRun, version) + "\n"
            };
            VersionParagraph.Inlines.Add(versionRun);
            // Application about text
            var aboutRun = new Run()
            {
                Text = AppResources.HelpPage_AboutRun + "\n"
            };
            AboutParagraph.Inlines.Add(aboutRun);
            // Link to project homepage
            var projectRunText = AppResources.AboutPage_ProjectRun;
            var projectRunTextSpans = projectRunText.Split(new string[] { "{0}" }, StringSplitOptions.None);
            var projectRunSpan1 = new Run { Text = projectRunTextSpans[0] };
            var projectsLink = new Hyperlink();
            projectsLink.Inlines.Add(AppResources.AboutPage_Hyperlink_Project);
            projectsLink.Click += ProjectsLink_Click;
            projectsLink.Foreground = new SolidColorBrush((Color)Application.Current.Resources["PhoneForegroundColor"]);
            projectsLink.MouseOverForeground = new SolidColorBrush((Color)Application.Current.Resources["PhoneAccentColor"]);
            var projectRunSpan2 = new Run { Text = projectRunTextSpans[1] + "\n" };
            ProjectParagraph.Inlines.Add(projectRunSpan1);
            ProjectParagraph.Inlines.Add(projectsLink);
            ProjectParagraph.Inlines.Add(projectRunSpan2);
        }

        /// <summary>
        /// Occurs when project link is clicked
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void ProjectsLink_Click(object sender, RoutedEventArgs e)
        {
            var webBrowserTask = new WebBrowserTask()
            {
                Uri = new Uri(AppResources.AboutPage_Hyperlink_Project, UriKind.Absolute)
            };
            webBrowserTask.Show();
        }
    }
}
