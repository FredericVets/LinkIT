using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class ProductFilterModel
	{
		/// <summary>
		/// Filter by 'CreatedBy'.
		/// </summary>
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		/// <summary>
		/// Filter by 'ModifiedBy'.
		/// </summary>
		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		/// <summary>
		/// Filter by 'Brand'.
		/// </summary>
		[MaxLength(30)]
		public string Brand { get; set; }

		/// <summary>
		/// Filter by 'Type'.
		/// </summary>
		[MaxLength(30)]
		public string Type { get; set; }

		/// <summary>
		/// Filter by 'Group'.
		/// </summary>
		[MaxLength(30)]
		public string Group { get; set; }

		public bool IsEmpty() =>
			string.IsNullOrWhiteSpace(CreatedBy) &&
			string.IsNullOrWhiteSpace(ModifiedBy) &&
			string.IsNullOrWhiteSpace(Brand) &&
			string.IsNullOrWhiteSpace(Type) &&
			string.IsNullOrWhiteSpace(Group);
	}
}