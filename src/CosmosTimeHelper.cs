using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosTime
{
	public static class CosmosTimeHelper
	{
		public static DateTime? ToUtcDateTime(this UtcTime? utc)
		{
			if (utc == null)
				return null;
			return utc.Value.UtcDateTime;
		}


		public static UtcTime ToUtcTime(this DateTime dt)
		{
			return new UtcTime(dt.ToUniversalTime());
		}

		public static UtcTime? ToUtcTime(this DateTime? dt)
		{
			if (dt == null)
				return null;
			return new UtcTime(dt.Value.ToUniversalTime());
		}

		public static UtcOffsetTime ToUtcOffsetTime(this DateTimeOffset dto)
		{
			return new UtcOffsetTime(dto);
		}


	}
}
