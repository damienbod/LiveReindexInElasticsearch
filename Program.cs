using System;
using LiveReindexInElasticsearch.Reindex;

namespace LiveReindexInElasticsearch
{
	class Program
	{
		static void Main(string[] args)
		{
			var createIndexPersonV1 = new CreateIndexPersonV1();	
	
			// CREATE NEW INDEX person_v1 for Person Entity class
			//createIndexPersonV1.SaveToElasticsearchPerson();
			//Console.WriteLine("Created new index person_v1 in elasticsearch");

			//Console.ReadLine();

			// CREATE NEW ALIAS person for INDEX  person_v1 
			//createIndexPersonV1.CreatePersonAliasForPersonV1Mapping();
			//Console.WriteLine("Created new alias person for index person_v1 in elasticsearch");

			//Console.ReadLine();

			// CREATE NEW INDEX person_v2 from INDEX person_v1 


		}
	}
}
