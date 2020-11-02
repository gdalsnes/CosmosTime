using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{
	/// <summary>
	/// Format\parse variable length format
	/// </summary>
	public class UtcOffsetTimeTypeConverter : TypeConverter
	{
		/// <inheritdoc />
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			=> sourceType == typeof(string);

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string utcOffsetString)
			{
				if (UtcOffsetTime.TryParse(utcOffsetString, out var uo))
					return uo;
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var u = (UtcOffsetTime)value;
				return u.ToString(); // variable length
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
