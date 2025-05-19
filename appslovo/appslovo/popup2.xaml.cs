using appslovo;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appslovoM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class popup2 : Rg.Plugins.Popup.Pages.PopupPage
    {
        public string username1 = "";
        public popup2(string username)
        {
            InitializeComponent();
            username1 = username;
            delete.Text = "Delete " + username + "?";
        } 
        public void deletefriend(object sender, EventArgs e)
        {
            homepage.Decline_friend_by_name(username1);
            Navigation.PopPopupAsync();
            
        }
        void delcancel(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}