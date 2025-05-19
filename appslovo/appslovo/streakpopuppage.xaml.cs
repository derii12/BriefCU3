using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appslovoM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class streakpopuppage : PopupPage
    {
        public streakpopuppage(string streak)
        {
            var streakint = Convert.ToInt32(streak);
            InitializeComponent();
            fire.Source = ImageSource.FromResource("appslovoM.fire.png");
            if (streakint > 1)
            {
                streaklabel.Text = "Your streak is " + streak + "!";
                secondlabel.Text = "You have been posting for " + streak + " days straight.";
            }
            if (streakint == 1)
            {
                streaklabel.Text = "Your streak is " + streak + "!";
                secondlabel.Text = "One day passed since the begining of your streak!";
            }
            if (streakint == 0)
            {
                streaklabel.Text = "Your streak is " + streak + "!";
                secondlabel.Text = "It's just the beginning!";
            }
        }
    }
}