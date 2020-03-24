using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	public class SpecialOwnerDto : Dto, IEquatable<SpecialOwnerDto>
	{
		public DateTime? CreationDate { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModificationDate { get; set; }

		public string ModifiedBy { get; set; }

		public string Name { get; set; }

		public string Remark { get; set; }

		public static bool operator ==(SpecialOwnerDto left, SpecialOwnerDto right) =>
			EqualityComparer<SpecialOwnerDto>.Default.Equals(left, right);

		public static bool operator !=(SpecialOwnerDto left, SpecialOwnerDto right) => 
			!(left == right);

		public override bool Equals(object obj) => 
			Equals(obj as SpecialOwnerDto);

		public override int GetHashCode()
		{
			var hashCode = -781278249;

			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + CreationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CreatedBy);
			hashCode = hashCode * -1521134295 + ModificationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifiedBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Remark);

			return hashCode;
		}

		public bool Equals(SpecialOwnerDto other) =>
			other != null &&
			base.Equals(other) &&
			CreationDate == other.CreationDate &&
			CreatedBy == other.CreatedBy &&
			ModificationDate == other.ModificationDate &&
			ModifiedBy == other.ModifiedBy &&
			Name == other.Name &&
			Remark == other.Remark;
	}
}