using System.Net.Http.Headers;
using System.Web;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	/// <summary>
	/// A context has access to a set of attributes.
	/// 
	/// Those attributes are based on datastructures that are available at the level of the ActionMethod. (In reality they 
	/// get injected into the IIS pipeline by the Shibboleth middleware).
	/// Should be set by using the From() method before usage.
	/// 
	/// A mock implementation is availbe : ShibbolethContextMock.
	/// This can be used in test environment where the Shibboleth middleware isn't available.
	/// </summary>
	public class ShibbolethContext : IShibbolethContext
	{
		public ShibbolethAttributesBase FromHeaders(HttpRequestHeaders headers) =>
			new ShibbolethHeaders(headers);

		public ShibbolethAttributesBase FromServerVariables(HttpRequestBase request = null) =>
			new ShibbolethServerVariables(request);
	}
}