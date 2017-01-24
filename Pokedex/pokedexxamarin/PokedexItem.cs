using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace pokedexxamarin
{
    public class Pokemon
    {
        string id;
        string name;
        string age;
        string gender;
        string img;
        string description;
        string emotion;

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [JsonProperty(PropertyName = "description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [JsonProperty(PropertyName = "emotion")]
        public string Emotion
        {
            get { return emotion; }
            set { emotion = value; }
        }

        [JsonProperty(PropertyName = "age")]
        public string Age
        {
            get { return age; }
            set { age = value; }
        }

        [JsonProperty(PropertyName = "gender")]
        public string Gender
        {
            get { return gender; }
            set { gender = value; }
        }

        [JsonProperty(PropertyName = "img")]
        public string Img
        {
            get { return img; }
            set { img = value; }
        }

        [Version]
        public string Version { get; set; }
    }
}

