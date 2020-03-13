using System;

namespace LinkIT.Data.Queries
{
	public class SpecialOwnerQuery : Query
	{
		public DateTime? CreationDate { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModificationDate { get; set; }

		public string ModifiedBy { get; set; }

		public string Name { get; set; }

		public string Remark { get; set; }
	}
}