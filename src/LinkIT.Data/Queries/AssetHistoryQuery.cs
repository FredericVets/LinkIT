namespace LinkIT.Data.Queries
{
	public class AssetHistoryQuery : Query
	{
		public long? AssetId { get; set; }

		public string Tag { get; set; }
	}
}