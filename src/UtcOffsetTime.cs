using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	public struct UtcOffsetTime : IEquatable<UtcOffsetTime>, IComparable<UtcOffsetTime>, IComparable
	{
		UtcTime _utc;
		short _offsetMins;

		//DateTimeOffset _dto;

		public static UtcOffsetTime Now => DateTimeOffset.Now.ToUtcOffsetTime();

		public UtcTime Utc => _utc;// _dto.UtcDateTime.ToUtcTime();

		/// <summary>
		/// Offset from Utc
		/// </summary>
		public int OffsetMins => _offsetMins;// (int)_dto.Offset.TotalMinutes;

		// PArse: allow inly local+offset

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

			// [+-]14h
			if (offsetMinutes < -840 || offsetMinutes > 840)
				throw new ArgumentException("offset can be max [+-] 14 hours");

			_offsetMins = offsetMinutes;
		}

		/// <summary>
		/// Variable length
		/// </summary>
		/// <returns></returns>
		public string ToUtcOffsetString()
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

			//			if (res.Length != 33)
			//			throw new Exception("not 33 chars");

			return res;
			//.ToString()..UtcDateTime.ToString("o", CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// minutes
		/// </summary>


		public override string ToString() => ToUtcOffsetString();

		// TODO: parse time+offset?

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

		public override int GetHashCode()
		{
			return _utc.GetHashCode();
		}

		public override bool Equals(object obj) => obj is UtcOffsetTime other && Equals(other);


		public bool Equals(UtcOffsetTime other)
		{
			return this._utc == other._utc;// && this._offsetMins == other._offsetMins;
		}

		public int CompareTo(UtcOffsetTime other)
		{
			return this._utc.CompareTo(other._utc);
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj is null)
			{
				return 1;
			}
			return CompareTo((UtcOffsetTime)obj);
		}

		public static bool operator ==(UtcOffsetTime a, UtcOffsetTime b)
		{
			return a._utc == b._utc;// && a._offsetMins == b._offsetMins;
		}
		public static bool operator !=(UtcOffsetTime a, UtcOffsetTime b)
		{
			return a._utc != b._utc;// || a._offsetMins != b._offsetMins;
		}


		//public object ToString(string v)
		//{
		//	return _dto.ToString(v, CultureInfo.InvariantCulture);
		//}
	}
}
