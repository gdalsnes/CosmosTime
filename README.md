# CosmosTime
A restricted set of time structs, UtcTime and UtcOffsetTime, trying to fix some of the problems with DateTime\DateTimeOffset and be nice to CosmosDB that require a specific fixed length format ("yyyy-MM-ddTHH:mm:ss.fffffffZ") for range queries to work. New date and time system functions in Azure Cosmos DB return this fixed length format (but unfortunately they allow parsing various length formats with\without Z, opening up for hiding bugs).

Point also to only support only ISO 8601 with CultureInfo.InvariantCulture for parse and formatting.  

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

Some collected links with info\problems about times in Newtonsoft\CosmosDb:

https://stackoverflow.com/questions/63444977/convert-string-to-datetime-in-cosmos-db

Json.NET interprets and modifies ISO dates when deserializing to JObject (DateParseHandling.None)
https://github.com/JamesNK/Newtonsoft.Json/issues/862

Default serialization of DateTime is not sortable
https://github.com/Azure/azure-cosmos-dotnet-v3/issues/1468

Most applications can use the default string representation for DateTime?
https://github.com/MicrosoftDocs/azure-docs/issues/65079

The docs seem to be completely unaware of all the problems:
https://docs.microsoft.com/en-us/azure/cosmos-db/working-with-dates

Please specify DateTime conversion fully
https://github.com/MicrosoftDocs/azure-docs/issues/19284

LINQ not honoring JsonSerializerSettings
https://github.com/Azure/azure-cosmos-dotnet-v2/issues/351

Unable to save property with value that looks like a date in Azure Cosmos DB Graph (Gremlin)
https://stackoverflow.com/questions/48304478/unable-to-save-property-with-value-that-looks-like-a-date-in-azure-cosmos-db-gra

[BUG] Date-like metadata is being parsed and serialized
https://github.com/Azure/azure-libraries-for-net/issues/1126

DateTimeOffset ignores offset during deserialisation, uses LOCAL TIME ZONE offset
https://github.com/Azure/azure-cosmos-dotnet-v2/issues/235

Saving and reading DateTimeOffset in DocumentDB
https://github.com/Azure/azure-cosmos-dotnet-v2/issues/197

DateTimeOffsets get converted to local time when deserialized
https://github.com/Azure/azure-cosmos-dotnet-v2/issues/292

Add DateTimeOffset support 
https://feedback.azure.com/forums/263030-azure-cosmos-db/suggestions/16838656-add-datetimeoffset-support

Sorting and filtering by DateTime in Azure Cosmos DB SQL Queries
https://codingazure.net/?p=62

BulkImportAsync does not respect JsonSerializerSettings
https://github.com/Azure/azure-cosmos-dotnet-v2/issues/601

Times displayed incorrectly (CosmosDbExplorer)
https://github.com/sachabruttin/CosmosDbExplorer/issues/44

Bug in DateTime handling of Cosmos DB DocumentClient (linq query generation)
https://stackoverflow.com/questions/63112044/bug-in-datetime-handling-of-cosmos-db-documentclient

LINQ queries with DateTime values are broken
https://github.com/Azure/azure-cosmos-dotnet-v3/issues/1732

Formatting date in Linq-to-Entities query causes exception
https://stackoverflow.com/questions/5839388/formatting-date-in-linq-to-entities-query-causes-exception

Datetime fields - ISO vs non-ISO - How does the behaviour of Cosmos DB change? 
https://github.com/MicrosoftDocs/azure-docs/issues/52598

Do not convert JToken date time string as DateTime (JToken.Parse, DateParseHandling)
https://stackoverflow.com/questions/33874334/do-not-convert-jtoken-date-time-string-as-datetime
