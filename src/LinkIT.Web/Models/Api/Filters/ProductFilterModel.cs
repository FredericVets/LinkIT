using System;
using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api.Filters
{
	public class ProductFilterModel
	{
		public DateTime? CreationDate { get; set; }

		[MaxLength(30)]
		public string CreatedBy { get; set; }

		public DateTime? ModificationDate { get; set; }

		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		[MaxLength(30)]
		public string Brand { get; set; }

		[MaxLength(30)]
		public string Type { get; set; }

		public bool IsEmpty()
		{
			return !CreationDate.HasValue &&
				string.IsNullOrWhiteSpace(CreatedBy) &&
				!ModificationDate.HasValue &&
				string.IsNullOrWhiteSpace(ModifiedBy) &&
				string.IsNullOrWhiteSpace(Brand) &&
				string.IsNullOrWhiteSpace(Type);
		}
	}
}