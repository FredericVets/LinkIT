using System;
using System.Collections.Generic;

namespace LinkIT.Data.DTO
{
	public abstract class Dto : IEquatable<Dto>
	{
		public long? Id { get; set; }

		public static bool operator ==(Dto left, Dto right) =>
			EqualityComparer<Dto>.Default.Equals(left, right);

		public static bool operator !=(Dto left, Dto right) => 
			!(left == right);

		public override bool Equals(object obj) => 
			Equals(obj as Dto);

		public override int GetHashCode() => 
			2108858624 + Id.GetHashCode();

		public bool Equals(Dto other) => 
			other != null && Id == other.Id;
	}
}