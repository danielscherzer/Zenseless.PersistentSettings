using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Zenseless.PersistentSettings.Tests
{
	[TestClass()]
	public class PersistentSettingsTests
	{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
		class Test : IEquatable<Test>
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
		{
			public Test()
			{
			}

			public Test(Test test)
			{
				CurrentFile = test.CurrentFile;
				ObservableItems = new(test.ObservableItems);
				Items = new(test.Items);
				Left = test.Left;
				TopMost = test.TopMost;
			}

			public Test(string currentFile, ObservableCollection<string> observableItems, double left, bool topMost, List<string> items)
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
				return Equals(other as Test);
			}

			public bool Equals(Test? other)
			{
				if (other is null) return false;
				if (CurrentFile != other.CurrentFile || Left != other.Left || TopMost != other.TopMost) return false;
				if (!Enumerable.SequenceEqual(ObservableItems, other.ObservableItems)) return false;
				if (!Enumerable.SequenceEqual(Items, other.Items)) return false;
				return true;
			}

			public override string ToString() => $"'{CurrentFile}'; {Left}; {TopMost}; <{string.Join(',', ObservableItems)}>; <{string.Join(',', Items)}>";
		}

		//class Data<TType>
		//{
		//	public TType Value { get; internal set; }
		//	public override string ToString() => $"{Value}";
		//}
		//Data<TType> Create<TType>(TType value) => new Data<TType> { Value = value };

		[TestMethod()]
		public void FullTest()
		{
			//_ = Tracker.Configure<MainWindow>().Id(w => nameof(MainWindow), SystemParameters.PrimaryScreenWidth)
			//	.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
			//Tracker.Track(window);

			PersistentSettings settings = new();

			//var left = Create(1234.456);
			Test expected = new("myFile", new ObservableCollection<string>() { "oldFile", "olderFile" }, 1234.456, true, new List<string>() { "item1", });
			Test actual = new(expected);
			Assert.AreEqual(expected, actual);

			//settings.AddFromProperty(() => left.Value);
			settings.AddFromProperty(() => actual.Left);
			settings.AddFromProperty(() => actual.Items);
			settings.AddFromProperty(() => actual.ObservableItems);
			//settings.AddFromGetterSetter(nameof(actual.ObservableItems), () => actual.ObservableItems, value => actual.ObservableItems = value);
			settings.AddFromProperty(() => actual.CurrentFile);
			settings.AddFromProperty(() => actual.TopMost);

			settings.Store();

			//left.Value = 0.0;

			actual.Clear();
			Assert.AreEqual(new Test(), actual);

			settings.Load();
			Assert.AreEqual(expected, actual);
			//Assert.AreEqual(left, Create(1234.456));
		}
	}
}