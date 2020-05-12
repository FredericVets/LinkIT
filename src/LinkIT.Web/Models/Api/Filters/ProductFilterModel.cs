using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class ProductFilterModel
	{
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		[MaxLength(30)]
		public string Brand { get; set; }

		[MaxLength(30)]
		public string Type { get; set; }

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