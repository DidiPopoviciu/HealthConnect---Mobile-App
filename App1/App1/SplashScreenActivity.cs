﻿using Android.App;
using Android.Widget;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using Android.Bluetooth;
using Android.Content;

namespace App1
{
    [Activity(Label = "Health Connect", MainLauncher = true, Icon = "@drawable/health_connect_android_icon")]
    public class SplashScreenActivity : Activity
    {

        ImageView logo_firstPage;
        
      

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstPage);
            logo_firstPage = FindViewById<ImageView>(Resource.Id.imageView1);
            logo_firstPage.Click += delegate
            {
                //var loginActivity = new Intent(this, typeof(LoginActivity));
                //StartActivity(loginActivity);
                var mainActivity = new Intent(this, typeof(MainActivity));
                StartActivity(mainActivity);
                //var recomandari = new Intent(this, typeof(RecomandriActivity));
                //StartActivity(recomandari);


            };


        }


    }
}

