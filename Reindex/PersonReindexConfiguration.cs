using System;
using System.Text;
using LiveReindexInElasticsearch.SQLDomainModel;

namespace LiveReindexInElasticsearch.Reindex
{
	public static class PersonReindexConfiguration
	{
		public static PersonV2 CreatePersonV2FromPerson(Person item)
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

		public static object GetKeyMethod(Person person)
		{
			return person.BusinessEntityID;
		}

		public static string BuildSearchModifiedDateTimeLessThan(DateTime dateTimeUtc)
		{
			return BuildSearchRange("lt", "modifieddate", dateTimeUtc);
		}

		public static string BuildSearchModifiedDateTimeGreaterThan(DateTime dateTimeUtc)
		{
			return BuildSearchRange("gte", "modifieddate", dateTimeUtc);
		}

		//{
		//   "query" :  {
		//	   "range": {  "modifieddate": { "lt":   "2003-12-29T00:00:00"  } }
		//	}
		//}
		private static string BuildSearchRange(string lessThanOrGreaterThan, string updatePropertyName, DateTime dateTimeUtc)
		{
			string isoDateTime = dateTimeUtc.ToString("s");
			var buildJson = new StringBuilder();
			buildJson.AppendLine("{");
			buildJson.AppendLine("\"query\": {");
			buildJson.AppendLine("\"range\": {  \"" + updatePropertyName + "\": { \"" + lessThanOrGreaterThan + "\":   \"" + isoDateTime + "\"  } }");
			buildJson.AppendLine("}");
			buildJson.AppendLine("}");

			return buildJson.ToString();
		}
	}
}
