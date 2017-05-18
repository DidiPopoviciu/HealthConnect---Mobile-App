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
using Java.Lang;
using System.Text.RegularExpressions;

namespace App1
{
    [Activity(Label = "MainActivity")]
    public class MainActivity : Activity
    {
        Button butonCautaBluetooth, butonConnectArduino;
        TextView Rezultat, bluetoothConn;

        int bluetoothConnected;
        string data = "";
        string olddata = "";

        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        //MAC adress of bluetooth device
        private static string address = "00:06:66:4E:DA:6C";
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
            butonConnectArduino = FindViewById<Button>(Resource.Id.button3);
            Rezultat = FindViewById<TextView>(Resource.Id.textView3);
            bluetoothConn = FindViewById<TextView>(Resource.Id.textView4);
            butonCautaBluetooth.Click += delegate
            {
                bluetoothConnected = CheckBt();
            };

            butonConnectArduino.Click += delegate
            {
                Connect();
            };

        }


        private int CheckBt()
        {

            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (mBluetoothAdapter.Enable())
            {
                Rezultat.Text = "Bluetooth activat";
                return 1;
            }
            if (mBluetoothAdapter == null)
            {
                Rezultat.Text = "Nu exista modul de Bluetooth sau este ocupat";
                return 0;
            }
            return 0;
        }

        public void Connect()
        {

            bluetoothConn.Text += "Cautare dispozitiv remote";
            BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            bluetoothConn.Text += "Comexiune in curs" + device.Name + "\n";

            //Thread.Sleep(20000);
            mBluetoothAdapter.CancelDiscovery();
            try
            {
                btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                btSocket.Connect();
                bluetoothConn.Text = bluetoothConn.Text + "Conexiune Creata";
            }
            catch (System.Exception e)
            {
                bluetoothConn.Text += e.Message;
                try
                {
                    btSocket.Close();
                }
                catch (System.Exception)
                {
                    bluetoothConn.Text += "Conexiune nereusita";
                }
                bluetoothConn.Text += "Socket creat";
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
                Rezultat.Text = ex.Message;
            }

            Task.Factory.StartNew(() =>
            {
                byte[] buffer = new byte[1024];
                int bytes;
                while (true)
                {
                    try
                    {
                        bytes = inStream.Read(buffer, 0, buffer.Length);
                        if (bytes > 0)
                        {
                            RunOnUiThread(() =>
                            {
                                string valor = System.Text.Encoding.ASCII.GetString(buffer);
                                Rezultat.Text = Rezultat.Text + " " + valor;
                                olddata = data;
                                data = string.Concat(olddata, valor);
                                bluetoothConn.Text = Regex.Replace(data, @"\n|\t|\r", "");
                            });
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        RunOnUiThread(() =>
                        {
                            bluetoothConn.Text = string.Empty;
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