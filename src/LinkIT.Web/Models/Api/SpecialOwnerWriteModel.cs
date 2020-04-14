using System.ComponentModel.DataAnnotations;

namespace LinkIT.Web.Models.Api
{
	public class SpecialOwnerWriteModel
	{
		[Required]
		[MaxLength(30)]
		public string Name { get; set; }

		[MaxLength(30)]
		public string Remark { get; set; }
	}
}