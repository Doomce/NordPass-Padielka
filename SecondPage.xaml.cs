// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PasswordManagerWINUI.BackEndLogic;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace PasswordManagerWINUI
{
    public sealed partial class SecondPage : Page
    {
        private PasswordListItemModel Model;
        private (CommandBar, AppBarToggleButton) ActiveBar;

        public SecondPage()
        {
            this.InitializeComponent();
            Model = new PasswordListItemModel();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Refresh_Btn_OnClick(object sender, RoutedEventArgs e)
        {
            Model.Passwords.Add(new PasswordItem { Title = "GOOGLE.COM", Password = "password12333", UserName = "TEST"});
            var obj = ListViewPasswords.ItemTemplate;
            ListViewPasswords.ItemsSource = Model.Passwords;
            GC.Collect();
        }

        private void Pointer_Entered_OnElem(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Microsoft.UI.Input.PointerDeviceType.Pen)
            {
                var item = sender as ListViewItem;
                HideOrShowControls(item.Content as CommandBar, true);
            }
        }

        private void Pointer_Exited_OnElem(object sender, PointerRoutedEventArgs e)
        {
            var item = sender as ListViewItem;
            var cmdBar = item.Content as CommandBar;
            if (cmdBar.IsOpen) return;
            if (cmdBar.PrimaryCommands[0] is AppBarToggleButton btn && btn.IsChecked == true)
            {
                if (ActiveBar.Item2 != null && btn == ActiveBar.Item2 && ActiveBar.Item2.DataContext is PasswordItem context && context.IsNotPassHidden) ActiveBar.Item1 = cmdBar;
                return;
            }
            HideOrShowControls(cmdBar, false);
        }

        private async void PassBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (AppBarToggleButton)sender;
            var dataItem = button.DataContext as PasswordItem;

            if (ActiveBar.Item1 != null) CheckIfVisibleExists(ref button);

            if (dataItem.GetState() == PasswordItemState.Hidden)
            {
                if (!await Security.CheckSecurity())
                {
                    button.IsChecked = false;
                    return;
                }
            }

            ChangeVisibility(dataItem);
            ChangeShowPassBtnText(button, dataItem.IsNotPassHidden);
            ActiveBar.Item2 = button;
            GC.Collect();
        }

        private void CheckIfVisibleExists(ref AppBarToggleButton senderBtn)
        {
            var cmdBtn = ActiveBar.Item1.PrimaryCommands[0] as AppBarToggleButton;
            if (cmdBtn == senderBtn) return;
            var activeContext = ActiveBar.Item1.DataContext as PasswordItem;
            if (!activeContext.IsNotPassHidden) return;
            
            ChangeVisibility(activeContext);
            HideOrShowControls(ActiveBar.Item1, false);
            ChangeShowPassBtnText(cmdBtn, activeContext.IsNotPassHidden);
            ActiveBar = (null, null);
        }

        private void HideOrShowControls(CommandBar cmdBar, bool show)
        {
            foreach (Control cmd in cmdBar.PrimaryCommands) cmd.Opacity = show ? 1 : 0;
        }

        private void ChangeVisibility(PasswordItem dataContext)
        {
            if (dataContext.GetState() == PasswordItemState.Hidden) dataContext.ChangeState(PasswordItemState.Visible);
            else dataContext.ChangeState(PasswordItemState.Hidden);
            dataContext.OnPropertyChanged(nameof(dataContext.IsNotPassHidden));
            dataContext.OnPropertyChanged(nameof(dataContext.PassHiddenString));
        }

        private void ChangeShowPassBtnText(AppBarToggleButton button, bool isBtnActive)
        {
            if (button is null) return;
            button.Label = isBtnActive ? "Sl�pti slapta�od�" : "Per�i�r�ti slapta�od�";
            button.Icon = new FontIcon()
            {
                FontFamily = new FontFamily("Segoe MDL2 Assets"),
                Glyph = isBtnActive ? "\uED1A" : "\uF78D"
            };
            button.IsChecked = isBtnActive;
        }

        private void CopyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var button = (AppBarButton)sender;
            if (button.DataContext is not PasswordItem dataItem) return;
            var package = new DataPackage();
            package.SetText(dataItem.Password);
            Clipboard.SetContent(package);
        }
    }
}
