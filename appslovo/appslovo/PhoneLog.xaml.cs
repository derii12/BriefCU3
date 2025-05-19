using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net.Http;
using Xamarin.Essentials;
using Rg.Plugins.Popup.Extensions;
namespace appslovo

{
    public partial class PhoneLog : ContentPage
    {
        public PhoneLog()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            try
            {
                Navigation.PopPopupAsync();
                try
                {
                    Navigation.PopPopupAsync();
                }
                catch { }
            }
            catch
            {

            }
            enrt.Focus();
        }

        async Task<string> SendRequest(string phone)
        {
            string res = "";
            try
            {


                HttpClient client = new HttpClient();

                string Url = homepage.web_connection + "Phone_number_checking?phone_number=" + phone;

                Uri uri = new Uri(string.Format(Url));

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string found_user = await response.Content.ReadAsStringAsync();

                    res = found_user;

                }
                else
                {
                    res = "Something went wrong while finding a user";
                }
            }
            catch
            {
                phone_input_error.Text = "No internet.";
            }
            return res;
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }


        async void getid(object idsender, EventArgs e)
        {



            var text = enrt.Text;
            
                if (text is null)
                {
                    phone_input_error.Text = "Fill the field.";
                }
                else
                {
                string user_with_enered_phone = "";
                    if (text.Length > 5)
                    {
                        var texts = text.Trim(' ', '\n').Replace("+", string.Empty);
                    string phoneres = "";
                    if (texts[0] == '8')
                    {
                        var texts1 = texts.TrimStart('8');
                        phoneres = "7" + texts1;
                    }
                    else
                    {
                        phoneres = texts;
                    }
                    if (texts.Length > 5)
                    {
                   
                            user_with_enered_phone = await SendRequest(phoneres);

                    }
                    else
                    {
                        phone_input_error.Text = "Wrong phone number.";
                    }
                    if (user_with_enered_phone == "found")
                        {
                            phone_input_error.Text = "";
                            await Navigation.PushAsync(new confirm(phoneres), false);
                        }
                        if (user_with_enered_phone == "notfound")
                        {
                            phone_input_error.Text = "";
                            await Navigation.PushAsync(new register(phoneres), false);
                        }
                        if (user_with_enered_phone == "error")
                        {
                            phone_input_error.Text = "Too much call requests,\ntry after 10 minutes.";
                        }
                        if (user_with_enered_phone == "bad_phone_number")
                        {
                            phone_input_error.Text = "Enter the correct phone number";
                        }
                    }
                    else
                    {
                        if (text.Length < 6)
                        {
                            phone_input_error.Text = "Wrong phone number.";
                        }
                    }

                }
        }
    }
}
