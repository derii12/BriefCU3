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
    class model
    {
        public string Image { get; set; }
        public string Text { get; set; }
        public string Font { get; set; }
        public string Text1 { get; set; }
        public string marg { get; set; }
        
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class widgettutorial : Rg.Plugins.Popup.Pages.PopupPage
    {

        List<model> content;
        public widgettutorial()
        {
            InitializeComponent();
            content = new List<model>
            {
                 new model{Image ="widgetchange.png", Text ="Change post", Text1="To change friend, which post will display, tap on the username.", Font="22", marg = "0,40,0,0"},
                 new model{Image ="widgetupdate.png", Text ="Update widget", Text1="If timer stopped tap on the background of widget.", Font="22", marg = "0,160,0,0"}
            };
            tutorial.ItemsSource = content;
        }


        void clicked(object sender, EventArgs e)
        {
            if (tutorial.Position == 1)
            {
                Navigation.PopPopupAsync();
            }
            else
            {
                tutorial.Position += 1;
            }
        }
    }
}