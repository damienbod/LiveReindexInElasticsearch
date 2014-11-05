using System;
using System.Text;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextSearch;
using LiveReindexInElasticsearch.SQLDomainModel;

namespace LiveReindexInElasticsearch.Reindex
{
	public class ReindexPersonV1ToPersonV2
	{
		private readonly ElasticsearchContext _context;
		private const int IndexerSize = 500;

		public ReindexPersonV1ToPersonV2()
		{
			IElasticsearchMappingResolver elasticsearchMappingResolver = new ElasticsearchMappingResolver();		
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(PersonV2), new PersonV2IndexTypeMapping());
			elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Person), new PersonV1IndexTypeMapping());

			_context = new ElasticsearchContext("http://localhost.fiddler:9200/", elasticsearchMappingResolver);
		}

		public void SwitchAliasfromPersonV1IndexToPersonV2Index()
		{
			_context.AliasReplaceIndex("persons", "persons_v1", "persons_v2");
		}

		public void Reindex(DateTime beginDateTime)
		{
			var result = _context.SearchCreateScanAndScroll<Person>(BuildSearchModifiedDateTimeLessThan(beginDateTime),
				new ScanAndScrollConfiguration(1, TimeUnits.Minute, 500));

			var scrollId = result.PayloadResult;
			Console.WriteLine("Total Hits in scan: {0}", result.TotalHits);

			int indexPointer = 0;
			while (result.TotalHits > indexPointer - IndexerSize)
			{
				Console.WriteLine("creating new documents, indexPointer: {0} Hits: {1}", indexPointer, result.TotalHits);

				var resultCollection = _context.Search<Person>(BuildSearchModifiedDateTimeLessThan(beginDateTime, BuildSearchFromTooForScanScroll(indexPointer, IndexerSize)),
					scrollId);

				foreach (var item in resultCollection.PayloadResult)
				{
					_context.AddUpdateDocument(CreatePersonV2FromPerson(item), item.BusinessEntityID);
				}
				_context.SaveChanges();
				indexPointer = indexPointer + IndexerSize;
			}
		}

		public void ReindexUpdateChangesWhileReindexing(DateTime beginDateTime)
		{
			var result = _context.SearchCreateScanAndScroll<Person>(BuildSearchModifiedDateTimeGreaterThan(beginDateTime),
				new ScanAndScrollConfiguration(1, TimeUnits.Minute, IndexerSize));

			var scrollId = result.PayloadResult;
			Console.WriteLine("Total Hits in scan: {0}", result.TotalHits);

			int indexPointer = 0;
			while (result.TotalHits > indexPointer - IndexerSize)
			{
				Console.WriteLine("creating new documents, indexPointer: {0} Hits: {1}", indexPointer, result.TotalHits);

				var resultCollection = _context.Search<Person>(BuildSearchModifiedDateTimeGreaterThan(beginDateTime,BuildSearchFromTooForScanScroll(indexPointer, IndexerSize)),
					scrollId);

				foreach (var item in resultCollection.PayloadResult)
				{
					_context.AddUpdateDocument(CreatePersonV2FromPerson(item), item.BusinessEntityID);
				}
				_context.SaveChanges();
				indexPointer = indexPointer + IndexerSize;
			}
		}

		private PersonV2 CreatePersonV2FromPerson(Person item)
		{
			return new PersonV2
			{
				BusinessEntityID = item.BusinessEntityID,
				PersonType = item.PersonType,
				NameStyle = item.NameStyle,
				Title = item.Title,
				FirstName = item.FirstName,
				MiddleName = item.MiddleName,
				LastName = item.LastName,
				Suffix = item.Suffix,
				EmailPromotion = item.EmailPromotion,
				AdditionalContactInfo = item.AdditionalContactInfo,
				Demographics = item.Demographics,
				rowguid = item.rowguid,
				ModifiedDate = item.ModifiedDate,
				Deleted = false
			};
		}

		//{   
		//   "from" : 100 , "size" : 100
		//}
		private string BuildSearchFromTooForScanScroll(int from, int size)
		{
			return "\"from\" : " + from + ", \"size\" : " + size + ",";
		}

		private string BuildSearchModifiedDateTimeLessThan(DateTime dateTimeUtc, string addFromSize = "")
		{
			return BuildSearchRange("lt", "modifieddate", dateTimeUtc, addFromSize);
		}

		private string BuildSearchModifiedDateTimeGreaterThan(DateTime dateTimeUtc, string addFromSize = "")
		{
			return BuildSearchRange("gte", "modifieddate", dateTimeUtc, addFromSize);
		}

		//{
		//   "query" :  {
		//	   "range": {  "modifieddate": { "lt":   "2003-12-29T00:00:00"  } }
		//	}
		//}
		private string BuildSearchRange(string lessThanOrGreaterThan, string updatePropertyName, DateTime dateTimeUtc, string addFromToSize)
		{
			string isoDateTime = dateTimeUtc.ToString("s");
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			if (!string.IsNullOrEmpty(addFromToSize))
			{
				buildJson.AppendLine(addFromToSize);
			}
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"range\": {  \"" + updatePropertyName + "\": { \"" + lessThanOrGreaterThan + "\":   \"" + isoDateTime + "\"  } }");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}
	}
}


