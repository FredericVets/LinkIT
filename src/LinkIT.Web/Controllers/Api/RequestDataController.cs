using LinkIT.Web.Infrastructure.Shibboleth;
using log4net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace LinkIT.Web.Controllers.Api
{
    /// <summary>
    /// To test what values are available in the headers and the ServerData collection.
    /// </summary>
    public class RequestDataController : ApiController
    {
        private readonly ILog _log;
        private readonly ShibbolethAttributes _shibbolethAttributes;

        public RequestDataController(ShibbolethAttributes shibbolethAttributes)
        {
            _log = LogManager.GetLogger(GetType());
            _shibbolethAttributes = shibbolethAttributes;
        }

        //[Route("api/request-data")]
        public void Get()
        {
            var sb = new StringBuilder();

            sb.AppendLine("HttpContext.Current.Request.ServerVariables :");
            sb.AppendLine();
            var serverVars = HttpContext.Current.Request.ServerVariables;
            foreach (string key in serverVars.AllKeys)
                sb.AppendLine($"key: {key}, value: {serverVars[key]}");

            sb.AppendLine();
            sb.AppendLine("HttpContext.Current.Request.Headers :");
            sb.AppendLine();
            var headers = HttpContext.Current.Request.Headers;
            foreach (string key in headers.AllKeys)
                sb.AppendLine($"key: {key}, value: {headers[key]}");

            sb.AppendLine();
            sb.AppendLine("Request.Headers :");
            sb.AppendLine();
            foreach (var kvp in Request.Headers)
                sb.AppendLine($"key: {kvp.Key}, value: {string.Join(",", kvp.Value)}");

            sb.AppendLine();
            sb.AppendLine("ShibbolethAttributes :");
            foreach (var kvp in _shibbolethAttributes.GetAll())
                sb.AppendLine($"key: {kvp.Key}, value: {kvp.Value}");
            sb.AppendLine();

            _log.Info(sb.ToString());
        }
    }
}