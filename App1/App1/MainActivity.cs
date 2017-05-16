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
using Java.Util;
using System.IO;
using System.Threading.Tasks;

namespace App1
{
    [Activity(Label = "MainActivity")]
    public class MainActivity : Activity
    {
        Button butonCautaBluetooth;
        TextView Rezultat;
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        //MAC adress of bluetooth device
        private static string address = "";
        //private ID of communication
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        private Stream outStream = null;
        private Stream inStream = null;

        private Java.Lang.String dataToSend;


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

        public void Connect()
        {
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            System.Console.WriteLine("Comexiune in curs" + device);

            
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                btSocket.Connect();
                System.Console.WriteLine("Conexiune Creata");
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                try
                {
                    btSocket.Close();
                }
                catch (System.Exception)
                {
                    System.Console.WriteLine("Conexiune nereusita");
                }
                System.Console.WriteLine("Socket creat");
            }
            beginListenForData();

            //writeData(dataToSend);
        }

        public void beginListenForData()
        {
            try
            {
                inStream = btSocket.InputStream;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            Task.Factory.StartNew(() => {
                byte[] buffer = new byte[1024];
                int bytes;
                while (true)
                {
                    try
                    {
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            RunOnUiThread(() => {
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                Rezultat.Text = Rezultat.Text + "\n" + valor;
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        RunOnUiThread(() => {
                            Rezultat.Text = string.Empty;
                        });
                        break;
                    }
                }
            });
        }

        private void writeData(Java.Lang.String data)
        {
            try
            {
                outStream = btSocket.OutputStream;
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Eroare la transmitere" + e.Message);
            }

            Java.Lang.String message = data;

            byte[] msgBuffer = message.GetBytes();

            try
            {
                outStream.Write(msgBuffer, 0, msgBuffer.Length);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine("Eroare la transmitere" + e.Message);
            }
        }
    }
}