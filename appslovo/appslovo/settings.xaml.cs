
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
    public partial class settings : Rg.Plugins.Popup.Pages.PopupPage
    {
       
        public settings()
        {
            InitializeComponent();
        }
        public async void logoutpopup(System.Object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new appslovoM.popup1());
        }
        public async void widgetpupup(System.Object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new appslovoM.widgettutorial());
        }
        public async void deleteaccount(Object sender, EventArgs e)
        {
            Navigation.PushPopupAsync(new appslovoM.account_deleting());
        }
        public async void infopopup(System.Object sender, System.EventArgs e)
        {
            string main = "";
            string content = "";
            if (sender == about)
            {
                main = "About Brief";
                content = "Quick short text posts that vanish after 24 hours.\nAbsolutely private due to end-to-end encryption.";
                Navigation.PushPopupAsync(new appslovoM.information(main, content,""));
            }
            if (sender == privacy)
            {
               
                     Browser.OpenAsync(new Uri("http://documents.zevent.ru/privacy.html"));
                
            }
            if (sender == contact)
            {
                main = "Contact Developers";
                content = "Send your feedback to:\n";
                Navigation.PushPopupAsync(new appslovoM.information(main, content, "brief@zevent.ru"));
            }
            
        }
        void delcancel(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}