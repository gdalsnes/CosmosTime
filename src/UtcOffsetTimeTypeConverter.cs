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

		/// <inheritdoc />
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string utcOffsetString)
			{
				/* 2020-10-27T10:59:54Z -> offset 0
				 * 2020-10-27T10:59:54 -> local time (BAD) Will not allow this... must check manually
				 * 2020-10-27T10:59:54+00:10  -> offset 10min

				 * 
				 * DateTimeStyles.AdjustToUniversal and DateTimeStyles.RoundtripKind are very similar in a way, and mutually exlusive (cannot be used together)
				 * */

				// offset local time(BAD) Will not allow this... must check manually
				Func<string, bool> endsWithZ = (str) => str.Length > 0 && str[str.Length - 1] == 'Z';
				Func<string, bool> hasOffset = (str) =>
				{
					for (int i = 2; i <= 6 && i <= str.Length; i++)
					{
						var c = str[str.Length - i];
						if (c == '-' || c == '+')
							return true;
					}
					return false;
				};

				// DateTimeStyles.RoundtripKind seem to have no effect on DateTimeOffset. but set it anyways
				if (endsWithZ(utcOffsetString) || hasOffset(utcOffsetString))
					if (DateTimeOffset.TryParse(utcOffsetString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dto))
						return dto.ToUtcOffsetTime();
			}

			return base.ConvertFrom(context, culture, value);
		}

		/// <inheritdoc />
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				var u = (UtcOffsetTime)value;
				return u.ToString(); // Round-trip date/time pattern (fixed length): "2009-06-15T13:45:30.0000000Z"
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
