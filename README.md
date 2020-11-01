# CosmosTime
A restricted set of time structs, UtcTime and UtcOffsetTime, trying to fix some of the problems with DateTime\DateTimeOffset and be nice to CosmosDB that require a specific fixed length format. Point is to only support only ISO 8601 with CultureInfo.InvariantCulture for parse and formatting.  

Also a goal is to make parsing and formatting more restricted:  

Parsing:

UtcTime: only allow parsing formats in Zulu or with local+UtcOffset:  
2020-10-27T10:59:54Z -> Kind.Utc  
2020-10-27T10:59:54+00:10  -> Kind.Utc  
Do not allow parsing formats in local\unspecified time:  
2020-10-27T10:59:54 -> Kind.Unspecified  
  
UtcOffsetTime: only allow parsing formats in Zulu or with local+UtcOffset:  
2020-10-27T10:59:54Z -> offset 0  
2020-10-27T10:59:54+00:10  -> offset 10min  
Do not allow parsing formats in local\unspecified time:  
2020-10-27T10:59:54  
  
Formats with local+UtcOffset does not work in CosmosDB because of lexical search, so to format for CosmosDB  
you must use UtcOffsetTimeCosmosDbJsonConverter that will format like this:  
"myTime":
{  
 "time": "2009-06-15T13:45:30.0000000Z",  
 "offsetMins": 30
}  

