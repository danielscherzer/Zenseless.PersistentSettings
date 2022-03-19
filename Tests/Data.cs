using System;
using System.Collections;

namespace Zenseless.PersistentSettings.Tests
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
	internal class Data<TType> : IEquatable<Data<TType>>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
	{
		public Data(TType value) => Value = value;

		public TType Value { get; internal set; }
		public override bool Equals(object? other)
		{
			if (ReferenceEquals(this, other)) return true;
			if (other?.GetType() != GetType()) return false;
			return Equals(other as Data<TType>);
		}

		public bool Equals(Data<TType>? other)
		{
			if (other is null) return false;
			if (Value is null) return (other.Value is null);
			if (typeof(TType).IsSubclassOf(typeof(IEnumerable))) return StructuralComparisons.StructuralEqualityComparer.Equals(Value, other.Value);
			if (!Value.Equals(other.Value)) return false;
			return true;
		}
		public override string ToString() => $"{Value}";
	}
}