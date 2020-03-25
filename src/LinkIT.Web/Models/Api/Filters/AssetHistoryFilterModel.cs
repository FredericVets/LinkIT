using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class AssetHistoryFilterModel
	{
		[Range(0, long.MaxValue)]
		public long? AssetId { get; set; }

		[MaxLength(15)]
		public string Tag { get; set; }

		public bool IsEmpty() =>
			!AssetId.HasValue &&
			string.IsNullOrWhiteSpace(Tag);
	}
}