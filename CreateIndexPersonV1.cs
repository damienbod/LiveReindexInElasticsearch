using System;
using System.Diagnostics;
using System.Linq;
using ElasticsearchCRUD;
using ElasticsearchCRUD.Utils;
using LiveReindexInElasticsearch.SQLDomainModel;

namespace LiveReindexInElasticsearch
{
	public class CreateIndexPersonV1
	{
		private readonly Stopwatch _stopwatch = new Stopwatch();
		readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();

		public CreateIndexPersonV1()
		{
			_elasticsearchMappingResolver.AddElasticSearchMappingForEntityType(typeof(Person), MappingUtils.GetElasticsearchMapping("persons_v1"));
		}

		public void SaveToElasticsearchPerson()
		{
			using (var elasticsearchContext = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				//ElasticsearchContext.TraceProvider = new ConsoleTraceProvider();
				using (var modelPerson = new ModelPerson())
				{
					int pointer = 0;
					const int interval = 1500;
					int length = modelPerson.Person.Count();

					while (pointer < length)
					{
						_stopwatch.Start();
						var collection = modelPerson.Person.OrderBy(t => t.BusinessEntityID).Skip(pointer).Take(interval).ToList();
						_stopwatch.Stop();
						Console.WriteLine("Time taken for select {0} persons: {1}", interval,_stopwatch.Elapsed);
						_stopwatch.Reset();

						foreach (var item in collection)
						{
							elasticsearchContext.AddUpdateDocument(item, item.BusinessEntityID);
						}

						_stopwatch.Start();
						elasticsearchContext.SaveChanges();
						_stopwatch.Stop();
						Console.WriteLine("Time taken to insert {0} person documents: {1}", interval, _stopwatch.Elapsed);
						_stopwatch.Reset();
						pointer = pointer + interval;
						Console.WriteLine("Transferred: {0} items", length);
					}
				}
			}
		}

		public void CreatePersonAliasForPersonV1Mapping(string alias)
		{
			using (var context = new ElasticsearchContext("http://localhost:9200/", _elasticsearchMappingResolver))
			{
				context.AliasCreateForIndex(alias, _elasticsearchMappingResolver.GetElasticSearchMapping(typeof(Person)).GetIndexForType(typeof(Person)));
			}
		}
	}
}
