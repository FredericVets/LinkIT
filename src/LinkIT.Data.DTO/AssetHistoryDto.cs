using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	public class AssetHistoryDto : AssetDto, IEquatable<AssetHistoryDto>
	{
		public long AssetId { get; set; }

		public static bool operator ==(AssetHistoryDto left, AssetHistoryDto right) =>
			EqualityComparer<AssetHistoryDto>.Default.Equals(left, right);

		public static bool operator !=(AssetHistoryDto left, AssetHistoryDto right) =>
			!(left == right);

		public override bool Equals(object obj) =>
			Equals(obj as AssetHistoryDto);

		public override int GetHashCode()
		{
			var hashCode = 874387948;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + AssetId.GetHashCode();

			return hashCode;
		}

		public bool Equals(AssetHistoryDto other) =>
			other != null &&
			base.Equals(other) &&
			AssetId == other.AssetId;
	}
}