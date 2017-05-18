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
using Microsoft.WindowsAzure.MobileServices;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace App1
{
    [Activity(Label = "RecomandriActivity")]

    public class RecomandriActivity : Activity
    {
        Button getActivities;
        HttpClient client;
        TextView recomandari;

        public class Recomandare
        {
            public string cnp;
            public string TipActivitate;
            public string Mesaj;

        }

        public static MobileServiceClient MobileService =
      new MobileServiceClient(
      "https://softmedmobile.azurewebsites.net"
      );

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Recomandari);

            getActivities = FindViewById<Button>(Resource.Id.button41);
            recomandari = FindViewById<TextView>(Resource.Id.textView1);
            getActivities.Click += async delegate
            {
                await getRequest();
            };
        }
        private async Task getRequest()
        {
            client = new HttpClient();
            var response = await client.GetAsync(new Uri("https://healthconnectapi.azurewebsites.net/api/Recomandare/1881211887143"));

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                // recomandari.Text = content;
                var result = JsonConvert.DeserializeObject<List<Recomandare>>(content);
                foreach (var recomandare in result)
                {
                    recomandari.Text += "Tip Actiitate:\n" + recomandare.TipActivitate + "\nMesaj Activitate:\n" + recomandare.Mesaj + "\n --------------------\n";
                }
            }

            await Task.Delay(1);
        }
    }
}