using Android.App;
using Android.Widget;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using Android.Bluetooth;
 
namespace App1
{
    [Activity(Label = "Health Connect", MainLauncher = true, Icon = "@drawable/health_connect_android_icon")]
    public class MainActivity : Activity
    {
        Button butonCautaBluetooth;
        EditText Rezultat;
        ImageView logo_firstPage;
        private BluetoothAdapter mBluetoothAdapter = null;
        public static MobileServiceClient MobileService =
        new MobileServiceClient(
        "https://softmedmobile.azurewebsites.net"
        );

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.FirstPage);
            logo_firstPage = FindViewById<ImageView>(Resource.Id.imageView1);
            logo_firstPage.Click += delegate
            {
                SetContentView(Resource.Layout.Main);
            };
            //butonCautaBluetooth = FindViewById<Button>(Resource.Id.button1);
            //Rezultat = FindViewById<EditText>(Resource.Id.editText1);
            CheckBt();
        }

        private void CheckBt()
        {
            
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (!mBluetoothAdapter.Enable())
            {
                Rezultat.Text = "Bluetooth acivat";
                Toast.MakeText(this, "Bluetooth Dezactivat",
                    ToastLength.Short).Show();
            }
            //verificamos que no sea nulo el sensor
            if (mBluetoothAdapter == null)
            {
                Rezultat.Text = "Nu exista modul de Bluetooth sau este ocupat";
                Toast.MakeText(this,
                    "Nu exista modul de Bluetooth sau este ocupat", ToastLength.Short)
                    .Show();
            }
        }
    }
}

