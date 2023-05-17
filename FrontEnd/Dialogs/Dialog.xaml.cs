// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Azure;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PasswordManagerWINUI.FrontEnd.PasswordDialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    internal sealed partial class Dialog : Page
    {
        private enum DialogType
        {
            AddAccount,
            EditAccount,
            RemoveAccount
        }

        private DialogType type;
        private ContentDialog dialog = new ContentDialog();
        private XamlRoot Root;

        public Dialog(XamlRoot root)
        {
            this.InitializeComponent();
            Root = root;
        }

       
        public async Task<bool> GetRemoveAccountResponse()
        {
            return await dialog.ShowAsync() == ContentDialogResult.Primary;
        }

        public Dialog ForRemoveAccount()
        {
            type = DialogType.RemoveAccount;
            BuildDialog();
            return this;
        }

        public Dialog ForAddAccount()
        {
            type = DialogType.AddAccount;

            BuildDialog();

            Tooltip1.Text = "Platforma, kurioje yra paskyra";
            Tooltip2.Text = "Vartotojo vardas platformoje";
            TooltipPass1.Text = "Slapta�odis";

            Tooltip1.Visibility = Visibility.Visible;
            Tooltip2.Visibility = Visibility.Visible;
            TooltipPass1.Visibility = Visibility.Visible;

            PassBox1.Visibility = Visibility.Visible;
            FirstTextBox.Visibility = Visibility.Visible;
            SecondTextBox.Visibility = Visibility.Visible;

            return this;
        }

        public Dialog ForEditAccount()
        {
            type = DialogType.EditAccount;

            BuildDialog();

            PassBox1.Visibility = Visibility.Visible; 
            PassBox2.Visibility = Visibility.Visible;

            TooltipPass1.Visibility = Visibility.Visible;
            TooltipPass2.Visibility = Visibility.Visible;

            TooltipPass1.Text = "Naujasis slapta�odis";
            TooltipPass2.Text = "Pakartokite nauj�j� slapta�od�";

            return this;
        }

        public async Task<PasswordItem> GetNewAccount()
        {
            if (type != DialogType.AddAccount) return null;

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) { }

            var portal = FirstTextBox.Text;
            var username = SecondTextBox.Text;
            var password = PassBox1.Password;

            if (String.IsNullOrEmpty(portal) || String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password)) return null;
            return new PasswordItem { Title = portal, Password = password, UserName = username };
        }

        public async Task<string> GetNewPassword(string oldPassword)
        {
            if (type != DialogType.EditAccount) return String.Empty;

            if (await dialog.ShowAsync() != ContentDialogResult.Primary) return null;

            var passw1 = PassBox1.Password;
            var passw2 = PassBox2.Password;

            if (String.IsNullOrEmpty(passw1) || String.IsNullOrEmpty(passw2)) { return String.Empty; }

            if (passw1 != passw2)
            {
                NavigationControl.ShowMessage("Informacija", "J�s �ved�te skirtingus slapta�od�ius. Bandykite dar kart�.", InfoBarSeverity.Warning);
                return String.Empty;
            }

            if (passw1 == oldPassword)
            {
                NavigationControl.ShowMessage("Informacija", "Kam tok� pat� slapta�od� keisti?", InfoBarSeverity.Error);
                return String.Empty;
            }
            return passw1;
        }


        private void BuildDialog()
        {
            dialog.XamlRoot = Root;
            dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            dialog.CloseButtonText = "At�aukti";
            dialog.DefaultButton = ContentDialogButton.Primary;

            switch (type)
            {
                case DialogType.EditAccount:
                    {
                        dialog.Title = "Slapta�od�io redagavimas";
                        dialog.PrimaryButtonText = "I�saugoti";
                        dialog.Content = this;
                        break;
                    }
                case DialogType.RemoveAccount:
                    {
                        dialog.Title = "Slapta�od�io pa�alinimas";
                        dialog.PrimaryButtonText = "Pa�alinti";
                        dialog.Content = "�is slapta�odis bus i�trintas i� duomen� baz�s. �io veiksmo atkurti nebebus galima!";
                        break;
                    }
                case DialogType.AddAccount:
                    {
                        dialog.Title = "Paskyros prid�jimas";
                        dialog.PrimaryButtonText = "I�saugoti";
                        dialog.Content = this;
                        break;
                    }
            }

        }
    }
}
