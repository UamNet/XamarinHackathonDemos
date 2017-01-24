using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Emotion;

namespace pokedexxamarin
{
    public partial class PokedexDetailsPage : ContentPage
    {

        private PokedexList parent;
        private String id;

        public PokedexDetailsPage(string name, String url, PokedexList parent, String id)
        {
            InitializeComponent();
            this.name.Text = name;
            this.id = id;
            this.parent = parent;
            picture.Source = url;
            describePicture(url);
            recognizeFaces(url);
            recognizeEmotion(url);
        }

        public async void describePicture(String url)
        {
            VisionServiceClient visionClient = new VisionServiceClient("f1a468cec1774887a6c2e0653651202a");
            var description = await visionClient.DescribeAsync(url);
            this.description.Text = description.Description.Captions[0].Text;
        }

        public async void recognizeFaces(String url)
        {
            FaceServiceClient faceClient = new FaceServiceClient("15ada265ee2548c1bbcf999fd0273c40");
            var atributes = new List<FaceAttributeType>() { FaceAttributeType.Age, FaceAttributeType.Gender };
            var description = await faceClient.DetectAsync(url, false, false, atributes);//No face id, no face landmarks, send face attributes
            if (description.Length > 0)
            {
                this.age.Text = description[0].FaceAttributes.Age.ToString();
                this.gender.Text = description[0].FaceAttributes.Gender.ToString();
            }
            else
            {
                this.age.Text = "None";
                this.gender.Text = "None";
            }
        }

        public async void recognizeEmotion(String url)
        {
            EmotionServiceClient emotionClient = new EmotionServiceClient("ddbbcdf41520452da2feed98f226d46d");
            var description = await emotionClient.RecognizeAsync(url);
            if (description.Length > 0)
            {
                this.emotion.Text = description[0].Scores.ToRankedList().First().Key;
            }
            else
            {
                this.emotion.Text = "None";
            }
        }

        public async void OnSave(object sender, EventArgs e)
        {
            Pokemon item = new Pokemon()
            {
                Name = this.name.Text,
                Description = this.description.Text,
                Age = this.age.Text,
                Gender = this.gender.Text,
                Emotion = this.emotion.Text,
                Img =id
            };
            await this.parent.AddItem(item);
        }
    }
}
