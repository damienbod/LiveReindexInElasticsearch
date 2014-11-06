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

		public static long GetKeyMethod(Person person)
		{
			return person.BusinessEntityID;
		}
	}
}
