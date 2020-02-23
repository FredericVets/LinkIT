using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	public class DeviceDto : IEquatable<DeviceDto>
	{
		public long? Id { get; set; }

		public string Tag { get; set; }

		public string Owner { get; set; }

		public string Brand { get; set; }

		public string Type { get; set; }

		public void ValidateRequiredFields(bool forInsert = false)
		{
			if (!forInsert)
				if (!Id.HasValue)
					throw new ArgumentNullException("Id is required");

			if (string.IsNullOrWhiteSpace(Tag))
				throw new ArgumentNullException("Tag is required");

			if (string.IsNullOrWhiteSpace(Owner))
				throw new ArgumentNullException("Owner is required");

			if (string.IsNullOrWhiteSpace(Brand))
				throw new ArgumentNullException("Brand is required");

			if (string.IsNullOrWhiteSpace(Type))
				throw new ArgumentNullException("Type is required");
		}

		// Auto-generated equality methods.
		public override bool Equals(object obj)
		{
			return Equals(obj as DeviceDto);
		}

		public bool Equals(DeviceDto other)
		{
			return other != null &&
				   EqualityComparer<long?>.Default.Equals(Id, other.Id) &&
				   Tag == other.Tag &&
				   Owner == other.Owner &&
				   Brand == other.Brand &&
				   Type == other.Type;
		}

		public override int GetHashCode()
		{
			var hashCode = -2085744510;

			hashCode = hashCode * -1521134295 + Id.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Owner);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Brand);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);

			return hashCode;
		}

		public static bool operator ==(DeviceDto left, DeviceDto right)
		{
			return EqualityComparer<DeviceDto>.Default.Equals(left, right);
		}

		public static bool operator !=(DeviceDto left, DeviceDto right)
		{
			return !(left == right);
		}
	}
}