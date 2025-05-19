using appslovo;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appslovoM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class account_deleting : Rg.Plugins.Popup.Pages.PopupPage
    {
        public account_deleting()
        {
            InitializeComponent();
        }

        async Task<string> DelUser(string token) //получение нужной информации о пользователе (на вход подается токен и проверка токена)
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = homepage.web_connection + "delete_user?token=" + token;

            Uri uri = new Uri(string.Format(Url));

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string status = await response.Content.ReadAsStringAsync();

                res = status;

            }
            else 
            {
                res = "Something went wrong while inserting the post.";
            }

            return res;
        }

        async void delete_logout(object sender, EventArgs e)
        {
            var token = Preferences.Get("token", "unlogged");
            string rr = await DelUser(token);
            if (rr == "success")
            {
                await Navigation.PopPopupAsync();
                await Navigation.PopPopupAsync();
                Preferences.Set("refreshtoken", "logout");
            }
        }
        void cancellogout(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}