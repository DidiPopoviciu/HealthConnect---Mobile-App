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
namespace App1
{
    [Activity(Label = "MainActivity")]
    public class MainActivity : Activity
    {
        Button butonCautaBluetooth, butonConnectArduino;
        TextView Rezultat, Puls, Temperatura, ECG;

        int bluetoothConnected;
        string data = "";
        string[] dataTable;
        int error_couner = 0;
        private BluetoothAdapter mBluetoothAdapter = null;
        private BluetoothSocket btSocket = null;
        private BluetoothSocket _socket = null;
        //MAC adress of bluetooth device
        private static string address = "00:06:66:4E:DA:6C";
        //private ID of communication
        private static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");

        string temp = "";
        string puls = "";
        string[] ecg;
        string ecg_str;
        private Stream outStream = null;
        private Stream inStream = null;

        private Java.Lang.String dataToSend;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            butonCautaBluetooth = FindViewById<Button>(Resource.Id.button1);
            butonConnectArduino = FindViewById<Button>(Resource.Id.button3);
            Rezultat = FindViewById<TextView>(Resource.Id.textViewInformatii);
            Puls = FindViewById<TextView>(Resource.Id.textViewPuls);
            Temperatura = FindViewById<TextView>(Resource.Id.textViewTemp);
            ECG = FindViewById<TextView>(Resource.Id.textViewEcg);
            //bluetoothConn = FindViewById<TextView>(Resource.Id.textView4);
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

            Rezultat.Text += "Cautare dispozitiv remote";
            try
            {
                BluetoothDevice device = (from bd in mBluetoothAdapter.BondedDevices
                                          where bd.Name == "Nexus 5X"
                                          select bd).FirstOrDefault();
                try
                {
                    //btSocket = device.CreateRfcommSocketToServiceRecord(MY_UUID);
                    //btSocket.Connect();
                    _socket = device.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                    // _socket.ConnectAsync();
                    _socket.Connect();
                    Rezultat.Text = Rezultat.Text + "Conexiune Creata";
                    beginListenForData();
                }
                catch (System.Exception e)
                {
                    Rezultat.Text += e.Message;
                    try
                    {
                        _socket.Close();
                    }
                    catch (System.Exception)
                    {
                        Rezultat.Text += "Conexiune nereusita" + "\n";
                    }
                    //Rezultat.Text += "Socket creat";
                }
            }
            catch (System.Exception e)
            {
                Rezultat.Text += e.Message + "\n";
            }
            ///BluetoothDevice device = mBluetoothAdapter.GetRemoteDevice(address);
            //Rezultat.Text += "Comexiune in curs" + device.Name + "\n";

            //Thread.Sleep(20000);
            mBluetoothAdapter.CancelDiscovery();
            
            

            //writeData(dataToSend);
        }

        public void beginListenForData()
        {
            try
            {
                inStream = _socket.InputStream;
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
                        string valor = System.Text.Encoding.ASCII.GetString(buffer);
                        if (bytes > 100)
                        {
                            RunOnUiThread(() =>
                            {
                               
                                if (!(System.String.IsNullOrEmpty(valor)))
                                {
                                    int index = 0;
                                    Rezultat.Text = valor;
                                    data = valor;

                                    dataTable = data.Split(';');

                                    for (int i = 0; i < dataTable.Length; i++)
                                    {
                                        if (dataTable[i] != "\0" && (dataTable[i].Contains("T:")))
                                        {
                                            if (dataTable[i].Contains("*T:"))
                                            {
                                                string[] temp_table = dataTable[i].Split('*');
                                                //bluetoothConn.Text += "1*." + temp_table[1] + "\n";
                                                if (!(temp_table[1].Contains(",")))
                                                {
                                                    temp = temp_table[1].Trim('T', ':');
                                                }

                                            }
                                            else
                                            {
                                                if (!(dataTable[i].Contains(",")))
                                                {
                                                    temp = dataTable[i].Trim('T', ':');
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (dataTable[i] != "\0" && (dataTable[i].Contains("B:")))
                                            {
                                                // bluetoothConn.Text += "2." + dataTable[1] + "\n";
                                                if (!(dataTable[i].Contains(",")))
                                                {
                                                    puls = dataTable[i].Trim('B', ':');
                                                }
                                            }
                                            else
                                            {
                                                if (dataTable[i] != "\0" && (dataTable[i].Contains("E:")))
                                                {
                                                    string[] ecg_table;
                                                    try
                                                    {
                                                        ecg_table = dataTable[i].Split(',');
                                                        if (ecg_table.Length > 90)
                                                        {
                                                            //bluetoothConn.Text += "3." + dataTable[i] + "\n";
                                                            ecg = ecg_table;
                                                        }
                                                        else
                                                        {
                                                            //bluetoothConn.Text += "3." + "ecg_error" + "\n";
                                                            ecg_table[0] = "ecg error";
                                                        }


                                                    }
                                                    catch
                                                    {
                                                    }

                                                }
                                            }
                                        }
                                    }
                                    if (ecg != null)
                                    {
                                        ecg_str = "";
                                        if (ecg[ecg.Length - 1].CompareTo("...") == 0)
                                        {
                                            ecg[0] = ecg[0].Trim('E', ':');
                                            for (int i = 0; i < ecg.Length - 1; i++)
                                            {
                                                ecg_str += ecg[i] + ",";
                                            }
                                        }
                                    }
                                    Temperatura.Text = temp;
                                    Puls.Text = puls;
                                    ECG.Text = ecg_str;

                                    data = "";
                                    bytes = 0;
                                    Array.Clear(buffer, 0, buffer.Length);
                                }
                                System.Threading.Tasks.Task.Delay(5000);

                                //olddata = data;
                                // bluetoothConn.Text = data;// Regex.Replace(data, @"\n|\t|\r", "");

                            });
                        }
                        else
                        {
                            RunOnUiThread(() =>
                            {
                                Rezultat.Text = valor;
                                data = "";
                                bytes = 0;
                                Array.Clear(buffer, 0, buffer.Length);
                            });
                            
                        }
                    }
                    catch (Java.IO.IOException)
                    {
                        RunOnUiThread(() =>
                        {
                            Rezultat.Text = "Conexiune pierduta!";
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