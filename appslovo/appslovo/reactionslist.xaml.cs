using appslovo;

using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace appslovoM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class reactionslist : Rg.Plugins.Popup.Pages.PopupPage
    {
        public string authors = "";
       public static async Task<string> LoadReactions(string token, string post_author) //search
        {
            try
            {
                string res = "";

                HttpClient client = new HttpClient();

                string Url = homepage.web_connection + "load_post_reactions?token=" + token + "&post_author=" + post_author;

                Uri uri = new Uri(string.Format(Url));

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string result_stroke = await response.Content.ReadAsStringAsync();

                    res = result_stroke;

                }
                else
                {
                    res = "error";
                }

                return res;
            }
            catch { return "error"; }
        }





        public reactionslist(string author, string u_author, string time, string post_txt)
        {
            InitializeComponent();
            authors = author;
            user_n.Text = u_author;
          
            user_post_txt.Text = post_txt;
            get_reacts();
        }
           
        async void get_reacts()
        {
            try
            {
                string token = Preferences.Get("token", "unlogged");

                string res = Preferences.Get(authors, "none");
                if (homepage.good_internet)
                {
                    res = await LoadReactions(token, authors);
                }

                if (res != "error" && res != "-1")
                {
                    no_react.IsVisible = false;
                    string[] reacts = res.Split('|');
                    int k = 0;
                    foreach (string act in reacts)
                    {
                        if (act != "")
                        {
                            try
                            {
                                string[] aa = act.Split('•');
                                string react_text_enc = aa[1].Replace('*', '+');
                                string key_encr = aa[3].Replace('*', '+');
                                var key = RsaEncryptionExample.StringDecryption(key_encr);
                                string react_text = SymmetricPostSequirity.DecryptSymmetric(react_text_enc, key);
                                loadreactionlist(aa[0], react_text, aa[2]);
                                k += 1;
                            }
                            catch { }
                        }
                    }
                    if (k == 0)
                    {
                        no_react.IsVisible = true;
                    }
                }
                else
                {
                    no_react.IsVisible = true;
                }
            }
            catch { }
        }

        async void loadreactionlist(string name, string reaction, string datetime)
        {
 
            Frame reactionframe = new Frame()
            {
                Content = new StackLayout()
                {
                    Children =
                                    {
                                        new Label() { Text = $"{name}", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("#D8D8D8"), FontSize = 16, FontFamily = "Inter", CharacterSpacing = 1,},

                                        new Label() { Text = $"{reaction}", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("#A9A9A9"), FontSize = 14, FontFamily = "Inter", CharacterSpacing = 1, },
                                    },
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.Center
                },
                BackgroundColor = Color.FromHex("#252728"),
                CornerRadius = 15,
                HeightRequest = 57,
                WidthRequest = 270,
                Padding = new Thickness(20, 0, 20, 1.5),
                HasShadow = false
            };

            reactionsview.Children.Add(reactionframe);
        }
    
        void close(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
    }
}
