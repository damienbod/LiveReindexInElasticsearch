using System;
using ElasticsearchCRUD;

namespace LiveReindexInElasticsearch.Reindex
{
	public class PersonV1IndexTypeMapping : ElasticsearchMapping
	{
		public override string GetIndexForType(Type type)
		{
			return "persons_v1";
		}
	}
}