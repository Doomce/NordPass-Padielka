// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PasswordManagerWINUI.BackEndLogic.Database;
using PasswordManagerWINUI.BackEndLogic.Microsoft;
using PasswordManagerWINUI.BackEndLogic.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Page = Microsoft.UI.Xaml.Controls.Page;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PasswordManagerWINUI.FrontEnd
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstPage : Page
    {

        internal static MsAccount MicrosoftAccount = new MsAccount();

        public FirstPage()
        {     
            NavigationCacheMode = NavigationCacheMode.Enabled;
            InitUser();
        }

        private void SetGuestTexts()
        {
            HeaderTitle.Text = "SVE�IO PASKYRA";
            HeaderText.Text = "Naudojant paskyr� neprisijungus, j�s� slapta�od�iai negali b�ti i�saugomi ar naudojami keliuose �renginiuose.\n" +
                "Rekomenduojame prisijungti prie slapta�od�i� tvarkykl�s paskyros naudojant Microsoft arba Google paskyr�.";

            HeaderIcon.Glyph = "\uEE57";
        }

        private void SetUserTexts(string userName)
        {
            HeaderTitle.Text = userName;
            HeaderText.Text = "J�s esate prisijung�s su Microsoft paskyra.";

            //HeaderIcon.
            HeaderIcon.Glyph = "\uEE57";
        }

        private void InitUser()
        {
            DispatcherQueue.TryEnqueue(async () =>
            {
                MicrosoftAccount.isHidden = true;
                string msAccId = await MicrosoftAccount.TryLoginToAccountAsync();
                InitializeComponent();
                if (msAccId == null)
                {
                    SetGuestTexts();
                    return;
                }
                SetUserTexts(MicrosoftAccount.GetUserData().Item1);
                SqlMethods.AddOrGetUser(msAccId);
            });
            
        }

        private async void Login_Btn_Click(object sender, RoutedEventArgs e)
        {
            MicrosoftAccount.isHidden = false;
            string msAccId = await MicrosoftAccount.TryLoginToAccountAsync();
            if (msAccId == null) return;
            MicrosoftAccount.DisplayLoginInfo();
            SetUserTexts(MicrosoftAccount.GetUserData().Item1);
            SqlMethods.AddOrGetUser(msAccId);
        }

        private async void LogOut_Btn_Click(object sender, RoutedEventArgs e)
        {
            if (MicrosoftAccount == null) return;
            if (await MicrosoftAccount.SignOut())
            {
                SetGuestTexts();
                SqlMethods.LogOutUser();
            }
        }
    }
}
