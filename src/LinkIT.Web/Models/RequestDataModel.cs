using System.Collections.Specialized;

namespace LinkIT.Web.Models
{
	public class RequestDataModel
	{
		public NameValueCollection ServerVariables { get; set; }

		public NameValueCollection Headers { get; set; }
	}
}