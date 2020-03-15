using System.Collections.Generic;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
    public class AssetsController : ApiController
    {
        // GET: api/Assets
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Assets/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Assets
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Assets/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Assets/5
        public void Delete(int id)
        {
        }
    }
}