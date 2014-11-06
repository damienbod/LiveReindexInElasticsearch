using System;
using System.Diagnostics;
using ElasticsearchCRUD;
using ElasticsearchCRUD.ContextSearch;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using LiveReindexInElasticsearch.Reindex;
using LiveReindexInElasticsearch.SQLDomainModel;

namespace LiveReindexInElasticsearch
{
	class Program
	{
		static void Main(string[] args)
		{
			var createIndexPersonV1 = new CreateIndexPersonV1();

			#region Setup initial index, not required usually because the index should already exist...
			//// CREATE NEW INDEX person_v1 for Person Entity class
			//createIndexPersonV1.SaveToElasticsearchPerson();
			//Console.WriteLine("Created new index person_v1 in elasticsearch");

			////Console.ReadLine();

			// CREATE NEW ALIAS person for INDEX  persons_v1 
			createIndexPersonV1.CreatePersonAliasForPersonV1Mapping("persons");
			Console.WriteLine("Created new alias person for index person_v1 in elasticsearch");

			#endregion  Setup initial index, not required usually because the index should already exist...

			// The following 3 steps show you how to do a reindex in elasticsearch using scroll and an alias with no downtime

			// ---------------------------------------------------------
			// STEP 1: CREATE NEW INDEX persons_v2 from INDEX persons_v1 
			// ---------------------------------------------------------
			// - This timestamp is usually DateTime.UtcNow. 
			// - It is required so all the indexes which were updated during the reindex can be found
			DateTime beginDateTime = DateTime.UtcNow.AddYears(-7);

			var reindex = new ElasticsearchCrudReindex<Person, PersonV2>(
				new IndexTypeDescription("persons_v1", "person"), 
				new IndexTypeDescription("persons_v2", "person"), 
				"http://localhost:9200");

			reindex.ScanAndScrollConfiguration = new ScanAndScrollConfiguration(5,TimeUnits.Second, 1000);
			reindex.TraceProvider = new ConsoleTraceProvider(TraceEventType.Information);

			reindex.Reindex(
				PersonReindexConfiguration.BuildSearchModifiedDateTimeLessThan(beginDateTime), 
				PersonReindexConfiguration.GetKeyMethod, 
				PersonReindexConfiguration.CreatePersonV2FromPerson);

			Console.WriteLine("Created new index from version 1 index");

			// ---------------------------------------------------------
			// STEP 2: REPLACE ALIAS persons TO INDEX persons_v2 
			// ---------------------------------------------------------
			reindex.SwitchAliasfromOldToNewIndex("persons");

			Console.WriteLine("Replace index for alias");

			// ---------------------------------------------------------
			// STEP 3: NOW GET ALL THE DOCUMENTS WHICH WERE UPDATED WHILE REINDEXING
			// ---------------------------------------------------------
			// NOTE: if the document is updated again in the meantime, it will be overwitten with this method. 
			// If required, you must check the update timestamp of the item in the new index!
			reindex.Reindex(
				PersonReindexConfiguration.BuildSearchModifiedDateTimeGreaterThan(beginDateTime), 
				PersonReindexConfiguration.GetKeyMethod, 
				PersonReindexConfiguration.CreatePersonV2FromPerson);

			Console.WriteLine("Replace index for person documents which were updating while reindexing");

			Console.ReadLine();
		}

		
	}
}


// TODO use object for the key
// TODO set the alias log to info