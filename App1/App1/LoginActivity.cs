using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        Button butonLogin;
        EditText userName, password;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Login);
            butonLogin = FindViewById<Button>(Resource.Id.button2);
            butonLogin.Click += delegate
            {
                userName = FindViewById<EditText>(Resource.Id.editText1);
                password = FindViewById<EditText>(Resource.Id.editText2);
                if (userName.Text == "admin" && password.Text == "admin")
                {
                    var mainActivity = new Intent(this, typeof(MainActivity));
                    StartActivity(mainActivity);
                    // 
                }
            };
            // Create your application here
        }
    }
}