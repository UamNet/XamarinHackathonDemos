using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Net.Http;

namespace StarterPokemons
{
    public partial class MainPage : ContentPage
    {
        String[] Regions { get; set; }
        List<Pokemon> Starters { get; set; }

        public MainPage()
        {
            InitializeComponent();
            GetRegions();
        }
        public async Task GetRegions()
        {
            using (var client = new HttpClient())
            {

                var url = string.Format("http://hackathonwebapi.azurewebsites.net/api/regions");
                var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    RegionPicker.Items.Clear();
                    Regions = JsonConvert.DeserializeObject<String[]>(resp.Content.ReadAsStringAsync().Result);
                    foreach (var region in Regions)
                    {
                        RegionPicker.Items.Add(region);
                    }
                }
            }
        }

        async void OnRegionSelected(Object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                var url = string.Format("http://hackathonwebapi.azurewebsites.net/api/pokemon/" + RegionPicker.SelectedIndex);
                var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    var startersReceived = JsonConvert.DeserializeObject<Pokemon[]>(resp.Content.ReadAsStringAsync().Result);
                    Starters = startersReceived.ToList<Pokemon>();
                    starterList.ItemsSource = Starters;
                    starterList.RowHeight = (int)this.Width+60;
                }
            }
        }

        public class Pokemon
        {
            public String name { get; set; }
            public String type { get; set; }
            public String image { get; set; }
        }

    }
}
