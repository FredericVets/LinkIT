using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class SpecialOwnerWriteModel
	{
		public string CreatedBy { get; set; }

		public string ModifiedBy { get; set; }

		[Required]
		[MaxLength(30)]
		public string Name { get; set; }

		public string Remark { get; set; }
	}
}