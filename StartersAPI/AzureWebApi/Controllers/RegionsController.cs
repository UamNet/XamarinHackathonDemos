using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AzureWebApi.Controllers
{
    public class RegionsController : ApiController
    {
        static List<String> regions = new List<String>() { "Kanto", "Johto"};
        // GET: api/Regions
        public IEnumerable<string> Get()
        {
            return regions;
        }

        // GET: api/Regions/5
        public string Get(int id)
        {
            return regions[id];
        }

        // POST: api/Regions
        public void Post([FromBody]string value)
        {
            regions.Add(value);
        }

        // PUT: api/Regions/5
        public void Put(int id, [FromBody]string value)
        {
            regions[id] = value;
        }

        // DELETE: api/Regions/5
        public void Delete(int id)
        {
            regions.RemoveAt(id);
        }
    }
}
