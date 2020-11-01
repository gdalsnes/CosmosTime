using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{
	[TypeConverter(typeof(UtcTimeTypeConverter))]
	public struct UtcTime : IEquatable<UtcTime>, IComparable<UtcTime>, IComparable
	{
		DateTime _utc;

		public DateTime UtcDateTime => _utc;

		public static UtcTime Now => new UtcTime(DateTime.UtcNow);

		/// <summary>
		/// Always uses InvariantCulture
		/// TODO: rename ToStringInvariant? ToUtcString?
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string ToString(string format)
		{
			return _utc.ToString(format, CultureInfo.InvariantCulture);
		}

		//public string ToString(string v, IFormatProvider fp)
		//{
		//	return _dt.ToString(v, fp);
		//}

		public override string ToString() => ToUtcString();


		/// <summary>
		/// Fixed length
		/// </summary>
		/// <returns></returns>
		public string ToCosmosDbString()
		{
			return _utc.ToString(FixedLengthFormatUtc, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Variable length
		/// </summary>
		/// <returns></returns>
		public string ToUtcString()
		{
			return _utc.ToString(VariableLengthFormatUtc, CultureInfo.InvariantCulture);
		}

		// PArse: allow inly UTC or localk+offset

		public UtcTime(DateTime utcTime)
		{
			if (utcTime.Kind != DateTimeKind.Utc)
				throw new Exception("not utc");

			_utc = utcTime;
		}

		public DateTime ToLocalTime()
		{
			return _utc.ToLocalTime();
		}

		public UtcTime(int v1, int v2, int v3) : this()
		{
			_utc = new DateTime(v1, v2, v3, 0, 0, 0, DateTimeKind.Utc);
		}

		public UtcTime(int v1, int v2, int v3, int v4, int v5, int v6) : this()
		{
			_utc = new DateTime(v1, v2, v3, v4, v5, v6, DateTimeKind.Utc);
		}

		public UtcTime Min(UtcTime other)
		{
			if (this._utc < other._utc)
				return this;
			else
				return other;
		}
		public UtcTime Max(UtcTime other)
		{
			if (this._utc > other._utc)
				return this;
			else
				return other;
		}

		public static TimeSpan operator -(UtcTime a, UtcTime b) => a._utc - b._utc;



		public static bool operator ==(UtcTime a, UtcTime b) => a._utc == b._utc;

		public static bool operator !=(UtcTime a, UtcTime b) => a._utc != b._utc;

		public static bool operator <(UtcTime a, UtcTime b) => a._utc < b._utc;

		public static bool operator >(UtcTime a, UtcTime b) => a._utc > b._utc;

		public static bool operator <=(UtcTime a, UtcTime b) => a._utc <= b._utc;

		public static bool operator >=(UtcTime a, UtcTime b) => a._utc >= b._utc;

		// can only cast when you know DateTime is utc. else use DateTime.ToUtcTime()
		//public static explicit operator UtcTime(DateTime dt)
		//{
		//	return new UtcTime(dt); 
		//}
		//public static explicit operator UtcTime?(DateTime? dt)
		//{
		//	if (dt == null)
		//		return null;
		//	else
		//		return dt.Value.ToUtcTime();
		//}

		//public static explicit operator DateTime(UtcTime dt)
		//{
		//	return dt.Utc;
		//}
		//public static explicit operator DateTime?(UtcTime? dt)
		//{
		//	if (dt == null)
		//		return null;
		//	else
		//		return dt.Value.Utc;
		//}

		public UtcTime AddSeconds(double v) => _utc.AddSeconds(v).ToUtcTime();


		// kind of both is utc
		public bool Equals(UtcTime other) => _utc.Equals(other._utc);


		public override bool Equals(object obj) => obj is UtcTime other && Equals(other);

		public override int GetHashCode() => _utc.GetHashCode();


		public int CompareTo(UtcTime other) => _utc.CompareTo(other._utc);


		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((UtcTime)obj);
		}

		internal UtcTime AddMinutes(double offset) => _utc.AddMinutes(offset).ToUtcTime();


		public const string FixedLengthFormatUtcWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'fffffff";
		// this is almost the same as "o" format (roundtrip), except roundtrip uses K (kind) instead of Z (zulu)
		public const string FixedLengthFormatUtc = FixedLengthFormatUtcWithoutZ + "Z";

		public const string VariableLengthFormatUtcWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'FFFFFFF";
		public const string VariableLengthFormatUtc = VariableLengthFormatUtcWithoutZ + "Z";

		public static UtcTime ParseCosmosDb(string utc)
		{
			// yyyy-MM-ddTHH:mm:ss.fffffffZ
			if (utc.Length != 28)
				throw new FormatException("not 28 chars");

			// does verify the length, but do it outselfs anyways to be sure
			var dt = DateTime.ParseExact(utc, FixedLengthFormatUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind /* needed? yes, else kind is wrong*/);
			return new UtcTime(dt);
		}

		public static readonly UtcTime MinValue = DateTime.MinValue.ToUtcTime();
		public static readonly UtcTime MaxValue = DateTime.MaxValue.ToUtcTime();

		public static UtcTime ParseUtc(string str)
		{
			if (TryParseUtc(str, out var ut))
				return ut;
			throw new FormatException("not utc or local[+-]offset");
		}

		/// <summary>
		/// Parse any ISO time that can be converter to utc (utc or local[+-]offset). Example:
		/// 2020-01-01Z
		/// 2020-01-01T12:12:12Z
		/// 2020-01-01T12:12:12.123Z
		/// 2020-01-01T12:12:12.123+00:30Z
		/// </summary>
		/// <param name="str"></param>
		/// <param name="utc"></param>
		/// <returns></returns>
		public static bool TryParseUtc(string str, out UtcTime utc)
		{
			/* 2020-10-27T10:59:54Z -> Kind.Utc
 * 2020-10-27T10:59:54 -> Kind.Unspec
 * 2020-10-27T10:59:54+00:10  -> Kind.Utc
 * This is becase of DateTimeStyles.AdjustToUniversal (and we require UTc here)
 * 
 * If using DateTimeStyles.RoundtripKind we would get
 * Kind.Utc
 * Kind.Unspec
 * Kind.Local
 * 
 * DateTimeStyles.AdjustToUniversal and DateTimeStyles.RoundtripKind are very similar in a way, and mutually exlusive (cannot be used together)
 * */
			if (DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt)
				&& dt.Kind == DateTimeKind.Utc)
			{
				utc = new UtcTime(dt);
				return true;
			}

			utc = UtcTime.MinValue;
			return false;
		}

	}
}
