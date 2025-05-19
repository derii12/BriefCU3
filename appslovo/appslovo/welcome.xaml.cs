using appslovo;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;       

namespace appslovoM
{
    class contentmodel
    {
        public string Image {  get; set; }
        public string Text { get; set; }    
        public string Font { get; set; }
        public string Text1 { get; set; }
        public string Font1 { get; set; }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class welcome : Rg.Plugins.Popup.Pages.PopupPage
    {
        List<contentmodel> contents;
        public welcome()
        {
            Preferences.Set("first_log", false);
            InitializeComponent();
            contents = new List<contentmodel>
            {
                 new contentmodel{Image ="messageimg.png", Text ="Welcome to Brief", Text1="Here you can share thoughts in the form of short posts that vanish after 24 hours.", Font="22" },
                new contentmodel{Image ="rsaenc.png", Text ="Your posts are\nabsolutely private.", Font="22",  Text1 ="Only you and your friends can read your posts. Nobody including us can`t see them due to end-to-end encryption."},
                new contentmodel{Image ="widgetimg.png", Text ="Widget", Font="22", Text1="You can set up a widget on your desktop to see your friend`s posts on real time!\nGo to ⋮ to learn how to use it."},
                  //new contentmodel{Image ="minifire.png", Text ="Streak", Font="22", Text1="If you post consistently every day your streak increaces."},
                   new contentmodel{Image ="invimg.png", Text ="Invite code", Font="22", Text1="You have a unique invite code, which you can use to invite your friends."},
                     // new contentmodel{Image ="logo.png", Text ="That‘s all, have fun with this app :)", Font="22"}
            };
            welcomeview.ItemsSource = contents;
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        void clicked(object sender, EventArgs e)
        {
            if (welcomeview.Position == 3)
            {
               Navigation.PopPopupAsync();
            }
            else
            {
                welcomeview.Position += 1;
            }
        }
    }
}
