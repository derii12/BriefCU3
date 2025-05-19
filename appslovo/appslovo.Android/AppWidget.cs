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

namespace appslovo.Droid
{

    [BroadcastReceiver(Label = "Widget", Exported =true)]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [IntentFilter(new string[] { "android.intent.action.BOOT_COMPLETED" })]
    [IntentFilter(new[] { Intent.ActionTimeTick })]
    [IntentFilter(new[] { Intent.ActionApplicationRestrictionsChanged })]
    [IntentFilter(new string[] { "android.intent.action.TIME_CHANGED" })]
    
    [IntentFilter(new [] { Intent.ActionAppError })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class AppWidget : AppWidgetProvider
    {

        public static string web_connection = homepage.web_connection;
        public static Context context_wdgt;
        public static AppWidgetManager appWidgetManager_wdgt;
        public static int[] appWidgetIds_wdgt;
        public static RemoteViews widgetView_wdgt;
        public static bool post_load = false;
        public async static void Settexterror(string rr)
        {
            try
            {
                var me = new ComponentName(context_wdgt, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
                
                appWidgetManager_wdgt.UpdateAppWidget(me, BuildRemoteViews(context_wdgt, appWidgetIds_wdgt, "", rr, "a:a".Split(':')));
               // post_load = false;
            }
            catch
            {

            }
        }
    



        private static Random random = new Random();
        public static string RandomString(int length) // generating random string some lenght
        {
            const string chars = "ABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        }

        public static string ss;
        public static string nn;
        public string [] tt;
        public bool update_works = false;

        public double last_postload = 0;



        public async void update_post()
        {
            
            Preferences.Set("last_postload", DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            
               
                    try
                    {
                        string posts_result_str = Preferences.Get("user_friends_posts", "nointernet");
                        try
                        {
                            posts_result_str = await homepage.LoadFriendsPosts(Preferences.Get("token", "unlogged")); //search
                        }
                        catch { }
                        bool correct_post = false;
                        string posttxt = "~";
                        string[] post_info_list = posts_result_str.Split('•');
                        string[] post_info_list_time = post_info_list[1].Split('|');
                        string[] results_info_list = post_info_list[0].Split('~');
                        int clicks = Preferences.Get("widget-click-count", 1);
                        int posts_count = results_info_list.Count();
                        string[] this_post_info = results_info_list[clicks % posts_count].Split('|');
                        string current_username = this_post_info[1];
                        string current_key = this_post_info[3].Replace('*', '+');
                        string current_textpost = this_post_info[0].Replace('*', '+');
                        try
                        {
                            var key = RsaEncryptionExample.StringDecryption(current_key);
                            posttxt = SymmetricPostSequirity.DecryptSymmetric(current_textpost, key);
                            if (posttxt.Split('\n').Length < 8 && Regex.IsMatch(posttxt, @"^[а-яА-Яa-zA-Z0-9“_\n.,!?%\s-\«»„""”=+)('&$#-—–/\:;@><#@'^Ёё*[]+$") && posttxt != "")
                            {
                                correct_post = true;
                            }
                        }
                        catch
                        {
                            correct_post = true;
                            //post_textcolor = Color.FromHex("#A9A9A9");
                            //post_textsize = 15;
                        }
                        if (correct_post)
                        {
                            ss = posttxt;
                            nn = current_username;
                            tt = homepage.TimeChecker(Convert.ToInt32(post_info_list_time[clicks % posts_count])).Split(':');
                        }

                    }
                    catch (Exception err)
                    {
                        nn = "e"; //error sign

                    }


                    var me = new ComponentName(context_wdgt, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
                    appWidgetManager_wdgt.UpdateAppWidget(me, BuildRemoteViews(context_wdgt, appWidgetIds_wdgt, ss, nn, tt));

                
                await Task.Delay(930);

                update_post();

            
        
        }
        private static string AnnouncementClick = "widgetBackground";


        
        public async override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

           // homepage.autentification();

            // Check if the click is from the "Announcement" button
            if (AnnouncementClick.Equals(intent.Action) && post_load)
            {
                int clicks = Preferences.Get("widget-click-count", 1);
                Preferences.Set("widget-click-count", clicks + 1);

            }
            else
            {
                post_load = true;
                if (context_wdgt != null)
                {
                    context_wdgt = Application.Context;
                    double lat_update = Preferences.Get("last_postload", 0.0);
                    double period = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - lat_update;
                    if (period > 3)
                    {
                        homepage.autentification();
                        update_post();
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                   
                }
            }
        }




        public async override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            
                
                context_wdgt = context;
                appWidgetIds_wdgt = appWidgetIds;
                appWidgetManager_wdgt = appWidgetManager;
                var me = new ComponentName(context_wdgt, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
                appWidgetManager_wdgt.UpdateAppWidget(me, BuildRemoteViews(context_wdgt, appWidgetIds_wdgt, ss, nn, tt));
            
            
        }




        private static RemoteViews BuildRemoteViews(Context context, int[] appWidgetIds, string ss, string nn,string[] tt)
        {


            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);
            widgetView_wdgt = widgetView;
            
            SetTextViewText(widgetView, ss, nn, tt);

            RegisterClicks(context, appWidgetIds, widgetView);
           
            return widgetView;
        }






        public static async void SetTextViewText(RemoteViews widgetView, string ss, string nn,string[] tt)
        {
            widgetView.SetViewVisibility(Resource.Id.widgetGlobalError, Android.Views.ViewStates.Gone);
            if (nn == "e")
            {

                widgetView.SetViewVisibility(Resource.Id.widgetPostText, Android.Views.ViewStates.Gone);
                widgetView.SetViewVisibility(Resource.Id.widgetPostNameBlock, Android.Views.ViewStates.Invisible);
                widgetView.SetViewVisibility(Resource.Id.widgetErrorText1, Android.Views.ViewStates.Visible);
                widgetView.SetViewVisibility(Resource.Id.widgetErrorText2, Android.Views.ViewStates.Visible);
            }
            else
            {
                try
                {
                    
                    widgetView.SetViewVisibility(Resource.Id.widgetPostText, Android.Views.ViewStates.Visible);
                    widgetView.SetViewVisibility(Resource.Id.widgetPostNameBlock, Android.Views.ViewStates.Visible);
                    widgetView.SetViewVisibility(Resource.Id.widgetErrorText1, Android.Views.ViewStates.Gone);
                    widgetView.SetViewVisibility(Resource.Id.widgetErrorText2, Android.Views.ViewStates.Gone);
                    widgetView.SetTextViewText(Resource.Id.widgetPostName, nn);
                    widgetView.SetTextViewText(Resource.Id.widgetPostText, ss);
                    widgetView.SetTextViewText(Resource.Id.widgetTimerSeconds, tt[2]);
                    widgetView.SetTextViewText(Resource.Id.widgetTimerMinutes, tt[1]);
                    widgetView.SetTextViewText(Resource.Id.widgetTimerHours, tt[0]);
                }
                catch
                {

                }
            }
            
            
           
        }

        private static void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
        {
            var intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);
            widgetView.SetOnClickPendingIntent(Resource.Id.widgetPostNameBlock,
    GetPendingSelfIntent(context, AnnouncementClick));
            
                var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);
           
                widgetView.SetOnClickPendingIntent(Resource.Id.widgetBackground, piBackground);
            var piError = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);

            widgetView.SetOnClickPendingIntent(Resource.Id.widgetErrorText1, piError);

        }

        private static PendingIntent GetPendingSelfIntent(Context context, string action)
        {
            
            var intent = new Intent(context, typeof(AppWidget));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.SetAction(action);
            return PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.Immutable);

        }

    }

}

