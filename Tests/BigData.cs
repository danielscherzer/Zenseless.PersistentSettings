using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Zenseless.PersistentSettings.Tests
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
	internal class BigData : IEquatable<BigData>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
	{
		public BigData()
		{
		}

		public BigData(BigData test)
		{
			CurrentFile = test.CurrentFile;
			ObservableItems = new(test.ObservableItems);
			Items = new(test.Items);
			Left = test.Left;
			TopMost = test.TopMost;
		}

		public BigData(string currentFile, ObservableCollection<string> observableItems, double left, bool topMost, List<string> items)
		{
			CurrentFile = currentFile;
			ObservableItems = observableItems;
			Left = left;
			TopMost = topMost;
			Items = items;
		}

		public string CurrentFile { get; internal set; } = "";
		public ObservableCollection<string> ObservableItems { get; internal set; } = new();
		public List<string> Items { get; internal set; } = new();
		public double Left { get; internal set; }
		public bool TopMost { get; internal set; }

		public void Clear()
		{
			CurrentFile = "";
			Left = default;
			TopMost = default;
			ObservableItems = new();
			Items = new();
		}

		public override bool Equals(object? other)
		{
			if (ReferenceEquals(this, other)) return true;
			if (other?.GetType() != GetType()) return false;
			return Equals(other as BigData);
		}

		public bool Equals(BigData? other)
		{
			if (other is null) return false;
			if (CurrentFile != other.CurrentFile || Left != other.Left || TopMost != other.TopMost) return false;
			if (!Enumerable.SequenceEqual(ObservableItems, other.ObservableItems)) return false;
			if (!Enumerable.SequenceEqual(Items, other.Items)) return false;
			return true;
		}

		public override string ToString() => $"'{CurrentFile}'; {Left}; {TopMost}; <{string.Join(',', ObservableItems)}>; <{string.Join(',', Items)}>";
	}
}