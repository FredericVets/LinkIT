using System.Collections.Generic;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
	[Route("api/values")]
	public class ValuesController : ApiController
	{
		// GET api/values
		public IEnumerable<string> Get()
		{
			// return new string[] { "value1", "value2" };

			var formatters = new List<string>();
			foreach (var item in GlobalConfiguration.Configuration.Formatters)
				formatters.Add(item.ToString());

			return formatters;
		}

		// GET api/values?id=5
		public string Get(int id)
		{
			return $"value for id {id}";
		}

		// GET api/values?name=bla
		public string Get(string name)
		{
			return $"value for name {name}";
		}

		// POST api/values
		public void Post([FromBody]string value)
		{
		}

		// PUT api/values/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		public void Delete(int id)
		{
		}
	}
}