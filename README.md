# CosmosTime
A restricted set of time structs, UtcTime and UtcOffsetTime, trying to fix some of the problems with DateTime\DateTimeOffset and be nice to CosmosDB that require a specific fixed length format. Point is to only support only ISO 8601 with CultureInfo.InvariantCulture for parse and formatting.  
