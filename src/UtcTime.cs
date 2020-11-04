﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CosmosTime
{
	[TypeConverter(typeof(UtcTimeTypeConverter))]
	public struct UtcTime : IEquatable<UtcTime>, IComparable<UtcTime>, IComparable
	{
		public const string FixedLengthFormatUtcWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'fffffff";
		// this is almost the same as "o" format (roundtrip), except roundtrip uses K (kind) instead of Z (zulu)
		public const string FixedLengthFormatUtc = FixedLengthFormatUtcWithoutZ + "Z";

		public const string VariableLengthFormatUtcWithoutZ = "yyyy'-'MM'-'ddTHH':'mm':'ss'.'FFFFFFF";
		public const string VariableLengthFormatUtc = VariableLengthFormatUtcWithoutZ + "Z";

		public static readonly UtcTime MinValue = DateTime.MinValue.ToUtcTime();
		public static readonly UtcTime MaxValue = DateTime.MaxValue.ToUtcTime();

		DateTime _utc;

		public DateTime UtcDateTime => _utc;

		public static UtcTime Now => new UtcTime(DateTime.UtcNow);

		/// <summary>
		/// Fixed length
		/// </summary>
		/// <returns></returns>
		public string ToCosmosDb()
		{
			return _utc.ToString(FixedLengthFormatUtc, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Invariant culture
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public string ToString(string format)
		{
			return _utc.ToString(format, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Variable length
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _utc.ToString(VariableLengthFormatUtc, CultureInfo.InvariantCulture);
		}

		public UtcTime(DateTime utcTime)
		{
			if (utcTime.Kind != DateTimeKind.Utc)
				throw new Exception("not utc");

			_utc = utcTime;
		}

		public DateTime ToLocalTime() => _utc.ToLocalTime();
		

		public UtcTime(int year, int month, int day) : this()
		{
			_utc = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
		}

		public UtcTime(int year, int month, int day, int hour, int minute, int second) : this()
		{
			_utc = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
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
		public static UtcTime operator -(UtcTime d, TimeSpan t) => (d._utc - t).ToUtcTime();
		public static UtcTime operator +(UtcTime d, TimeSpan t) => (d._utc + t).ToUtcTime();

		public static bool operator ==(UtcTime a, UtcTime b) => a._utc == b._utc;
		public static bool operator !=(UtcTime a, UtcTime b) => a._utc != b._utc;
		public static bool operator <(UtcTime a, UtcTime b) => a._utc < b._utc;
		public static bool operator >(UtcTime a, UtcTime b) => a._utc > b._utc;
		public static bool operator <=(UtcTime a, UtcTime b) => a._utc <= b._utc;
		public static bool operator >=(UtcTime a, UtcTime b) => a._utc >= b._utc;

		public UtcTime AddSeconds(double sec) => _utc.AddSeconds(sec).ToUtcTime();
		public UtcTime AddMinutes(double min) => _utc.AddMinutes(min).ToUtcTime();
		public UtcTime AddHours(double h) => _utc.AddHours(h).ToUtcTime();
		public UtcTime AddDays(double d) => _utc.AddDays(d).ToUtcTime();

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

		public static UtcTime ParseCosmosDb(string utc)
		{
			// yyyy-MM-ddTHH:mm:ss.fffffffZ
			if (utc.Length != 28)
				throw new FormatException("not 28 chars");

			// does verify the length, but do it outselfs anyways to be sure
			var dt = DateTime.ParseExact(utc, FixedLengthFormatUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind /* needed? yes, else kind is wrong*/);
			return new UtcTime(dt);
		}

		/// <summary>
		/// Only allows {utc}Z or {local}[+-]{offset}
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static UtcTime Parse(string str)
		{
			if (TryParse(str, out var ut))
				return ut;
			throw new FormatException("not utc or local[+-]offset");
		}

		/// <summary>
		/// Parse any Iso time in utc or local[+-]offset. Example:
		/// 2020-01-01Z
		/// 2020-01-01T12:12:12Z
		/// 2020-01-01T12:12:12.123Z
		/// 2020-01-01T12:12:12.123+00:30
		/// </summary>
		/// <param name="str"></param>
		/// <param name="utc"></param>
		/// <returns></returns>
		public static bool TryParse(string str, out UtcTime utc)
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
