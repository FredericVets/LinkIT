using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class SpecialOwnerWriteModel
	{
		[MaxLength(30)]
		public string CreatedBy { get; set; }

		[MaxLength(30)]
		public string ModifiedBy { get; set; }

		[Required]
		[MaxLength(30)]
		public string Name { get; set; }

		public string Remark { get; set; }
	}
}