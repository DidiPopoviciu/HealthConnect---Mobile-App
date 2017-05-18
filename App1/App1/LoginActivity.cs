using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;

namespace App1
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        Button butonLogin;
        EditText userName, password;

        public class Pacient
        {
            public int Id { get; set; }
            public int IdMedic { get; set; }
            public string Cnp { get; set; }
            public string Nume { get; set; }
            public string Prenume { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public int Varsta { get; set; }
            public string Localitate { get; set; }
            public string Strada { get; set; }
            public int NrStrada { get; set; }
            public string NrTelefon { get; set; }
            public string Email { get; set; }
            public string Profesie { get; set; }
            public string LocMunca { get; set; }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            butonLogin = FindViewById<Button>(Resource.Id.button2);
            butonLogin.Click += delegate
            {
                userName = FindViewById<EditText>(Resource.Id.editText1);
                password = FindViewById<EditText>(Resource.Id.editText2);

                authenticateAsync(userName.Text, password.Text);
            };
        }
        private async System.Threading.Tasks.Task authenticateAsync(string username, string password)
        {
            string passwordchiper = Encrypt(password);
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(new Uri("https://healthconnectapi.azurewebsites.net/api/Users/Pacient/" + username + "/" + passwordchiper));
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Pacient>(content);
                var mainActivity = new Intent(this, typeof(MainActivity));
                StartActivity(mainActivity);
                //salvare logare
                //salvare cnp global
                
            }
            else
            {
                //mesaj eroare fail login

            }
        }
        private string Encrypt(string source)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();
            byte[] byteHash;
            byte[] byteBuff;
            string key = "234fjhdf3";

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB;
            byteBuff = Encoding.UTF8.GetBytes(source);
            string cipher = Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return HttpUtility.UrlEncode(cipher);
        }
    }
}