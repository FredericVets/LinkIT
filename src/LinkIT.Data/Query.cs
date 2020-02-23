namespace LinkIT.Data
{
	public abstract class Query
	{
		public long? Id { get; set;}

		public Paging Paging { get; set; }
	}
}