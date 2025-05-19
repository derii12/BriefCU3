using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace appslovo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class register : ContentPage
    {
        public register(string phonenumber)
        {
            
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            phone_number_label.Text = phonenumber;



        }

        async Task<string> SendRequestUserInsert(string phone, string username, string invite_code)
        {
            string res = "";
            try
            {
                HttpClient client = new HttpClient();

                string Url = homepage.web_connection + "Registration?phone_number=" + phone + "&username=" + username + "&invitecode=" + invite_code;

                Uri uri = new Uri(string.Format(Url));

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string inserted_user = await response.Content.ReadAsStringAsync();

                    res = inserted_user;

                }
                else
                {
                    res = "Something went wrong while inserting a user";
                }
            }
            catch
            {
                input_errors.Text = "No internet.";
            }
            return res;
        }

        async void go_to_entry_invite(object sender, EventArgs e)
        {
            enrty_invitecode.Focus();
        }
        async void send_user_insertion(object sender, EventArgs e)
        {
            string inserted_user = "";
            var phone_number = phone_number_label.Text;
            var username = enrty_username.Text;
            var invite_code = (enrty_invitecode).Text;
            if (invite_code is null || username is null)
            {
                input_errors.Text = "Fill all the fields.";
            }
            else
            {
                if (username.Length > 2 && invite_code.Length == 10)
                {
                    inserted_user = await SendRequestUserInsert(phone_number, username, invite_code);
                }
                else
                {
                    if (invite_code.Length != 10)
                    {
                        input_errors.Text = "Enter the correct invite code.";
                    }
                    if (username.Length < 3)
                    {
                        input_errors.Text = "Username is too short.";
                    }
                }

                if (inserted_user == "success")
                {
                    await Navigation.PushAsync(new confirm(phone_number), false);
                }
                if (inserted_user == "bad_username")
                {
                    input_errors.Text = "The username must contain only latin letters, digits and '_'";
                }
                if (inserted_user == "-1")
                {
                    input_errors.Text = "This username has alredy taken.";
                }
                if (inserted_user == "-2")
                {
                    input_errors.Text = "This invite code does not exist yet.";
                }
                if (inserted_user == "send_error")
                {
                    input_errors.Text = "Something went wrong while sending data.";
                }
            }

        }
    }
}