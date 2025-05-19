using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using static Xamarin.Essentials.Permissions;


namespace appslovo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class confirm : ContentPage
    {
        public int attempts = 7;
        public string public_modulus = "";
        public confirm(string phonenumber)
        {
            RsaEncryptionExample.CreateAsymmetricalKeys();
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            public_modulus = Preferences.Get("current_public_key_modulus", "none");

            confirm_entry.Focus();
            Preferences.Set("widget-click-count", 1);
            image2.Source = ImageSource.FromResource("appslovo.message.png");

            phone_number_label.Text = phonenumber;
            info_label.Text = "Calling you: \n" + phonenumber;
            TryAgainSeconds(phonenumber);
        }

        async void TryAgainSeconds(string phone)
        {
            attempt_button.IsVisible = false;
            int seconds = 46;
            while (seconds > 0)
            {
                call_attempt.Text = "We can call you again in " + (seconds -= 1).ToString() + " seconds";
                await Task.Delay(1000);
            }
            call_attempt.Text = "";
            attempt_button.IsVisible = true;

        }

        async void attempt_request_call(object sender, EventArgs e)
        {
            string phone = phone_number_label.Text;
            TryAgainSeconds(phone);
            string res = await SendRequest(phone);

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
                    res = "Something went wrong when we tried to find a user.";
                }
            }
            catch
            {
                confirm_input_error.Text = "No internet.";
            }
            return res;
        }

        async Task<string> SendRequestConfirmCheck(string phone, string confirm_code, string public_modulus)
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = homepage.web_connection + "Confirmation?phone_number=" + phone + "&confirm_code=" + confirm_code + "&public_modulus=" + public_modulus.Replace('+', '*');

            Uri uri = new Uri(string.Format(Url));

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string inserted_user = await response.Content.ReadAsStringAsync();

                res = inserted_user;

            }
            else
            {
                res = "error";
            }

            return res;
        }
        async void backtolog()
        {
            await Navigation.PushAsync(new PhoneLog(), false);
        }

        async void send_user_confirmation(object sender, EventArgs e)
        {
            var phone_number = phone_number_label.Text.Replace(" ", "");
            var confirm_code = confirm_entry.Text;
            if (confirm_code is null)
            {
                confirm_input_error.Text = "Fill the field.";
            }
            else
            {
                var confirmed_user = await SendRequestConfirmCheck(phone_number, confirm_code, public_modulus.Replace('+', '*'));
                if (confirmed_user == "bad_confirm_code" || confirmed_user == "error")
                {
                    confirm_input_error.Text = "Wrong format.";
                }
                if (confirmed_user == "-1")
                {
                    confirm_input_error.Text = "Wrong code. \n Attempts left: " + (attempts -= 1).ToString();
                }
                if (confirmed_user == "-2")
                {
                    confirm_input_error.Text = "No attempts left.";
                    await Navigation.PushAsync(new PhoneLog(), false);
                }
                if (confirmed_user != "-2" && confirmed_user != "-1" && confirmed_user != "bad_confirm_code" && confirmed_user != "error")
                {
                    Preferences.Set("first_log", true);
                    Preferences.Set("token", confirmed_user.Split(';')[0]);
                    Preferences.Set("refreshtoken", confirmed_user.Split(';')[1]);
                    await Navigation.PushAsync(new homepage(), false);
                }
            }
        }
    }

}