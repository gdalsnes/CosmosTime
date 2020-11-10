using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	public static class CosmosTimeHelper
	{

		/// <summary>
		/// DateTime must already be UTC
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime ToUtcTime(this DateTime dt)
		{
			return new UtcTime(dt);
		}

		/// <summary>
		/// DateTime must already be UTC
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime? ToUtcTime(this DateTime? dt)
		{
			if (dt == null)
				return null;
			return new UtcTime(dt.Value);
		}

		/// <summary>
		/// Will convert from local to UTC. Should ideally never be used!
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime LocalToUtcTime(this DateTime dt)
		{
			return new UtcTime(dt.ToUniversalTime());
		}

		/// <summary>
		/// Will convert from local to UTC. Should ideally never be used!
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static UtcTime? LocalToUtcTime(this DateTime? dt)
		{
			if (dt == null)
				return null;
			return new UtcTime(dt.Value.ToUniversalTime());
		}

		public static UtcOffsetTime ToUtcOffsetTime(this DateTimeOffset dto)
		{
			return new UtcOffsetTime(dto);
		}

		public static UtcOffsetTime? ToUtcOffsetTime(this DateTimeOffset? dto)
		{
			if (dto == null)
				return null;
			return new UtcOffsetTime(dto.Value);
		}

		public static DateTime? ToUtcDateTime(this UtcTime? utc)
		{
			if (utc == null)
				return null;
			return utc.Value.UtcDateTime;
		}
	}
}
