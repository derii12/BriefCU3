using appslovo;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appslovoM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class popup1 : Rg.Plugins.Popup.Pages.PopupPage
    {
        public popup1()
        {
            InitializeComponent();
        }
        public void logout1(object sender, EventArgs e)
        {
           
            Navigation.PopPopupAsync();
            Preferences.Set("refreshtoken", "logout");
           

        }
        void cancellogout(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}