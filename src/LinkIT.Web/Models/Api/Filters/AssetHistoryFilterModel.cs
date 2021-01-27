using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class AssetHistoryFilterModel
	{
		/// <summary>
		/// Filter by 'AssetId'.
		/// </summary>
		[Range(0, long.MaxValue)]
		public long? AssetId { get; set; }

		/// <summary>
		/// Filter by 'Tag'.
		/// </summary>
		[MaxLength(15)]
		public string Tag { get; set; }

		public bool IsEmpty() =>
			!AssetId.HasValue &&
			string.IsNullOrWhiteSpace(Tag);
	}
}