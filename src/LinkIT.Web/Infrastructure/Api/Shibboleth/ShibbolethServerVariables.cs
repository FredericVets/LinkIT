using System;
using System.Collections.Generic;
using System.Web;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	/// <summary>
	/// See remark in the ShibbolethHeaders class.
	/// </summary>
	public class ShibbolethServerVariables : ShibbolethAttributesBase
	{
		private readonly HttpRequestBase _request;

		public ShibbolethServerVariables(HttpRequestBase request = null) =>
			_request = request ?? new HttpRequestWrapper(HttpContext.Current.Request);

		private bool HasServerVariables() =>
			_request.ServerVariables?.HasKeys() == true;

		protected override bool InnerTryGet(string key, out string value)
		{
			value = null;

			if (!HasServerVariables())
				return false;

			string temp = _request.ServerVariables[key];
			if (string.IsNullOrWhiteSpace(temp))
				return false;

			value = temp;

			return true;
		}

		public override IDictionary<string, string> GetAll()
		{
			var result = new Dictionary<string, string>();
			if (!HasServerVariables())
				return result;

			var serverVariables = _request.ServerVariables;
			foreach (string key in serverVariables.AllKeys)
			{
				if (!IsShibbolethKey(key))
					continue;

				result.Add(key, serverVariables[key]);
			}

			return result;
		}
	}
}