using System;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using Android.Widget;
using Android.Content;
using Java.Lang;
using Java.Util.Logging;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Appwidget;
using Android.Content;
using System.Configuration;
using System.IO;
using Xamarin.Essentials;
using Android.Runtime;
using Android.Content.PM;
using System.Collections.Specialized;
using Android.Widget;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Android.Preferences;
using Plugin.FirebasePushNotification;
using System.Collections.Generic;
using Xamarin.Forms;

namespace appslovo.Droid
{
    [Activity(Label = "Brief", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            base.OnCreate(savedInstanceState);


            //here is what you want to do always, i just want to push a notification every 5 seconds here
            Rg.Plugins.Popup.Popup.Init(this);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            
            LoadApplication(new App());
            if (DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - Preferences.Get("last_autentification_id", 0.0) > 2)
            {
                homepage.idinfier();
            }
            FirebasePushNotificationManager.ProcessIntent(this, Intent);

        }


        protected override void OnResume()
        {


     
            

            base.OnResume();

        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            

        }
        protected override void OnPause()
        {
            base.OnPause();
          

        }



        public override void OnBackPressed()
        {
            Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed);
        }
    }
}