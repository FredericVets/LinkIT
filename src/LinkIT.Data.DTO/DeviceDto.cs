using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	public class DeviceDto : Dto, IEquatable<DeviceDto>
	{
		public string Tag { get; set; }

		public string Owner { get; set; }

		public string Brand { get; set; }

		public string Type { get; set; }

		public static bool operator ==(DeviceDto left, DeviceDto right)
		{
			return EqualityComparer<DeviceDto>.Default.Equals(left, right);
		}

		public static bool operator !=(DeviceDto left, DeviceDto right) => !(left == right);

		public override bool Equals(object obj) => Equals(obj as DeviceDto);

		public override int GetHashCode()
		{
			var hashCode = -1165835544;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tag);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Owner);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Brand);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);

			return hashCode;
		}

		public bool Equals(DeviceDto other)
		{
			return other != null &&
				   base.Equals(other) &&
				   Tag == other.Tag &&
				   Owner == other.Owner &&
				   Brand == other.Brand &&
				   Type == other.Type;
		}
	}
}