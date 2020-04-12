using System;
using System.Collections.Generic;

namespace LinkIT.Web.Infrastructure.Api.Shibboleth
{
	public class ShibbolethAttributesMock : ShibbolethAttributesBase
	{
		private readonly IDictionary<string, string> _data;

		public ShibbolethAttributesMock(IDictionary<string, string> data) =>
			_data = data ?? throw new ArgumentNullException(nameof(data));

		protected override bool InnerTryGet(string key, out string value)
		{
			value = null;

			if (!_data.ContainsKey(key))
				return false;

			value = _data[key];

			return true;
		}

		public override IDictionary<string, string> GetAll() =>
			_data;
	}
}