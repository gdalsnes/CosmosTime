using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CosmosTime
{
	public struct UtcOffsetTime : IEquatable<UtcOffsetTime>, IComparable<UtcOffsetTime>, IComparable
	{
		UtcTime _utc;
		short _offsetMins;

		public static UtcOffsetTime Now => DateTimeOffset.Now.ToUtcOffsetTime();

		public UtcTime UtcTime => _utc;

		public static readonly UtcOffsetTime MinValue = DateTimeOffset.MinValue.ToUtcOffsetTime();
		public static readonly UtcOffsetTime MaxValue = DateTimeOffset.MaxValue.ToUtcOffsetTime();

		/// <summary>
		/// Offset from Utc
		/// </summary>
		public int OffsetMins => _offsetMins;

		public static UtcOffsetTime Parse(string str)
		{
			if (TryParse(str, out var ut))
				return ut;
			throw new FormatException("not utc or local[+-]offset");
		}

		public static bool TryParse(string utcOffsetString, out UtcOffsetTime uo)
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
				{
					uo = dto.ToUtcOffsetTime();
					return true;
				}

			uo = UtcOffsetTime.MinValue;
			return false;
		}


		/// <summary>
		/// Problem: dto may be messed up already, but we cant do anything about that......
		/// </summary>
		public UtcOffsetTime(DateTimeOffset dto)
		{
			//_dto = dto;
			_utc = dto.UtcDateTime.ToUtcTime();
			_offsetMins = (short)dto.Offset.TotalMinutes;
		}

		public UtcOffsetTime(UtcTime utcs, short offsetMinutes) : this()
		{
			_utc = utcs;

			if (offsetMinutes < -840 || offsetMinutes > 840)
				throw new ArgumentException("offset can be max [+-] 14 hours");

			_offsetMins = offsetMinutes;
		}

		/// <summary>
		/// Variable length utc[+-]offset
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var local = _utc.AddMinutes(_offsetMins);

			//int seconds = 10000; //or whatever time you have
			//string.Format("{0:00}':'{1:00}", seconds / 3600, (seconds / 60) % 60);
			var mins = _offsetMins;
			bool neg = mins < 0;
			if (neg)
				mins *= -1;

			var strNoZ = local.ToString(UtcTime.VariableLengthFormatUtcWithoutZ);

			var off = string.Format("{0:00}:{1:00}", mins / 60, mins % 60);
			var res = $"{strNoZ}{(neg ? '-' : '+')}{off}";

			return res;
		}

		public static UtcOffsetTime ParseCosmosDb(string utc, short offsetMinutes)
		{
			var utcs = UtcTime.ParseCosmosDb(utc);
			return new UtcOffsetTime(utcs, offsetMinutes);
		}

		internal DateTimeOffset ToDateTimeOffset()
		{
			var local = _utc.AddMinutes(_offsetMins).UtcDateTime;

			return new DateTimeOffset(DateTime.SpecifyKind(local, DateTimeKind.Local), TimeSpan.FromMinutes(OffsetMins));
		}

		public override int GetHashCode() => _utc.GetHashCode();
		
		public override bool Equals(object obj) => obj is UtcOffsetTime other && Equals(other);

		public bool Equals(UtcOffsetTime other) => this._utc == other._utc;
		
		public int CompareTo(UtcOffsetTime other) => this._utc.CompareTo(other._utc);

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((UtcOffsetTime)obj);
		}

		public static bool operator ==(UtcOffsetTime a, UtcOffsetTime b) => a._utc == b._utc;
		public static bool operator !=(UtcOffsetTime a, UtcOffsetTime b) => a._utc != b._utc;
		
	}
}
