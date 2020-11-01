using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosTime
{
	/// <summary>
	/// Format\parse composite format 
	/// { 
	///   "time": "yyyy-MM-ddTHH:mm:ss.fffffffZ", // fixed length
	///   "offsetMins": {mins}
	/// }
	/// </summary>
	public class UtcOffsetTimeCosmosDbJsonConverter : JsonConverter<UtcOffsetTime>
	{
		public override UtcOffsetTime ReadJson(JsonReader reader, Type objectType, UtcOffsetTime existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var obj = JObject.Load(reader);
			return UtcOffsetTime.ParseCosmosDb(obj["time"].Value<string>(), obj["offsetMins"].Value<short>());
		}

		public override void WriteJson(JsonWriter writer, UtcOffsetTime value, JsonSerializer serializer)
		{
			if (serializer.DateParseHandling != DateParseHandling.None)
				throw new NotSupportedException("DateParseHandling.None required");

			var rr = (UtcOffsetTime)value;

			writer.WriteStartObject();

			writer.WritePropertyName("time");
			writer.WriteValue(rr.Utc.ToCosmosDbString());

			writer.WritePropertyName("offsetMins");
			writer.WriteValue(rr.OffsetMins);

			writer.WriteEndObject();
		}
	}
}
