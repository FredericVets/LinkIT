using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class SpecialOwnerFilterModel
	{
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		[MaxLength(30)]
		public string Name { get; set; }

		[MaxLength(30)]
		public string Remark { get; set; }

		public bool IsEmpty() =>
			string.IsNullOrWhiteSpace(CreatedBy) &&
			string.IsNullOrWhiteSpace(ModifiedBy) &&
			string.IsNullOrWhiteSpace(Name) &&
			string.IsNullOrWhiteSpace(Remark);
	}
}