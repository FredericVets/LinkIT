using System;
using System.ComponentModel;
using System.Globalization;

namespace LinkIT.Data.Paging
{
	public class OrderByConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
			(sourceType == typeof(string)) ? true : base.CanConvertFrom(context, sourceType);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string valueString = value as string;
			if (valueString == null)
				return base.ConvertFrom(context, culture, value);

			if (OrderBy.TryParse(valueString, out OrderBy result))
				return result;

			return base.ConvertFrom(context, culture, value);
		}
	}
}