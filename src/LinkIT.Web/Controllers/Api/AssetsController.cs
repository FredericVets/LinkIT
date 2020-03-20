using LinkIT.Data.DTO;
using LinkIT.Data.Queries;
using LinkIT.Data.Repositories;
using LinkIT.Web.Infrastructure.Api;
using log4net;
using System.Collections.Generic;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
    public class AssetsController : ApiController
    {
        private readonly IRepository<AssetDto, AssetQuery> _assetRepo;
        private readonly ILog _log;

        public AssetsController()
        {
            var productRepo = new ProductRepository(ConnectionString.Get());
            _assetRepo = new AssetRepository(ConnectionString.Get(), productRepo);

            _log = LogManager.GetLogger(GetType());
        }

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