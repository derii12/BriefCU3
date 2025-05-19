using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.FirebasePushNotification;

using Xamarin.Essentials;

namespace appslovo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
          
            MainPage = new NavigationPage(new homepage());
            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;

        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            Preferences.Set("device_token", e.Token.ToString());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
