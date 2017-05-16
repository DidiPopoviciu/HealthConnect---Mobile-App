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
using Android.Bluetooth;


namespace App1
{
    [Activity(Label = "MainActivity")]
    public class MainActivity : Activity
    {
        Button butonCautaBluetooth;
        //EditText Rezultat;
        TextView Rezultat;
        private BluetoothAdapter mBluetoothAdapter = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            butonCautaBluetooth = FindViewById<Button>(Resource.Id.button1);
            Rezultat = FindViewById<TextView>(Resource.Id.textView3);

            butonCautaBluetooth.Click += delegate
            {
                CheckBt();
            };
        }

        
        private void CheckBt()
        {

            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (mBluetoothAdapter.Enable())
            {
                Rezultat.Text = "Bluetooth activat";
            }
            if (mBluetoothAdapter == null)
            {
                Rezultat.Text = "Nu exista modul de Bluetooth sau este ocupat";
            }
        }
    }
}