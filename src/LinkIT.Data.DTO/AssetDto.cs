using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	/// <summary>
	/// Asset has a "Deleted" column, that is used instead of physically deleting records.
	/// Soft Delete.
	/// </summary>
	public class AssetDto : Dto, IEquatable<AssetDto>
	{
		public DateTime? CreationDate { get; set; }

		public string CreatedBy { get; set; }

		public DateTime? ModificationDate { get; set; }

		public string ModifiedBy { get; set; }

		public string IctsReference { get; set; }

		public string Tag { get; set; }

		public string Serial { get; set; }

		public ProductDto Product { get; set; }

		public string Description { get; set; }

		public DateTime? InvoiceDate { get; set; }

		public string InvoiceNumber { get; set; }

		public decimal? Price { get; set; }

		public string PaidBy { get; set; }

		public string Owner { get; set; }

		public DateTime? InstallDate { get; set; }

		public string InstalledBy { get; set; }

		public string Remark { get; set; }

		public bool TeamAsset { get; set; }

		public static bool operator ==(AssetDto left, AssetDto right)
		{
			return EqualityComparer<AssetDto>.Default.Equals(left, right);
		}

		public static bool operator !=(AssetDto left, AssetDto right) => !(left == right);

		public override bool Equals(object obj) => Equals(obj as AssetDto);

		public override int GetHashCode()
		{
			var hashCode = -133760364;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + CreationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CreatedBy);
			hashCode = hashCode * -1521134295 + ModificationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModifiedBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(IctsReference);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Serial);
			hashCode = hashCode * -1521134295 + EqualityComparer<ProductDto>.Default.GetHashCode(Product);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Description);
			hashCode = hashCode * -1521134295 + InvoiceDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InvoiceNumber);
			hashCode = hashCode * -1521134295 + Price.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PaidBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Owner);
			hashCode = hashCode * -1521134295 + InstallDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(InstalledBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Remark);
			hashCode = hashCode * -1521134295 + TeamAsset.GetHashCode();

			return hashCode;
		}

		public bool Equals(AssetDto other)
		{
			return other != null &&
				base.Equals(other) &&
				CreationDate == other.CreationDate &&
				CreatedBy == other.CreatedBy &&
				ModificationDate == other.ModificationDate &&
				ModifiedBy == other.ModifiedBy &&
				IctsReference == other.IctsReference &&
				Tag == other.Tag &&
				Serial == other.Serial &&
				EqualityComparer<ProductDto>.Default.Equals(Product, other.Product) &&
				Description == other.Description &&
				InvoiceDate == other.InvoiceDate &&
				InvoiceNumber == other.InvoiceNumber &&
				Price == other.Price &&
				PaidBy == other.PaidBy &&
				Owner == other.Owner &&
				InstallDate == other.InstallDate &&
				InstalledBy == other.InstalledBy &&
				Remark == other.Remark &&
				TeamAsset == other.TeamAsset;
		}

		// Supplies a shallow copy.
		public AssetDto Copy()
		{
			return (AssetDto)MemberwiseClone();
		}
	}
}