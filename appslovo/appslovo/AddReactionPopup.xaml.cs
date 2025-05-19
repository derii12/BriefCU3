using appslovo;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appslovoM
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddReactionPopup : Rg.Plugins.Popup.Pages.PopupPage
    {

        async public static Task<string> SendReaction(string token, string reciever, string react) //search
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = homepage.web_connection + "new_post_reaction?token=" + token + "&post_author=" + reciever + "&reaction_txt=" + react;

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

        async static public Task<string> NewPrivateReacts(string token, string keys) //search
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = homepage.web_connection + "new_private_reacts?token=" + token + "&keys=" + keys;

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

        public string authors = "";
        public AddReactionPopup(string author)
        {
            authors = author;
            InitializeComponent();
            focusing();
        }
        async void focusing()
        {
            await Task.Delay(8);
            reaction.Focus();
        }
        void cancel(object sender, EventArgs e)
        {
            Navigation.PopPopupAsync();
        }
        async void send(object sender, EventArgs e)
        {
            try
            {
                string sending_reaction = reaction.Text.Trim();
                ReactSending(sending_reaction, authors);
            }
            catch { }
            Navigation.PopPopupAsync();


        }
        public async static void ReactSending(string sending_reaction, string authorr)
        {
            try
            {
                
                    
                    if (sending_reaction.Length > 0)
                    {
                        string new_encoded_react = Convert.ToBase64String(SymmetricPostSequirity.ToAes256(sending_reaction, "react"));
                        string token = Preferences.Get("token", "unlogged");
                        // var key = RsaEncryptionExample.StringDecryption(aa[3].Replace('*', '+'));
                        await SendReaction(token, authorr, new_encoded_react.Replace('+', '*'));

                        string new_react_decrypter = Preferences.Get("last_react_key", "none");
                   NewPrivateReacts(token, authorr + ";me;" + RsaEncryptionExample.StringEncryption(new_react_decrypter, Preferences.Get("current_public_key_modulus", "none")).Replace('+', '*'));
                    string friends_public_keys_list_result = await homepage.LoadFriendsPublicKeys(token);
                        
                        if (friends_public_keys_list_result != "-1" && friends_public_keys_list_result != "error" && friends_public_keys_list_result != "notfound")
                        {
                            string[] friends_public_keys = friends_public_keys_list_result.Replace("}", "").Replace("{", "").Replace('*', '+').Split(';');



                            for (int i = 0; i < friends_public_keys.Length; i++)
                            {
                                var curr_elem = friends_public_keys[i].Split('•');
                                try
                                {

                                    string friends_public_post_encodes = authorr + ";" + curr_elem[0] + ";" + RsaEncryptionExample.StringEncryption(new_react_decrypter, curr_elem[1]);
                                    NewPrivateReacts(token, friends_public_post_encodes.Replace('+', '*'));
                                }
                                catch
                                {

                                }
                            }



                        }




                    }
                
            }
            catch { }
        }
        public void countsymbreaction(object sender, EventArgs e)
        {
            var countsmb = reaction.Text.Length.ToString() + "/10";
            count.Text = countsmb;
        }
    }
}