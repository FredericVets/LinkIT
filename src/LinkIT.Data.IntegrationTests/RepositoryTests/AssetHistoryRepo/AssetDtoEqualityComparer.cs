using LinkIT.Data.DTO;
using System.Collections.Generic;

namespace LinkIT.Data.IntegrationTests.RepositoryTests.AssetHistoryRepo
{
	/// <summary>
	/// Should not take the ID property into account.
	/// For the Product property, only the Id should be taken into account.
	/// </summary>
	internal class AssetDtoForHistoryEqualityComparer : IEqualityComparer<AssetDto>
	{
		public bool Equals(AssetDto x, AssetDto y)
		{
			if (ReferenceEquals(x, y))
				return true;

			if (x == null)
				return false;

			if (y == null)
				return false;

			return x.CreationDate == y.CreationDate &&
				x.CreatedBy == y.CreatedBy &&
				x.ModificationDate == y.ModificationDate &&
				x.ModifiedBy == y.ModifiedBy &&
				x.IctsReference == y.IctsReference &&
				x.Tag == y.Tag &&
				x.Serial == y.Serial &&
				x.Product.Id == y.Product.Id &&
				x.Description == y.Description &&
				x.InvoiceDate == y.InvoiceDate &&
				x.InvoiceNumber == y.InvoiceNumber &&
				x.Price == y.Price &&
				x.PaidBy == y.PaidBy &&
				x.Owner == y.Owner &&
				x.InstallDate == y.InstallDate &&
				x.InstalledBy == y.InstalledBy &&
				x.Remark == y.Remark &&
				x.TeamAsset == y.TeamAsset;
		}

		public int GetHashCode(AssetDto obj)
		{
			if (obj == null)
				return -1;

			var hashCode = -133760364;
			hashCode = hashCode * -1521134295 + obj.CreationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.CreatedBy);
			hashCode = hashCode * -1521134295 + obj.ModificationDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.ModifiedBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.IctsReference);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Tag);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Serial);
			hashCode = hashCode * -1521134295 + obj.Product.Id.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Description);
			hashCode = hashCode * -1521134295 + obj.InvoiceDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.InvoiceNumber);
			hashCode = hashCode * -1521134295 + obj.Price.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.PaidBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Owner);
			hashCode = hashCode * -1521134295 + obj.InstallDate.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.InstalledBy);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(obj.Remark);
			hashCode = hashCode * -1521134295 + obj.TeamAsset.GetHashCode();

			return hashCode;
		}
	}
}