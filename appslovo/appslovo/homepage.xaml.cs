using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;
using System.Threading;
using System.Net.Http;
using Xamarin.CommunityToolkit;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System;
using System.Net;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Http;
using Xamarin.Forms.PlatformConfiguration;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Reflection;
using Rg.Plugins.Popup.Extensions;
using appslovoM;
using PlainEntryAndroidSample.Effects;
using System.Windows.Input;
using System.ComponentModel.Design;
using Xamarin.CommunityToolkit.Effects;
using Org.Apache.Http.Authentication;


namespace appslovo

{

    public class SymmetricPostSequirity
    {
        //private static Random random = new Random();


        private static string RandomString(int length, string k) // generating random string some lenght
        {
            var r = new Random();
            string s = new String(Enumerable.Range(0, length).Select(n => (Char)(r.Next(32, 127))).ToArray());
            if (k != "react")
            {
                Preferences.Set("encode_post_key", s);
            }
            else
            {
                Preferences.Set("last_react_key", s);
            }
            return s;
        }

        private static byte[] GetKey(string key)
        {
            SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        public static string DecryptSymmetric(string shifr, string key)
        {
            return FromAes256(Convert.FromBase64String(shifr), GetKey(key));
        }


        public static string FromAes256(byte[] shifr, byte[] newkey)
        {
            byte[] bytesIv = new byte[16];
            byte[] mess = new byte[shifr.Length - 16];
            //Списываем соль
            for (int i = shifr.Length - 16, j = 0; i < shifr.Length; i++, j++)
                bytesIv[j] = shifr[i];
            //Списываем оставшуюся часть сообщения
            for (int i = 0; i < shifr.Length - 16; i++)
                mess[i] = shifr[i];
            //Объект класса Aes
            Aes aes = Aes.Create();
            //Задаем тот же ключ, что и для шифрования
            aes.Key = newkey;
            //Задаем соль
            aes.IV = bytesIv;
            //Строковая переменная для результата
            string text = "";
            byte[] data = mess;
            ICryptoTransform crypt = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        //Результат записываем в переменную text в вие исходной строки
                        text = sr.ReadToEnd();
                    }
                }
            }
            return text;
        }

        public static byte[] ToAes256(string src, string s)
        {
            //Объявляем объект класса AES
            Aes aes = Aes.Create();
            //Генерируем соль
            aes.GenerateIV();
            //Присваиваем ключ. aeskey - переменная (массив байт), сгенерированная методом GenerateKey() класса AES
            aes.Key = GetKey(RandomString(35, s));
            byte[] encrypted;
            ICryptoTransform crypt = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, crypt, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(src);
                    }
                }
                //Записываем в переменную encrypted зашиврованный поток байтов
                encrypted = ms.ToArray();
            }
            //Возвращаем поток байт + крепим соль
            return encrypted.Concat(aes.IV).ToArray();
        }
    }

    public class RsaEncryptionExample
    {

        public static string StringEncryption(string text, string publicKey)
        {
            return Convert.ToBase64String(EncryptString(text, GetPublicKey(publicKey)));
        }
        public static string StringDecryption(string text)
        {
            return DecryptString(Convert.FromBase64String(text), GetPrivateKey());
        }

        public static RSAParameters GetPublicKey(string public_modulus)
        {
            RSAParameters CurrentPublicKey;

            using (RSA rsa = RSA.Create())
            {

                CurrentPublicKey = new RSAParameters() // секретный ключ, аналогично ему получу и публичный, после запишу байты в строку и передам на сервер:)
                {
                    P = null,
                    D = null,
                    Exponent = Convert.FromBase64String("AQAB"),
                    Q = null,
                    InverseQ = null,
                    Modulus = Convert.FromBase64String(public_modulus),
                    DQ = null,
                    DP = null
                };
            }
            return CurrentPublicKey;
        }

        public static RSAParameters GetPrivateKey()
        {
            RSAParameters CurrentPrivateKey;

            using (RSA rsa = RSA.Create())
            {
                string[] parameters = Preferences.Get("current_private_key", "none").Split(';');
                CurrentPrivateKey = new RSAParameters() // секретный ключ, аналогично ему получу и публичный, после запишу байты в строку и передам на сервер:)
                {
                    P = Convert.FromBase64String(parameters[0]),
                    Q = Convert.FromBase64String(parameters[1]),
                    D = Convert.FromBase64String(parameters[2]),
                    Exponent = Convert.FromBase64String("AQAB"),
                    InverseQ = Convert.FromBase64String(parameters[3]),
                    Modulus = Convert.FromBase64String(parameters[4]),
                    DP = Convert.FromBase64String(parameters[5]),
                    DQ = Convert.FromBase64String(parameters[6])
                };
            }

            return CurrentPrivateKey;
        }



        public static void CreateAsymmetricalKeys()
        {

            using (RSA rsa = RSA.Create())
            {
                RSAParameters mykey;


                RSAParameters publicKey = rsa.ExportParameters(false);
                RSAParameters privateKey = rsa.ExportParameters(true);

                string PublicModulus = Convert.ToBase64String(publicKey.Modulus);

                string PrivateP = Convert.ToBase64String(privateKey.P);
                string PrivateQ = Convert.ToBase64String(privateKey.Q);
                string PrivateD = Convert.ToBase64String(privateKey.D);
                string PrivateInverseQ = Convert.ToBase64String(privateKey.InverseQ);
                string PrivateModulus = Convert.ToBase64String(privateKey.Modulus);
                string PrivateDP = Convert.ToBase64String(privateKey.DP);
                string PrivateDQ = Convert.ToBase64String(privateKey.DQ);


                Preferences.Set("current_public_key_modulus", PublicModulus);
                Preferences.Set("current_private_key", PrivateP + ";" + PrivateQ + ";" + PrivateD + ";" + PrivateInverseQ + ";" + PrivateModulus + ";" + PrivateDP + ";" + PrivateDQ);
            }

        }

        public static byte[] EncryptString(string inputString, RSAParameters publicKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKey);
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
                byte[] encryptedBytes = rsa.Encrypt(inputBytes, RSAEncryptionPadding.OaepSHA1);
                return encryptedBytes;
            }
        }

        public static string DecryptString(byte[] inputBytes, RSAParameters privateKey)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(privateKey);
                byte[] decryptedBytes = rsa.Decrypt(inputBytes, RSAEncryptionPadding.OaepSHA1);
                string decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedData;
            }
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class homepage : CarouselPage

    {
        public static double period = 0;
        public static bool timer_disabled = false;
        public static List<Label> timers = new List<Label>();
        public static List<int> timers_ticks = new List<int>();
        public static List<FlexLayout> react_lst = new List<FlexLayout>();
        public static List<string> post_authors = new List<string>();
        public static List<string> postr_reacts_str_list = new List<string>();
        public static string user_actual_unique = "";
        public static string date_updates = "";
        public static int device_timer = 0;
        public static int velocity = 1000;
        public static bool internet_on = true;
        public static string user_actual_post_txt = "";
        public static bool refresh_updated = false;
        public static bool error_displaying = false;
        public static string user_actual_username = "";
        public static bool activate = true;
        public static string web_connection = "https://slovo-web-changefz8.zevent.ru/Slovo/";
        public Xamarin.Forms.Entry entryactive = null;
        async static Task<string> GetUsername(string token) //получение нужной информации о пользователе (на вход подается токен и проверка токена)
        {
            string res = "";


            HttpClient client = new HttpClient();

            string Url = web_connection + "username_get?token=" + token;

            Uri uri = new Uri(string.Format(Url));

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string username = await response.Content.ReadAsStringAsync();

                res = username;

            }
            else
            {
                res = "Something went wrong while inserting the user)";
            }

            return res;
        }

        async Task<string> NewPost(string token, string post_txt) //получение нужной информации о пользователе (на вход подается токен и проверка токена)
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "new_post?token=" + token + "&post_text=" + post_txt;

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


        async Task<string> SearchUsers(string token, string search_stroke) //search
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "search_user?token=" + token + "&search_stroke=" + search_stroke;

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


        async Task<string> LoadFriends(string token) //отображение друзей
        {
            period = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "load_friends?token=" + token;

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
        public static bool good_internet = true;
        public async void InternetQuality()//регулярная проверка качества интернета и прерывание запросов на сервер, чтобы не вызвать краш приложения.
        {
            if (activate)
            {
                double lat_update = DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - period;
                if (error_displaying is false)
                {
                    if (lat_update > 2 || lat_update == 0)
                    {
                        error_alert_post.FontSize = 15;
                        error_alert_post.Margin = new Thickness(0, -20, 0, 0);
                        error_alert_post.Text = "Updating...";
                        good_internet = false;
                    }
                    else
                    {
                        if (internet_on)
                        {
                            error_alert_post.FontSize = 25;
                            error_alert_post.Margin = new Thickness(5, -27, 0, 0);
                            error_alert_post.Text = "Brief";
                            good_internet = true;
                        }
                        else
                        {
                            error_alert_post.FontSize = 15;
                            error_alert_post.Margin = new Thickness(0, -20, 0, 0);
                            error_alert_post.Text = "Connection...";
                            good_internet = false;
                        }
                    }
                }
                await Task.Delay(1000);
                InternetQuality();
            }
        }


        async static public Task<string> LoadFriendsPosts(string token) //подгрузка постов друзей
        {

            string res = "";


            HttpClient client = new HttpClient();

            string Url = web_connection + "load_friends_posts?token=" + token;

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

      

        async static public Task<string> LoadFriendsPublicKeys(string token) //search
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "load_friends_public_keys?token=" + token;

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



        async public Task<string> LoadUserPublicKey(string personal_code) //search
        {
            string token = Preferences.Get("token", "unlogged");
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "load_user_public_key?token=" + token + "&personal_code=" + personal_code;

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


        async static public Task<string> NewPrivatePosts(string token, string keys) //search
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "new_private_posts?token=" + token + "&keys=" + keys;

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


        async Task<string> LoadFriendsRequests(string token) //search
        {


            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "load_requests?token=" + token;

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

        async Task<string> AddFriend(string token, string username) //
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "add_friend?token=" + token + "&username=" + username;

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

        async public static Task<string> Autentificate_User(string refresh_token) //
        {
            
            try
            {
                string res = "success";

                HttpClient client = new HttpClient();

                string Url = web_connection + "user_autentification?refresh_token=" + refresh_token + "|" + Preferences.Get("device_token", "");
                date_updates = date_updates + "|" + DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                Uri uri = new Uri(string.Format(Url));

                HttpResponseMessage response = await client.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string result_stroke = await response.Content.ReadAsStringAsync();



                    if (result_stroke == "error" || result_stroke == "-1")
                    {

                        
                            Preferences.Set("refreshtoken", "logout");

                            Preferences.Set("token", result_stroke);
                           
                        
                         
                    }
                    else
                    {
                     
                        if (result_stroke.Split(';')[0] != "none")
                        {
                            Preferences.Set("token", result_stroke.Split(';')[0]);
                            Preferences.Set("refreshtoken", result_stroke.Split(';')[1]);
                          

                        }

                    }

                }
                else
                {
                    res = "error";
                }
                
                return res;
            }
            catch
            {
                return "error";
            }
        }

        
        async Task<string> AcceptFriend(string token, string username) //
        {
            string res = "";
          
            HttpClient client = new HttpClient();

            string Url = web_connection + "accept_friend?token=" + token + "&username=" + username;

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
       
        public async static Task<string> DeclineFriend(string token, string username) //
        {
            string res = "";
            

            HttpClient client = new HttpClient();

            string Url = web_connection + "delete_friend?token=" + token + "&username=" + username;

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
       
        async Task<string> EditUsernameSend(string token, string new_username) //
        {
            string res = "";
           
            HttpClient client = new HttpClient();

            string Url = web_connection + "edit_username?token=" + token + "&new_username=" + new_username;

            Uri uri = new Uri(string.Format(Url));

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string result_stroke = await response.Content.ReadAsStringAsync();
                res = result_stroke;
                if (res == "1")
                {
                    user_actual_username = edit_username_entry.Text;

                }
                else
                {
                    error_alert.IsVisible = true;
                    if (res == "bad_username")
                    {

                        if (edit_username_entry.Text.Length < 3)
                        {
                            display_menu_error("The username is too short.");
                            edit_username_entry.Text = user_actual_username;
                        }
                        else
                        {
                            display_menu_error("Wrong formate.");
                            edit_username_entry.Text = user_actual_username;
                        }
                    }
                    else
                    {
                        display_menu_error("That`s your current username.");
                        edit_username_entry.Text = user_actual_username;
                    }
                }
            }
            else
            {
                res = "Error";
            }

            return res;
        }


        async Task<string> LoadUserPost(string token) //
        {
            string res = "";

            HttpClient client = new HttpClient();

            string Url = web_connection + "load_post?token=" + token;

            Uri uri = new Uri(string.Format(Url));

            HttpResponseMessage response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                string result_stroke = await response.Content.ReadAsStringAsync();

                res = result_stroke;

            }
            else
            {
                res = "Error";
            }

            return res;
        }


        async void CreateNewPublicKey(string personal_code)
        {
            string new_public = await LoadUserPublicKey(personal_code);
            string new_post_decrypter = Preferences.Get("encode_post_key", "none");
            string token = Preferences.Get("token", "unlogged");
            string new_key = personal_code + ";" + RsaEncryptionExample.StringEncryption(new_post_decrypter, new_public.Replace('*', '+')).Replace('+', '*');
            string new_keys_for_new_post = await NewPrivatePosts(token, new_key);

        }

        async void Add_friend_by_name(string username)
        {
            var token = Preferences.Get("token", "unlogged");
            var search_result_str = await AddFriend(token, username);
            var search_user = AddFriendEntry.Text;
            Search_user_by_name(search_user);
        }
        async void EditUserPost(string post_txt)
        {
            var token = Preferences.Get("token", "unlogged");
           
            if (post_txt.Split('\n').Length < 8 && Regex.IsMatch(post_txt, @"^[а-яА-Яa-zA-Z0-9“_\n.,!?%\s-\«»„""”=+)('&$#-—–/\:;@><#@'^Ёё*[]+$") || post_txt == "")
            {
                if (post_txt.Length > 3 || post_txt == "")
                {


                    timer_disabled = true;
                    await Task.Delay(1001);
                    string result_str;
                    if (post_txt == "")
                    {
                        result_str = await NewPost(token, "*");
                    }
                    else
                    {
                        string new_encoded_post = "";
                        encrypt_static.Text = "Encryption";
                        new_encoded_post = Convert.ToBase64String(SymmetricPostSequirity.ToAes256(post_txt, "post"));
                        string new_post_decrypter = Preferences.Get("encode_post_key", "none");
                        string friends_public_keys_list_result = await LoadFriendsPublicKeys(token);
                        NewPrivatePosts(token, "me;" + RsaEncryptionExample.StringEncryption(new_post_decrypter, Preferences.Get("current_public_key_modulus", "none")).Replace('+', '*'));
                        if (friends_public_keys_list_result != "-1" && friends_public_keys_list_result != "error" && friends_public_keys_list_result != "notfound")
                        {
                            string[] friends_public_keys = friends_public_keys_list_result.Replace("}", "").Replace("{", "").Replace('*', '+').Split(';');



                            for (int i = 0; i < friends_public_keys.Length; i++)
                            {
                                var curr_elem = friends_public_keys[i].Split('•');
                                try
                                {

                                    string friends_public_post_encodes = curr_elem[0] + ";" + RsaEncryptionExample.StringEncryption(new_post_decrypter, curr_elem[1]);
                                    NewPrivatePosts(token, friends_public_post_encodes.Replace('+', '*'));
                                }
                                catch
                                {

                                }
                            }



                        }

                        result_str = await NewPost(token, new_encoded_post.Replace('+', '*'));
                    }
                    encrypt_static.Text = "";
                    TextPostEntry.IsReadOnly = false;
                    user_actual_post_txt = post_txt;
                    load_post(true, true);
                    load_info(true);
                    // result_str = "nointernet";



                }
                else
                {
                    display_post_error("Your post is too short.");
                    load_post(false, true);
                }
            }
            else
            {
                if (post_txt.Split('\n').Length > 7)
                {
                    display_post_error("Too much break lines!");
                    load_post(false, true);
                }
                else
                {
                    display_post_error("Use only letters, digits and punctuation.");
                    load_post(false, true);
                }

            }
        }

        async void Accept_friend_by_name(string username)
        {
            var token = Preferences.Get("token", "unlogged");
            if (good_internet)
            {
                try
                {
                    var accept_result_str = await AcceptFriend(token, username);
                }
                catch { }
            }
            else
            {
                display_menu_error("No internet connection.");
            }
        }
        protected override bool OnBackButtonPressed()
        {
            return true;
        }
        public static async void Decline_friend_by_name(string username) // функция, вызываемая при подтверждении удаления друга из попапа
        {
            var token = Preferences.Get("token", "unlogged");

            try
            {
                var decline_result_str = await DeclineFriend(token, username);
            }
            catch
            {

            }

        }
        public async void Delete_user_by_name(string username) // функция удаления друга
        {
            if (good_internet)
            {
                try
                {
                    Navigation.PopPopupAsync();
                }
                catch
                {
                    Navigation.PushPopupAsync(new appslovoM.popup2(username)); //вызов какого-то окна, в скобках popup2() можно передать параметр, а именно юзернейм.(он будет доступен на странице попапа) 
                }
            }
            else
            {
                display_menu_error("No internet connection.");
            }
        }

        async void Edit_username(string new_username)
        {
            if (good_internet)
            {
                var token = Preferences.Get("token", "unlogged");
                try
                {
                    var decline_result_str = await EditUsernameSend(token, new_username);
                }
                catch { }
            }
            else
            {
                display_menu_error("No internet connection.");
                edit_username_entry.Text = user_actual_username;
            }
        }

        public async void swaptolog()
        {
            activate = false;
            Preferences.Set("refresh_token", "unlogged");
            Preferences.Set("encode_post_key", "");
            Preferences.Set("token", "unlogged");
            await Navigation.PushAsync(new PhoneLog(), false);

        }

        public void CloseInputs(object sender, EventArgs e)
        {
            edit_username_entry.Unfocus();
            friends_found.IsVisible = false;
            TextPostEntry.Unfocus();
            AddFriendEntry.Unfocus();
        }
        public void OpenEditor(object sender, EventArgs e)
        {

            if (encrypt_static.Text != "Encryption")
            {
                if (sender == TextPostEntry || sender == BlockEntry && post_action_button.Text == "Edit")
                {
                    if (post_action_button.Text == "Edit")
                    {
                        TextPostEntry.Focus();
                    }
                    post_action_button.Text = "Share";

                    counter_static.TextColor = Color.Transparent;


                }


                else
                {
                    string new_post_txt = TextPostEntry.Text.Trim('\n', ' ');
                    if (new_post_txt != user_actual_post_txt || new_post_txt != "" && user_actual_post_txt == "")
                    {
                        if (good_internet)
                        {


                            EditUserPost(new_post_txt);
                        }
                        else
                        {
                            display_post_error("Can`t edit the post without internet.");
                            TextPostEntry.Text = user_actual_post_txt;
                        }
                        TextPostEntry.Unfocus();

                    }
                    else
                    {
                        TextPostEntry.Text = user_actual_post_txt;
                    }

                    counter_static.TextColor = Color.FromHex("#A9A9A9");
                    TextPostEntry.Unfocus();
                    post_action_button.Text = "Edit";


                }

            }
            else
            {
                TextPostEntry.Unfocus();
                TextPostEntry.IsReadOnly = true;
            }
        }
        public async void CopyInvite(object sender, EventArgs e)
        {
            // aaaddd.CurrentPage = aaaddd.Children[0];

            await Clipboard.SetTextAsync(invite_label.Text);
        }

        public async void Edit_escaped(object sender, EventArgs e)
        {
            TextPostEntry.Text = TextPostEntry.Text.Trim('\n', ' ');
            if (TextPostEntry.Text == user_actual_post_txt)
            {
                OpenEditor(BlockEntry, e);
            }
        }





        public async void load_post(bool events, bool appload)
        {


            var token = Preferences.Get("token", "unlogged");
            var user_post_txt = Preferences.Get("curent_user_post", "nointernet"); ;
            try
            {
                if (appload || user_post_txt == "nointernet")
                {

                    user_post_txt = await LoadUserPost(token);

                    Preferences.Set("curent_user_post", user_post_txt);
                }

            }
            catch
            {

            }
            if (user_post_txt != "nointernet" && user_post_txt != "-1" && user_post_txt != "error")
            {



                user_post_txt = user_post_txt.Replace("}", "");
                user_post_txt = user_post_txt.Replace("{", "");
                string[] results = user_post_txt.Split(';');
                try
                {
                    user_actual_post_txt = SymmetricPostSequirity.DecryptSymmetric(results[0].Replace('*', '+'), Preferences.Get("encode_post_key", "none"));
                }
                catch
                {
                    user_actual_post_txt = "~";
                    if (results[0] == "")
                    {
                        user_actual_post_txt = "";
                    }

                }
                if (events || user_actual_post_txt == "~")
                {
                    if (results[0] == "" || user_actual_post_txt == "~" || user_actual_post_txt == "-1" || user_actual_post_txt == "" || results[0] == "*")
                    {
                        if (results[0] == "" || user_actual_post_txt == "" || results[0] == "*")
                        {
                            TextPostEntry.PlaceholderColor = Color.FromHex("#A9A9A9");
                            TextPostEntry.Placeholder = "Share a post";
                            TextPostEntry.Text = "";
                            timer_disabled = true;
                            counter_static.Text = "";

                            symbcount.Text = "0/300";
                        }
                        else
                        {
                            TextPostEntry.PlaceholderColor = Color.FromHex("#A9A9A9");
                            TextPostEntry.Text = "";
                            TextPostEntry.Placeholder = "Can`t read your previous post";
                            timer_disabled = true;
                            counter_static.Text = "";

                            symbcount.Text = "0/300";
                        }
                    }
                    else
                    {
                        timer_disabled = true;

                        await Task.Delay(1500);
                        TextPostEntry.Text = user_actual_post_txt;
                        TextPostEntry.Placeholder = "Share a post";
                        timer_disabled = false;
                        PostTimer(Convert.ToInt32(results[1]));
                        var countsmb = TextPostEntry.Text.Length.ToString() + "/300";

                        symbcount.Text = countsmb;
                    }

                }
                else
                {

                    timer_disabled = false;
                    TextPostEntry.Text = user_actual_post_txt;
                }
            }
            else
            {
                TextPostEntry.PlaceholderColor = Color.FromHex("#A9A9A9");
                TextPostEntry.Placeholder = "Share a post";
                user_actual_post_txt = "";
                TextPostEntry.Text = "";
                counter_static.Text = "";
                timer_disabled = true;
                symbcount.Text = "0/300";
            }
            if (appload is false)
            {
                load_post(true, true);
            }


        }
     
        async void Load_User_Requests(string previous_result, bool appload)
        {
            var token = Preferences.Get("token", "unlogged");
            string friends_result_str = Preferences.Get("user_requests", "nointernet"); ;
            try
            {
                if (appload)
                {
                    friends_result_str = await LoadFriendsRequests(token);
                    Preferences.Set("user_requests", friends_result_str);
                    //     status_bar.Text = "";
                }
            }
            catch
            {

            }
            friends_result_str = friends_result_str.Replace("}", "");
            friends_result_str = friends_result_str.Replace("{", "");
            if (friends_result_str != "notfound" && friends_result_str != "error" && friends_result_str != previous_result && friends_result_str != "nointernet")
            {
                Search_user_by_name(AddFriendEntry.Text);
                RequestText.IsVisible = true;
                RequestsResultStack.Children.Clear();
                string[] results = friends_result_str.Split(';');

                StackLayout LoadRequestsLayout = RequestsResultStack;
                Frame FrameLoad = null;

                foreach (var username in results)
                {
                    string[] user_params = username.Split('•');
                    var name = user_params[0];
                    var personal_code = user_params[1];
                    Frame DeclineFriend = new Frame()
                    {
                        Content = new StackLayout()
                        {
                            Children = {
                                            new Label() { Text="Decline", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions= LayoutOptions.CenterAndExpand, TextColor=Color.FromHex("#ED4A34"), FontSize=15, FontFamily="Inter", CharacterSpacing= 0}
                                        },
                            Padding = new Thickness(0, 0, 0, 0)
                        },
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        Padding = new Thickness(2, 18, 6, 20),
                        HasShadow = false,
                        BackgroundColor = Color.FromHex("#252728")
                    };
                    Frame AcceptFriend = new Frame()
                    {
                        Content = new StackLayout()
                        {
                            Children = {
                                            new Label() { Text="Accept", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions= LayoutOptions.CenterAndExpand, TextColor=Color.FromHex("#596AC7"), FontSize=15, FontFamily="Inter", CharacterSpacing= 0}
                                        },
                            Padding = new Thickness(0, 0, 0, 0)
                        },
                        HorizontalOptions = LayoutOptions.End,
                        Padding = new Thickness(2, 18, 0, 20),
                        HasShadow = false,
                        BackgroundColor = Color.FromHex("#252728")


                    };
                    FrameLoad = new Frame()
                    {
                        Content = new StackLayout()
                        {
                            Children =
                            {
                                new Label() { Text = $"{name}", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions= LayoutOptions.CenterAndExpand, TextColor=Color.FromHex("#D8D8D8"), FontSize=18, FontFamily="Inter", CharacterSpacing=1},
                               DeclineFriend,
                               AcceptFriend
                            },
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Center
                        },
                        BackgroundColor = Color.FromHex("#252728"),
                        CornerRadius = 15,
                        HeightRequest = 57,
                        Padding = new Thickness(20, 0, 20, 1.5),
                        HasShadow = false

                    };
                    var AcceptRecognizer = new TapGestureRecognizer();
                    AcceptRecognizer.Tapped += (s, e) =>
                    {
                        if (entryactive == null)
                        {
                            var accept_friend_username = name;
                            CreateNewPublicKey(personal_code);
                            Accept_friend_by_name(accept_friend_username);
                        }
                        else
                        {
                            entryactive.Focus();
                        }
                    };
                    AcceptFriend.GestureRecognizers.Add(AcceptRecognizer);

                    var DeclineRecognizer = new TapGestureRecognizer();
                    DeclineRecognizer.Tapped += (s, e) =>
                    {
                        if (entryactive == null)
                        {
                            var accept_friend_username = name;
                            if (good_internet)
                            {
                                Decline_friend_by_name(accept_friend_username);
                            }
                            else
                            {
                                display_menu_error("No internet connection.");
                            }
                        }
                        else
                        {
                            entryactive.Focus();
                        }
                    };
                    DeclineFriend.GestureRecognizers.Add(DeclineRecognizer);


                    LoadRequestsLayout.Children.Add(FrameLoad);
                }
                RequestsResultStack = LoadRequestsLayout;
            }
            else
            {

                if (friends_result_str == "notfound" || friends_result_str == "error")
                {

                    RequestText.IsVisible = false;
                    RequestsResultStack.Children.Clear();

                    
                    
                }
            }
            if (activate)
            {
                await Task.Delay(1111);

                Load_User_Requests(friends_result_str, true);
            }
        }



        public static async void replay_autentific()
        {


             autentification();


            if (activate)
            {
                await Task.Delay(590000);
                replay_autentific();
            }
        }




        
        async void Load_User_Friends_Posts(string previous_result, bool appload, bool success_load)
        {

            var token = Preferences.Get("token", "unlogged");
            string posts_result_str = "nointernet";
            if (success_load)
            {
                posts_result_str = Preferences.Get("user_friends_posts", "nointernet");
            }
            else
            {
                if (good_internet)
                {

                    posts_result_str = await LoadFriendsPosts(token);
                }
            }
            try
            {
                if (Preferences.Get("refreshtoken","unlogged") == "logout")
                {
             //  DisplayAlert("logout", date_updates, "ok");
                
               //     DisplayAlert("tokens1", Preferences.Get("refreshtoken", "unlogged"), "ok");
                 //  DisplayAlert("tokens2", Preferences.Get("token", "unlogged"), "ok");
                    swaptolog();
                }
            }
            catch { }
            try
            {

                if (appload || posts_result_str == "nointernet")
                {
                    posts_result_str = await LoadFriendsPosts(token);
                    Preferences.Set("user_friends_posts", posts_result_str);
                    if (internet_on == false) //после отключения снова появился интернет 

                    {
                        edit_username_entry.IsReadOnly = false;
                        internet_on = true;
                        timer_disabled = true;
                        await Task.Delay(1500);
                        load_post(true, true);
                    }
                }
                
            }
            catch
            {
                edit_username_entry.IsReadOnly = true;
                if (action_username_button.Text == "Save")
                {
                    edit_username_entry.Unfocus();
                    OpenUsernameEdit(action_username_button, null);
                }
                internet_on = false;
            }
            try
            {
                if(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - Preferences.Get("last_autentification_id", 0.0) > 2)
                {
                    idinfier();
                }
                if (posts_result_str != "notfound" && posts_result_str != "error" && posts_result_str != "nointernet")
                {


                    string[] post_info_list = posts_result_str.Split('•');
                    string[] post_info_list_time = post_info_list[1].Split('|');
                    string[] results_info_list = post_info_list[0].Split('~');
                    int server_time = Convert.ToInt32(post_info_list_time[0]);
                    if ((server_time - device_timer > 3 && server_time - device_timer < 8) || (device_timer - server_time > 3 && device_timer - server_time < 8))
                    {
                        if (server_time - device_timer > 3)
                        {
                            velocity = 1500;
                        }
                        if (device_timer - server_time > 3)
                        {
                            velocity = 500;
                            //DisplayAlert("fast", "", "ok");
                        }
                    }
                    else
                    {
                        velocity = 1000;
                    }
                    if (post_info_list[0] != previous_result || server_time - device_timer > 8 || device_timer - server_time > 8)
                    {
                        List<Frame> allposts = new List<Frame>();
                        postr_reacts_str_list.Clear();
                        post_authors.Clear();
                        timers.Clear();
                        react_lst.Clear();
                        string[] author_unique = post_info_list[2].Split('|');
                        timers_ticks.Clear();
                        // DisplayAlert("go", "go", "up");
                        device_timer = Convert.ToInt32(post_info_list_time[0]) - 1;
                        PostsResultStack.Children.Clear();
                        bool noposts = true;
                        StackLayout FrameLoad = null;
                        for (int i = 0; i < results_info_list.Length; i++)
                        {
                            bool decrypted = false;
                            var author_code = author_unique[i + 1].ToString();
                            post_authors.Add(author_code);
                            bool correct_post = false;
                            string posttxt = "Can`t read this post :( \nAsk your friend to post it again.";
                            var text_info = results_info_list[i].Split('|');
                            var time_info = Convert.ToInt32(post_info_list_time[i]); // количество секунд до удаления
                            List<Frame> reatcs_lst = new List<Frame>();
                            var posttxt_encrypted = text_info[0].Replace('*', '+'); // текст поста
                            var postauthor = text_info[1]; // автор поста
                            var streak = text_info[2];
                            var key_encrypted = text_info[3].Replace('*', '+');
                            var post_textcolor = Color.FromHex("#D8D8D8");
                            StackLayout txt_cont = new StackLayout()
                            {
                                Padding = new Thickness(10, 6, 6, 0),
                                Spacing = 1
                            };
                           
                            FlexLayout Reacts_layout = new FlexLayout()

                            {
                                Padding = new Thickness(0, 0, 0, 0),
                                Direction = FlexDirection.Row,
                                Wrap = FlexWrap.Wrap
                            };
                            react_lst.Add(Reacts_layout);


                            var imagestr = new Image()
                            {
                                Aspect = Aspect.AspectFit,
                                Source = "fire.png",
                                HeightRequest = 16,
                                WidthRequest = 16,
                                Margin = new Thickness(7,0,0,0)
                            };






                            var post_textsize = 19;
                            Label this_post_timer = new Label() { Text = $"{TimeChecker(time_info)}", FontFamily = "Inter", FontSize = 12, Margin = new Thickness(0, 0, 5, 0), Padding = new Thickness(0, 8, 8, 8), HorizontalOptions = LayoutOptions.EndAndExpand, TextColor = Color.FromHex("#A9A9A9") };
                            try
                            {
                                var key = RsaEncryptionExample.StringDecryption(key_encrypted);
                                posttxt = SymmetricPostSequirity.DecryptSymmetric(posttxt_encrypted, key);
                                if (posttxt.Split('\n').Length < 8 && Regex.IsMatch(posttxt, @"^[а-яА-Яa-zA-Z0-9“_\n.,!?%\s-\«»„""”=+)('&$#-—–/\:;@><#@'^Ёё*[]+$") && posttxt != "")
                                {
                                    decrypted = true;
                                    correct_post = true;
                                    timers_ticks.Add(time_info);
                                    var text_wr = posttxt.Split('\n');
                                    foreach (var elem in text_wr)
                                    {
                                        FormattedString text_wrap = new FormattedString();
                                        foreach (var ss in elem.Split(' '))
                                        {
                                            if (Uri.IsWellFormedUriString(ss, UriKind.Absolute))
                                            {
                                               


                                                var new_l = new Span() { Text = $"{ss}" + " ", TextDecorations=TextDecorations.Underline, FontSize = post_textsize, FontFamily = "Inter", TextColor = post_textcolor };
                                                new_l.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(async () => Browser.OpenAsync(new Uri(ss))) });

                                                text_wrap.Spans.Add(new_l);
                                            }
                                            else
                                            {
                                                var new_l = new Span() { Text = $"{ss}" + " ", FontSize = post_textsize, FontFamily = "Inter", TextColor = post_textcolor };
                                                text_wrap.Spans.Add(new_l);
                                            }
                                            
                                        }
                                        txt_cont.Children.Add(new Label { FormattedText = text_wrap });
                                       
                                        
                                    }
                                }
                            }
                            catch
                            {
                                
                                timers_ticks.Add(time_info);
                                correct_post = true;
                                post_textcolor = Color.FromHex("#A9A9A9");
                                post_textsize = 15;
                                Label error_txt_decrypt = new Label() { Text = $"{posttxt}", FontSize = post_textsize, FontFamily = "Inter", Margin = new Thickness(10, 5, 10, 0), TextColor = post_textcolor };
                                txt_cont.Children.Add(error_txt_decrypt);
                            }
                            if (correct_post)
                            {
                                noposts = false;

                                FrameLoad = new StackLayout()
                                {

                                    Children = {
                            new Frame()
                            {
                                Content = new StackLayout()
                                {
                                    Children =
                                    {
                                        new Frame()
                                        {
                                            Content = new StackLayout()
                                            {
                                                Children =
                                                {
                                                    new Frame()
                                                    {
                                                        Content = new Frame()
                                                        {
                                                            Content = new StackLayout()
                                                            {
                                                                Children =
                                                                {
                                                                    new Frame()
                                                                    {
                                                                        Content = new Frame()
                                                                        {
                                                                            Content = new Label() { Text = $"{postauthor}", FontFamily = "Inter", FontSize = 14, Margin = new Thickness(10, 0, 10, 2), VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("#A9A9A9") },
                                                                            BackgroundColor = Color.FromHex("#161718"),
                                                                            CornerRadius = 15,
                                                                            Padding = new Thickness(2, 0, 2, 0),
                                                                            HasShadow = false,
                                                                            IsClippedToBounds = true
                                                                        },
                                                                        BackgroundColor = Color.FromHex("#252728"),
                                                                        CornerRadius = 18,
                                                                        Padding = new Thickness(3.5,4,3.5,3.5),
                                                                        Margin = new Thickness(-3.5),
                                                                        HasShadow = false,
                                                                        HeightRequest = 17,
                                                                        IsClippedToBounds = true
                                                                    },
                                                                    imagestr,
                                                                    new Label() { Text = $"{streak}", FontFamily = "Inter", FontSize = 16, Margin = new Thickness(0, -0.3, 13, 0), HorizontalOptions = LayoutOptions.Center, TextColor = Color.FromHex("#596AC7"), Padding = new Thickness(0), VerticalOptions = LayoutOptions.Center },
                                                                                                                                       
                                                                },
                                                                Orientation = StackOrientation.Horizontal
                                                            },
                                                            BackgroundColor = Color.FromHex("#161718"),
                                                            CornerRadius = 15,
                                                            Padding = new Thickness(0),
                                                            HasShadow = false,
                                                            IsClippedToBounds = false
                                                        },
                                                        BackgroundColor = Color.FromHex("#252728"),
                                                        CornerRadius = 18,
                                                        Padding = new Thickness(3.5),
                                                        Margin = new Thickness(-3, -3.5, 0, -3.5),
                                                        HasShadow = false,
                                                        HeightRequest = 17,
                                                        IsClippedToBounds = false
                                                    },
                                                    this_post_timer
                                                },
                                                Orientation = StackOrientation.Horizontal
                                            },
                                            BackgroundColor = Color.FromHex("#161718"),
                                            CornerRadius = 14,
                                            Padding = new Thickness(0, 0, 0, 0.5),
                                            HasShadow = false

                                        },
                                      //  
                                      
                                    txt_cont
                                  
                                    },
                                    VerticalOptions = LayoutOptions.Center
                                },
                                BackgroundColor = Color.FromHex("#252728"),
                                CornerRadius = 20,
                                Padding = new Thickness(7, 7, 8, 10),
                                Margin = new Thickness(0, 0, 0, 0),
                                MinimumHeightRequest = 100,
                                HasShadow = false,


                            },  new Frame()
                                        {
                                            Content = Reacts_layout,

                                            BackgroundColor = Color.Transparent,
                                            Padding = new Thickness(0, 0, 0, 0),
                                            Margin=new Thickness(7, 0, 0, 0),

                                        }
                            }


                                };
                                Frame FameLoadGlobal = new Frame()
                                {
                                    Content = FrameLoad,
                                    BackgroundColor = Color.FromHex("#252728"),
                                    CornerRadius = 20,
                                    Padding = new Thickness(0, 0, 0, 5),
                                    Margin = new Thickness(0, 0, 0, 5)
                                };



                                ICommand LongPressCommand = new Command(() =>
                                {

                                    var duration = TimeSpan.FromMilliseconds(40);
                                    Vibration.Vibrate(duration);
                                    if (decrypted)
                                    {
                                        Navigation.PushPopupAsync(new appslovoM.reactionslist(author_code, postauthor, this_post_timer.Text, posttxt));
                                    }
                                    else
                                    {
                                        Navigation.PushPopupAsync(new appslovoM.reactionslist(author_code, postauthor, this_post_timer.Text, "~"));
                                    }

                                });

                                TouchEffect.SetLongPressCommand(FrameLoad, LongPressCommand);


                                timers.Add(this_post_timer);
                                postr_reacts_str_list.Add("none");
                                allposts.Add(FameLoadGlobal);
                            }
                        }
                        foreach (var x in allposts)
                        {
                            PostsResultStack.Children.Add(x);
                        }
                        if (noposts)
                        {
                            PostsResultStack.Children.Clear();
                            Label no_posts = new Label() { Text = "Wow... So empty...", TextColor = Color.FromHex("#4f4f4f"), VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Center, FontSize = 18 };
                            PostsResultStack.Children.Add(no_posts);
                        }

                    }

                }
                else
                {
                    if (posts_result_str == "notfound" || posts_result_str == "error")
                    {
                        PostsResultStack.Children.Clear();
                        Label no_posts = new Label() { Text = "Wow... So empty...", TextColor = Color.FromHex("#4f4f4f"), VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Center, FontSize = 18 };
                        PostsResultStack.Children.Add(no_posts);
                    }
                }

                await Task.Delay(2500);
                if (activate)
                {
                    if (appload)
                    {
                        Load_User_Friends_Posts(posts_result_str.Split('•')[0], true, true);
                    }
                    else
                    {
                        Load_User_Friends_Posts("", true, true);
                    }
                }
            }
            catch
            {
                await Task.Delay(2500);
                if (activate)
                {
                    Load_User_Friends_Posts("", true, false);
                }
            }
        }


        async void openlist(object sender, EventArgs e)
        {
            if (user_actual_post_txt != "" && user_actual_post_txt != "*")
            {
                Navigation.PushPopupAsync(new appslovoM.reactionslist(user_actual_unique, user_actual_username, counter_static.Text, user_actual_post_txt));
            }
        }

        public class ReactMini
        {
            public int num { get; set; }
            public string react_txt { get; set; }
            public int react_count { get; set; }
            public bool react_stat { get; set; } //false means that is reaction by current user
        }

        public async void UpdateReact()
        {

            string token = Preferences.Get("token", "unlogged");
            try
            {
                if (good_internet)
                {

                    string my_reacts = await reactionslist.LoadReactions(token, user_actual_unique);
                    string k = "0";
                    if (my_reacts != "error" && my_reacts != "-1")
                    {
                       k = my_reacts.Trim('|').Split('|').Length.ToString();
                    }
                    
                    if (k != my_react_count.Text)
                    {

                            my_react_count.Text = k;
                        
                    }
                }
            
            for (int i = 0; i < post_authors.Count; i++)
            {
                string author = post_authors[i];
                string res = Preferences.Get(author, "none");
                try
                {
                    if (good_internet)
                    {
                        res = await reactionslist.LoadReactions(token, post_authors[i]);
                        Preferences.Set(author, res);
                    }
                }
                catch { }
                if (postr_reacts_str_list[i] != res)
                {
                    List<Frame> new_reacts = new List<Frame>();
                    List<ReactMini> new_reacts_mini = new List<ReactMini>();

                    string[] reacts_mini = res.Trim('|').Split('|');
                    foreach (string minreact in reacts_mini)
                    {
                        try
                        {

                            string[] react_info = minreact.Split('•');
                            string react_text_enc = react_info[1].Replace('*', '+');
                            string key_encr = react_info[3].Replace('*', '+');
                            var key = RsaEncryptionExample.StringDecryption(key_encr);
                            string react_text = SymmetricPostSequirity.DecryptSymmetric(react_text_enc, key);
                            string new_txt = react_text;
                            int new_count = 1;
                            bool new_stat = true;
                            if (react_info[4] == user_actual_unique)
                            {
                                new_stat = false; // this react is by current user
                            }
                            foreach (var rct_min in new_reacts_mini)
                            {
                                if (rct_min.react_txt == new_txt)
                                {
                                    rct_min.react_count++; // increasing numb of reacts
                                    rct_min.react_stat = rct_min.react_stat && new_stat; // if it is react by user it became false
                                    new_count -= 1;
                                    break;
                                }
                            }
                            if (new_count == 1)
                            {
                                ReactMini new_min = new ReactMini();
                                new_min.num = new_reacts_mini.Count();
                                new_min.react_txt = new_txt;
                                new_min.react_count = new_count;
                                new_min.react_stat = new_stat;
                                new_reacts_mini.Add(new_min);
                            }
                        }
                        catch { }
                    }



                    ICommand AddPressCommand = new Command(() =>
                    {
                        if (good_internet)
                        {
                            var duration = TimeSpan.FromMilliseconds(30);
                            Vibration.Vibrate(duration);

                            Navigation.PushPopupAsync(new appslovoM.AddReactionPopup(author));
                        }
                    });
                    Frame AddReaction = new Frame()

                    {
                        Content = new Label() { Text = "+", FontSize = 20, FontFamily = "Inter", TextColor = Color.FromHex("#596AC7"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand },
                        Padding = new Thickness(10, 0, 10, 0),
                        HeightRequest = 30,
                        BackgroundColor = Color.FromHex("#161718"),
                        CornerRadius = 20,
                        Margin = new Thickness(2.5, 2.5, 2.5, 2.5),
                        HasShadow = false,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        IsClippedToBounds = false
                    };
                    TouchEffect.SetCommand(AddReaction, AddPressCommand);
                    TouchEffect.SetPressedScale(AddReaction, 0.85);




                    foreach (var react in new_reacts_mini)
                    {
                        try
                        {
                            if (react.react_stat)
                            {
                                Frame reaction = new Frame()

                                {
                                    Content = new StackLayout()
                                    {
                                        Children =
                                                {

                                                    new Label() {Text=react.react_txt,  FontSize=14, FontFamily="Inter", TextColor= Color.FromHex("#A9A9A9"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand},

                                                    new Label() {Text=react.react_count.ToString(),  FontSize=13, FontFamily="Inter", TextColor= Color.FromHex("#A9A9A9"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand},
                                                },
                                        Orientation = StackOrientation.Horizontal
                                    },
                                    Padding = new Thickness(10, -2, 10, 0),
                                    HeightRequest = 30,
                                    BackgroundColor = Color.FromHex("#343739"),
                                    CornerRadius = 20,
                                    Margin = new Thickness(2.5, 2.5, 2.5, 2.5),
                                    HasShadow = false,
                                    VerticalOptions = LayoutOptions.CenterAndExpand,
                                    IsClippedToBounds = false
                                };
                                TouchEffect.SetPressedScale(reaction, 1.08);
                                ICommand ReactPressCommand = new Command(() =>
                                {
                                    if (good_internet)
                                    {
                                        var duration = TimeSpan.FromMilliseconds(20);
                                        Vibration.Vibrate(duration);
                                        AddReactionPopup.ReactSending(react.react_txt, author);
                                        UpdateReact();
                                    }
                                });
                                TouchEffect.SetCommand(reaction, ReactPressCommand);
                                new_reacts.Add(reaction);
                            }
                            else
                            {
                                Frame active_reaction = new Frame()

                                {
                                    Content = new StackLayout()
                                    {
                                        Children =
                                                            {
                                                                new Label() { Text = react.react_txt, FontSize = 14, FontFamily = "Inter", TextColor = Color.FromHex("#161718"), FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand },
                                                                new Label() { Text = react.react_count.ToString(), FontSize = 13, FontFamily = "Inter", TextColor = Color.FromHex("#161718"), HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand },
                                                            },
                                        Orientation = StackOrientation.Horizontal
                                    },
                                    Padding = new Thickness(10, -2, 10, 0),
                                    HeightRequest = 30,
                                    BackgroundColor = Color.FromHex("#6b7ddb"),

                                    CornerRadius = 20,
                                    Margin = new Thickness(2.5, 2.5, 2.5, 2.5),
                                    HasShadow = false,
                                    VerticalOptions = LayoutOptions.CenterAndExpand,
                                    IsClippedToBounds = false
                                };
                                TouchEffect.SetPressedScale(active_reaction, 0.95);
                                ICommand UnReactPressCommand = new Command(() =>
                                {
                                    if (good_internet)
                                    {
                                        var duration = TimeSpan.FromMilliseconds(30);
                                        Vibration.Vibrate(duration);
                                        AddReactionPopup.SendReaction(token, author, "*");
                                        UpdateReact();
                                    }
                                });
                                TouchEffect.SetCommand(active_reaction, UnReactPressCommand);

                                new_reacts.Add(active_reaction);
                            }


                        }
                        catch
                        {

                        }
                    }
                    react_lst[i].Children.Clear();

                    foreach (var r in new_reacts)
                    {
                        react_lst[i].Children.Add(r);
                    }
                    react_lst[i].Children.Add(AddReaction);
                    postr_reacts_str_list[i] = res;

                }
            }
            }
            catch { }
        }

        async void CircleUpdateReact()
        {
            UpdateReact();
            await Task.Delay(500);
            if (activate)
            {
                CircleUpdateReact();
            }
        }








        async void Load_User_Friends(string previous_result, bool appload)
        {

            var token = Preferences.Get("token", "unlogged");
            string friends_result_str = Preferences.Get("user_friends", "nointernet");
            try
            {
                if (appload || friends_result_str == "nointernet")
                {
                    friends_result_str = await LoadFriends(token);
                    Preferences.Set("user_friends", friends_result_str);
                }
                if (user_actual_unique == "")
                {
                    await Task.Delay(2000);
                    if (activate)
                    {

                        load_info(true);
                    }
                }
            }
            catch { }


            friends_result_str = friends_result_str.Replace("}", "");
            friends_result_str = friends_result_str.Replace("{", "");
            if (friends_result_str != "notfound" && friends_result_str != "error" && friends_result_str != previous_result && friends_result_str != "nointernet")
            {
                Search_user_by_name(AddFriendEntry.Text);
                var waiting_count = 0;

                FriendsResultStack.Children.Clear();
                WaitingResultStack.Children.Clear();
                string[] results = friends_result_str.Split(';');

                StackLayout LoadFriendsLayout = FriendsResultStack;
                StackLayout LoadWaitingLayout = WaitingResultStack;
                Frame FrameLoad = null;
                int k = 0;
                foreach (var username in results)
                {

                    string[] t_username = username.Split('•');
                    string curr_username = t_username[0];
                    string curr_status = t_username[1];
                    string deleting_txt = "Delete";
                    Color deleting_color = Color.FromHex("#ED4A34");
                    if (curr_status == "0")
                    {
                        deleting_color = Color.FromHex("#a9a9a9");
                        deleting_txt = "Cancel";
                    }
                    Frame DeleteFriend = new Frame()
                    {
                        Content = new Label() { Text = $"{deleting_txt}", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = deleting_color, FontSize = 15, FontFamily = "Inter", CharacterSpacing = 0 }

                        ,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        Padding = new Thickness(0, 15, 0, 15),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HasShadow = false,
                        BackgroundColor = Color.FromHex("#252728")
                    };
                    FrameLoad = new Frame()
                    {
                        Content = new StackLayout()
                        {
                            Children =
                            {
                                new Label() { Text = $"{curr_username}", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions= LayoutOptions.CenterAndExpand, TextColor=Color.FromHex("#D8D8D8"), FontSize=18, FontFamily="Inter", CharacterSpacing=1},
                                DeleteFriend
                            },
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Center
                        },
                        BackgroundColor = Color.FromHex("#252728"),
                        CornerRadius = 15,
                        HeightRequest = 55,
                        Padding = new Thickness(20, 1.5, 20, 1.5),
                        HasShadow = false,
                        Margin = new Thickness(0, 0, 0, 2)

                    };
                    var DeleteRecognizer = new TapGestureRecognizer();
                    DeleteRecognizer.Tapped += (s, e) =>
                    {
                        if (entryactive == null)
                        {
                            var delete_friend_username = curr_username;
                            if (curr_status == "1")
                            {
                                Delete_user_by_name(delete_friend_username);//oii
                            }
                            else
                            {
                                Decline_friend_by_name(delete_friend_username);
                            }
                        }
                        else
                        {
                            entryactive.Focus();
                        }
                    };

                    DeleteFriend.GestureRecognizers.Add(DeleteRecognizer);
                    if (curr_status == "0")
                    {
                        waiting_count += 1;
                        WaitingText.IsVisible = true;
                        LoadWaitingLayout.Children.Add(FrameLoad);
                    }
                    else
                    {
                        FriendsText.IsVisible = true;
                        LoadFriendsLayout.Children.Add(FrameLoad);
                        k += 1;
                    }
                }
                if (waiting_count == 0)
                {
                    WaitingText.IsVisible = false;
                    WaitingResultStack.Children.Clear();
                }
                if (k == 0)
                {
                    FriendsText.IsVisible = false;
                    FriendsResultStack.Children.Clear();
                }
                friends_count.Text = k.ToString();
                FriendsResultStack = LoadFriendsLayout;
                WaitingResultStack = LoadWaitingLayout;
            }
            else
            {
                if (friends_result_str == "error")
                {
                    string ressf = Preferences.Get("refreshtoken", "none");
                    if (activate)
                    {

                        autentification();
                        await Task.Delay(2000);
                        load_info(true);
                        load_post(true, true);
                    }
                }
                if (user_actual_post_txt == "-")
                {
                    load_info(true);
                    load_post(true,true);
                }
                if ((friends_result_str == "notfound") && friends_result_str != previous_result)
                {

                    
                        Search_user_by_name(AddFriendEntry.Text);
                    
                    WaitingText.IsVisible = false;
                    FriendsText.IsVisible = false;
                    FriendsResultStack.Children.Clear();
                    WaitingResultStack.Children.Clear();
                }
                
            }
            await Task.Delay(1111);
            if (activate)
            {
                Load_User_Friends(friends_result_str, true);
            }
        }

        async void Search_user_by_name(string search_stroke)
        {
            var token = Preferences.Get("token", "unlogged");
            string search_result_str = "error";
            try
            {
                search_result_str = await SearchUsers(token, search_stroke);
            }
            catch { }
            SearchResultStack.Children.Clear();
            if (search_result_str != "notfound" && search_result_str != "error")
            {
                friends_found.IsVisible = false;
                search_result_str = search_result_str.Replace("}", "");
                search_result_str = search_result_str.Replace("{", "");
                string[] results = search_result_str.Split(';');

                StackLayout LoadSearchRes = SearchResultStack;
                Frame FrameLoad = null;

                foreach (var username in results)
                {
                    Label labelstatus = new Label();
                    string[] fr_info = username.Split('|');
                    string friend_status = fr_info[1];
                    string friend_name = fr_info[0];
                    string friend_personal = fr_info[2];
                    if (friend_status == "0")
                    {
                        labelstatus = new Label() { Text = "Cancel", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("#a9a9a9"), FontSize = 15, FontFamily = "Inter", CharacterSpacing = 1 };
                    }
                    else
                    {
                        labelstatus = new Label() { Text = "Add", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = Color.FromHex("#596AC7"), FontSize = 15, FontFamily = "Inter", CharacterSpacing = 1 };
                    }
                    FrameLoad = new Frame()
                    {
                        Content = new StackLayout()
                        {
                            Children =
                            {
                                new Label() { Text = $"{friend_name}", HorizontalOptions = LayoutOptions.StartAndExpand, VerticalOptions= LayoutOptions.CenterAndExpand, TextColor=Color.FromHex("#D8D8D8"), FontSize=20, FontFamily="Inter", CharacterSpacing=1},
                                labelstatus
                            },
                            Orientation = StackOrientation.Horizontal,
                            VerticalOptions = LayoutOptions.Center
                        },
                        BackgroundColor = Color.FromHex("#252728"),
                        CornerRadius = 15,
                        HeightRequest = 60,
                        Padding = new Thickness(20, 0, 20, 0)

                    };
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (s, e) => {
                        var add_friend_username = friend_name;
                        if (good_internet)
                        {
                            if (friend_status == "0")
                            {

                                Decline_friend_by_name(add_friend_username);
                                Search_user_by_name(search_stroke);
                            }
                            else
                            {

                                Add_friend_by_name(add_friend_username);
                                CreateNewPublicKey(friend_personal);
                                Search_user_by_name(search_stroke);
                            }
                        }
                        else
                        {
                            display_menu_error("No internet connection.");
                        }
                    };
                    FrameLoad.GestureRecognizers.Add(tapGestureRecognizer);
                    LoadSearchRes.Children.Add(FrameLoad);
                }
                SearchResultStack = LoadSearchRes;
            }
            else
            {
                if (AddFriendEntry.Text != "")
                {
                    if (AddFriendEntry.Text.Length < 3)
                    {
                        friends_found.Text = "Enter at least 3 letters";
                    }
                    else
                    {
                        friends_found.Text = "No user with this username";
                    }
                    friends_found.IsVisible = true;
                }
                SearchResultStack.Children.Clear();
            }

        }


        public void SearchPeople(object sender, EventArgs e)
        {

            var search_user = AddFriendEntry.Text;
            if (search_user.Length > 0)
            {
                AddFriendEntryBlock.IsVisible = true;
                AddFriendBlock.IsVisible = false;
                Search_user_by_name(search_user);
            }
            else
            {
                friends_found.IsVisible = true;
                friends_found.Text = "Enter at least 3 letters";
                if (search_user.Length == 0)
                {
                    friends_found.IsVisible = false;
                }
                AddFriendEntryBlock.IsVisible = false;
                AddFriendBlock.IsVisible = true;
            }

        }
        public void ClearSearch(object sender, EventArgs e)
        {
            friends_found.IsVisible = false;
            AddFriendEntry.Text = "";
            Search_user_by_name("");
        }

        public void OpenUsernameEdit(object sender, EventArgs e)
        {
            if (action_username_button.Text == "Edit" && internet_on == true)
            {

                edit_name_underline.BackgroundColor = Color.FromHex("#D8D8D8");
                edit_username_entry.IsReadOnly = false;
                edit_username_entry.Focus();
                edit_name_underline.IsVisible = true;
                action_username_button.Text = "Save";
            }
            else
            {
                action_username_button.Text = "Edit";
                edit_username_entry.Unfocus();
                edit_username_entry.IsReadOnly = true;
                edit_name_underline.BackgroundColor = Color.FromHex("#00000000");
                if (edit_username_entry.Text != user_actual_username)
                {
                    Edit_username(edit_username_entry.Text);
                }
                else
                {

                }
            }
        }

        public void OpenSearcherBlock(object sender, EventArgs args)
        {
            AddFriendEntry.Focus();
        }

        public void OpenSearcher(object sender, EventArgs e)
        {
            AddFriendEntryBlock.IsVisible = false;
            friends_found.IsVisible = false;
            AddFriendBlock.IsVisible = true;
            AddFriendEntry.Text = "";
            AddFriendEntry.Unfocus();
        }


        public void countsymbols(object sender, EventArgs e)
        {
            var a = TextPostEntry.Text.Length.ToString() + "/300";

            symbcount.Text = a;
            if (TextPostEntry.Text.Split('\n').Length > 7)
            {
                OpenEditor(BlockEntry, e);
            }
        }
        public async void logout(object sender, EventArgs e)

        {
            swaptolog();
        }

        public static string autentification()
        {
            try
            {
                string res = "nice";
                if (DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds - Preferences.Get("last_autentification", 0.0) > 30)
                {

                    Preferences.Set("last_autentification", DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);

                    string ressf = Preferences.Get("refreshtoken", "none");
                   
                    
                    Autentificate_User(ressf);

                    

                }
                return res;
            }
            catch { return "error"; }
        }

        async public void load_info(bool appload)
        {
            string username = Preferences.Get("user_info", "nointernet") + ";";
            string token = Preferences.Get("token", "unlogged");
            try
            {
                if (appload || username == "nointernet")
                {
                    username = await GetUsername(token);
                    Preferences.Set("user_info", username);
                }
            }
            catch
            {

            }
            try
            {
                if (username != "-1" && username != "nointernet")
                {
                    string[] static_info = username.Split(';');
                    user_actual_username = static_info[0];
                    user_actual_unique = static_info[3];
                    edit_username_entry.Text = static_info[0];
                    invite_label.Text = static_info[1];
                    if (static_info[2] != "")
                    {
                        user_streak.Text = static_info[2];
                    }
                }
                if (appload is false)
                {
                    await Task.Delay(2000);
                    if (activate)
                    {

                        load_info(true);
                    }
                }
            }
            catch
            {
                
            }
        }
        public async static void idinfier()
        {
            device_timer -= 1;
            Preferences.Set("last_autentification_id", DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
            for (int i = 0; i < timers.Count; i++)
            {
                int curr_time = timers_ticks[i];
                timers_ticks[i] -= 1;
                try
                {
                    timers[i].Text = TimeChecker(curr_time);
                }
                catch
                {

                }
            }

            await Task.Delay(velocity);
            if (activate)
            {
                idinfier();
            }
        }
        public homepage()
        {
            try
            {


                if (Preferences.Get("token", "unlogged") == "unlogged" && Preferences.Get("refreshtoken", "unlogged") == "unlogged")
                {
                    swaptolog();
                }
                Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
                string token = Preferences.Get("token", "unlogged");

                activate = true;
                bool is_just_logged = Preferences.Get("first_log", false);
                replay_autentific();
                CircleUpdateReact();

                // выполняем действия, если в словаре есть ключ "name"
                InitializeComponent();
                if (is_just_logged)
                {
                    Navigation.PushPopupAsync(new appslovoM.welcome());
                }
                fire.Source = ImageSource.FromResource("appslovoM.fire.png");
                
                MainCarouselPage.CurrentPage = MainCarouselPage.Children[1]; //строка для смены текущей страницы

                load_post(true, false);
                load_info(false);

                Load_User_Friends("", false);
                Load_User_Requests("", false);
                Load_User_Friends_Posts("", false, true);
                InternetQuality();
                edit_username_entry.IsReadOnly = true;

            }
            catch(Exception e)
            {
          //  DisplayAlert("aa", e.ToString(), "oj");
            }
            
            

        }
        public async void show_streak(System.Object sender, System.EventArgs e)
        {
            //DisplayAlert("e", "loadposts:\n"+ k_fr_posts +"\nfriends:\n"+k_fr +"\n+requests:\n"+k_req +"\naccept:\n"+k_accept +"\nautentific:\n"+k_aut+"\nsearches:\n"+k_search+"\nfr_keys:\n"+k_fr_pbk+"\nuser_pbk:\n"+k_upbk+"\ndecline:\n"+k_decline + "\nusername:\n"+k_username+"\nchanges:\n"+k_user_edits, "ok");
            Navigation.PushPopupAsync(new appslovoM.streakpopuppage(user_streak.Text));
        }

        public async void show_settings(System.Object sender, System.EventArgs e)
        {
            try
            {
                Navigation.PopPopupAsync();
            }
            catch
            {
                Navigation.PushPopupAsync(new appslovoM.settings());
            }
        }
        async void display_post_error(string error_str)
        {
            error_displaying = true;
            error_alert_post.Margin = new Thickness(0, -20, 0, 0);
            error_alert_post.FontSize = 15;
            error_alert_post.TextColor = Color.FromHex("#ED4A34");
            error_alert_post.Text = error_str;
            await Task.Delay(2700);
            error_alert_post.Text = "";
            error_alert_post.TextColor = Color.FromHex("#D8D8D8");
            error_displaying = false;
        }

        async void display_menu_error(string error_str)
        {
            error_alert.Text = error_str;
            await Task.Delay(2700);
            error_alert.Text = "";
        }


        private async void openmenu(object sender, EventArgs e)
        {
            MainCarouselPage.CurrentPage = MainCarouselPage.Children[0];
        }
        private async void closemenu(object sender, EventArgs e)
        {
            MainCarouselPage.CurrentPage = MainCarouselPage.Children[1];
        }
        public bool alive = true;

        // таймер для поста
        public async void PostTimer(int seconds_count)
        {
            while (seconds_count > 0 && timer_disabled == false)
            {
                if (timer_disabled)
                {
                    seconds_count = 1;
                }

                else
                {
                    seconds_count -= 1;
                    counter_static.Text = TimeChecker(seconds_count);

                }
                if (seconds_count < 1)
                {
                    var token = Preferences.Get("token", "unlogged");
                    await NewPost(token, "*");
                    load_post(false, true);
                    TextPostEntry.Text = "";
                    user_actual_post_txt = "";

                }
                await Task.Delay(1000);
            }
        }

        // делаем из времени в секундах время в utc
        public static string TimeChecker(int countseconds)
        {
            string res = "";

            string seconds_separator = "";
            string minutes_separator = "";
            string hours_separator = "";
            int seconds = countseconds % 60;
            int minutes = ((countseconds - seconds) / 60) % 60;
            int hours = (((countseconds - seconds) / 60) - minutes) / 60;
            if (seconds < 10)
            {
                seconds_separator = "0";
            }
            if (minutes < 10)
            {
                minutes_separator = "0";
            }
            if (hours < 10)
            {
                hours_separator = "0";
            }
            res = hours_separator + hours + ":" + minutes_separator + minutes + ":" + seconds_separator + seconds;
            return res;
        }

        private void aaaddd_CurrentPageChanged(object sender, EventArgs e)
        {

        }
    }

}
