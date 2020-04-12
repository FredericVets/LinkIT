using System.Net.Http.Headers;
using System.Web;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	public interface IShibbolethContext
	{
		ShibbolethAttributesBase FromHeaders(HttpRequestHeaders headers);

		ShibbolethAttributesBase FromServerVariables(HttpRequestBase request = null);

	}
}