using System;
using ElasticsearchCRUD;

namespace LiveReindexInElasticsearch.Reindex
{
	public class PersonV2IndexTypeMapping : ElasticsearchMapping
	{
		public override string GetIndexForType(Type type)
		{
			return "persons_v2";
		}

		public override string GetDocumentType(Type type)
		{
			return "person";
		}
	}
}