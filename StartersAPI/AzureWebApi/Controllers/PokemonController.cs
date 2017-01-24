using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AzureWebApi.Controllers
{
    public class PokemonController : ApiController
    {
        public class Pokemon {
            public String name;
            public String type;
            public String image;
        }

        static Pokemon Bulbasaur = new Pokemon() { name = "Bulbasaur", type = "Grass", image = "http://cdn.bulbagarden.net/upload/thumb/2/21/001Bulbasaur.png/500px-001Bulbasaur.png" };
        static Pokemon Charmander = new Pokemon() { name = "Charmander", type = "Fire", image = "http://cdn.bulbagarden.net/upload/thumb/7/73/004Charmander.png/500px-004Charmander.png" };
        static Pokemon Squirtle = new Pokemon() { name = "Squirtle", type = "Water", image = "http://cdn.bulbagarden.net/upload/thumb/3/39/007Squirtle.png/500px-007Squirtle.png" };
        static List<Pokemon> KantoStarters = new List<Pokemon>() { Bulbasaur, Charmander, Squirtle };

        static Pokemon Chikorita = new Pokemon() { name = "Chikorita", type = "Grass", image = "http://cdn.bulbagarden.net/upload/thumb/b/bf/152Chikorita.png/500px-152Chikorita.png" };
        static Pokemon Cyndaquil = new Pokemon() { name = "Cyndaquil", type = "Fire", image = "http://cdn.bulbagarden.net/upload/thumb/9/9b/155Cyndaquil.png/500px-155Cyndaquil.png" };
        static Pokemon Totodile = new Pokemon() { name = "Totodile", type = "Water", image = "http://cdn.bulbagarden.net/upload/thumb/d/df/158Totodile.png/500px-158Totodile.png" };
        static List<Pokemon> JohtoStarters = new List<Pokemon>() { Chikorita, Cyndaquil, Totodile };


        // GET: api/Pokemon/regionName
        public IEnumerable<Pokemon> Get(int id)
        {
            switch (id) {
                case 0:
                    return KantoStarters;
                case 1:
                    return JohtoStarters;
                default:
                    return new List<Pokemon>() ;
            }
        }

        // POST: api/Pokemon
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Pokemon/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Pokemon/5
        public void Delete(int id)
        {
        }
    }
}
