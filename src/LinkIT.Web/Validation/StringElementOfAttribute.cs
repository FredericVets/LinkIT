using System;
using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Validation
{
	public class StringElementOfAttribute : ValidationAttribute
	{
		private StringComparison _stringComparison;

		public StringElementOfAttribute(
			StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase) =>
			_stringComparison = stringComparison;

		public string[] Elements { get; set;  }

		public override bool IsValid(object value)
		{
			string valueString = value as string;

			// So it's still usable on both mandatory and non-mandatory fields.
			if (string.IsNullOrWhiteSpace(valueString))
				return true;

			foreach (string s in Elements)
			{
				if (s.Equals(valueString, _stringComparison))
					return true;
			}

			return false;
		}
	}
}