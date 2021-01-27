using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class SpecialOwnerFilterModel
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
		/// Filter by 'Name'.
		/// </summary>
		[MaxLength(30)]
		public string Name { get; set; }

		/// <summary>
		/// Filter by 'Remark'.
		/// </summary>
		[MaxLength(30)]
		public string Remark { get; set; }

		public bool IsEmpty() =>
			string.IsNullOrWhiteSpace(CreatedBy) &&
			string.IsNullOrWhiteSpace(ModifiedBy) &&
			string.IsNullOrWhiteSpace(Name) &&
			string.IsNullOrWhiteSpace(Remark);
	}
}