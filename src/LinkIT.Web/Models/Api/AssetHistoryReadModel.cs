namespace LinkIT.Web.Models.Api
{
	public class AssetHistoryReadModel : BaseAssetReadModel
	{
		public long AssetId { get; set; }

		public long ProductId { get; set; }
	}
}