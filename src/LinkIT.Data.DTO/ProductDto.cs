using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	public class ProductDto : Dto, IEquatable<ProductDto>
	{
		public DateTime? CreationDate { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModificationDate { get; set; }

		public string ModifiedBy { get; set; }

		public string Brand { get; set; }

		public string Type { get; set; }

		public static bool operator ==(ProductDto left, ProductDto right)
		{
			return EqualityComparer<ProductDto>.Default.Equals(left, right);
		}

		public static bool operator !=(ProductDto left, ProductDto right) => !(left == right);

		public override bool Equals(object obj) => Equals(obj as ProductDto);

		public override int GetHashCode()
		{
			var hashCode = -307904605;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + CreationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CreatedBy);
			hashCode = hashCode * -1521134295 + ModificationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifiedBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Brand);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);

			return hashCode;
		}

		public bool Equals(ProductDto other)
		{
			return other != null &&
				base.Equals(other) &&
				CreationDate == other.CreationDate &&
				CreatedBy == other.CreatedBy &&
				ModificationDate == other.ModificationDate &&
				ModifiedBy == other.ModifiedBy &&
				Brand == other.Brand &&
				Type == other.Type;
		}
	}
}