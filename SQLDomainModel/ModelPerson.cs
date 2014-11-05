using System.Data.Entity;

namespace LiveReindexInElasticsearch.SQLDomainModel
{
	public partial class ModelPerson : DbContext
	{
		public ModelPerson()
			: base("name=ModelPerson")
		{
			//this.Configuration.LazyLoadingEnabled = false;
		}


		public virtual DbSet<Person> Person { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{

			modelBuilder.Entity<Person>()
				.Property(e => e.PersonType)
				.IsFixedLength();

		}
	}
}
