using System;
using LiveReindexInElasticsearch.Reindex;

namespace LiveReindexInElasticsearch
{
	class Program
	{
		static void Main(string[] args)
		{
			var createIndexPersonV1 = new CreateIndexPersonV1();

			#region Setup initial index, not required usually cause the index should already exist...
			//// CREATE NEW INDEX person_v1 for Person Entity class
			//createIndexPersonV1.SaveToElasticsearchPerson();
			//Console.WriteLine("Created new index person_v1 in elasticsearch");

			////Console.ReadLine();

			// CREATE NEW ALIAS person for INDEX  persons_v1 
			createIndexPersonV1.CreatePersonAliasForPersonV1Mapping();
			Console.WriteLine("Created new alias person for index person_v1 in elasticsearch");

			#endregion  Setup initial index, not required usually cause the index should already exist...

			//Console.ReadLine();

			// STEP 1: CREATE NEW INDEX persons_v2 from INDEX persons_v1 

			DateTime beginDateTime = DateTime.UtcNow.AddYears(-7);
			var reindex = new ReindexPersonV1ToPersonV2();
			reindex.Reindex(beginDateTime);
			Console.WriteLine("Created new index from version 1 index");

			// STEP 2: REPLACE ALIAS persons TO INDEX persons_v2 
			reindex.SwitchAliasfromPersonV1IndexToPersonV2Index();
			Console.WriteLine("Replace index for alias");

			// STEP 3: TODO invalidate the _scroll_id

			// STEP 4: NOW GET ALL THE DOCUMENTS WHICH WERE UPDATED WHILE REINDEXING
			reindex.ReindexUpdateChangesWhileReindexing(beginDateTime);
			Console.WriteLine("Replace index for person documents which were updating while reindexing");
		}
	}
}
