// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PasswordManagerWINUI.Appearance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using static System.Net.WebRequestMethods;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PasswordManagerWINUI
{
    public partial class NavigationControl : UserControl
    {
        private FrameNavigationOptions navOptions = new FrameNavigationOptions();

        public static void ShowMessage(string title, string message, InfoBarSeverity severity)
        {
            GlobalInfoBar.MessageBar.IsOpen = false;
            GlobalInfoBar.MessageBar.Title = title;
            GlobalInfoBar.MessageBar.Message = message;
            GlobalInfoBar.MessageBar.Severity = severity;
            GlobalInfoBar.MessageBar.IsOpen = true;
        }

        public NavigationControl()
        {
            InitializeComponent();
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            navOptions.TransitionInfoOverride = args.RecommendedNavigationTransitionInfo;
            if (sender.PaneDisplayMode == NavigationViewPaneDisplayMode.LeftCompact)
            {
                navOptions.IsNavigationStackEnabled = false;
            }
            Type pageType = typeof(SecondPage); ;
            if (args.InvokedItem == "Vartotojas")
            {
                pageType = typeof(SecondPage);
            }
            else if (args.InvokedItem == "Slaptažodžiai")
            {
                //pageType = typeof(SamplePage2);
            }
            else if (args.InvokedItem == "Parametrai")
            {
                //pageType = typeof(SamplePage3);
            }
            ContentFrame.NavigateToType(pageType, null, navOptions);

        }
    }
}
