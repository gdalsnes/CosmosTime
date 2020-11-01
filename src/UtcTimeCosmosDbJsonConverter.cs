using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CosmosTime
{
	/// <summary>
	/// Format\parse fixed length "yyyy-MM-ddTHH:mm:ss.fffffffZ"
	/// </summary>
	public class UtcTimeCosmosDbJsonConverter : JsonConverter<UtcTime>
	{
		public override UtcTime ReadJson(JsonReader reader, Type objectType, UtcTime existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			return UtcTime.ParseCosmosDb((string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, UtcTime value, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var utc = (UtcTime)value;

			writer.WriteValue(utc.ToCosmosDbString());
		}
	}
}
