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
    public partial class information : Rg.Plugins.Popup.Pages.PopupPage
    {
       
        public information(string main, string content, string big)
        {
            InitializeComponent();
            if (main == "Privacy policy")
            {
                scroll_content.HeightRequest = 550;
            }
            else
            {
                scroll_content.HeightRequest = 130;
            }
            Main_label.Text = main;
            content_text.Text = content;
            content_text_big.Text = big;
        }
        public void logoutpopup(System.Object sender, System.EventArgs e)
        {
            Navigation.PushPopupAsync(new appslovoM.popup1());
        }
        void delcancel(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}