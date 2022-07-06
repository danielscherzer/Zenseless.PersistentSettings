using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zenseless.PersistentSettings.Tests
{
	[TestClass()]
	public partial class PersistentSettingsTests
	{
		private static Data<TType> Create<TType>(TType value) => new(value);

		[TestMethod()]
		public void FullTest()
		{
			//_ = Tracker.Configure<MainWindow>().Id(w => nameof(MainWindow), SystemParameters.PrimaryScreenWidth)
			//	.WhenPersistingProperty((wnd, property) => property.Cancel = WindowState.Normal != wnd.WindowState)
			//Tracker.Track(window);

			PersistentSettings settings = new();

			//var left = Create(1234.456);
			BigData expected = new("myFile", new ObservableCollection<string>() { "oldFile", "olderFile" }, 1234.456, true, new List<string>() { "item1", });
			BigData actual = new(expected);
			Assert.AreEqual(expected, actual);

			//settings.AddFromProperty(() => left.Value);
			settings.AddFromProperty(() => actual.Left);
			settings.AddFromProperty(() => actual.Items);
			settings.AddFromProperty(() => actual.ObservableItems);
			settings.AddFromProperty(() => actual.CurrentFile);
			settings.AddFromProperty(() => actual.TopMost);

			settings.Store();

			//left.Value = 0.0;

			actual.Clear();
			Assert.AreEqual(new BigData(), actual);

			settings.Load();
			Assert.AreEqual(expected, actual);
			//Assert.AreEqual(left, Create(1234.456));
		}

		[DataTestMethod()]
		public void AddFromPropertyTest()
		{
			AddFromPropertyTestHelper(12, 0);
			AddFromPropertyTestHelper(1234.456, 0.0);
			AddFromPropertyTestHelper(true, false);
			AddFromPropertyTestHelper("", "7");
			AddFromPropertyTestHelper("testtest", "");
			//AddFromPropertyTestHelper(new List<string> { }, new List<string>());
			//AddFromPropertyTestHelper(new string[] { "testtest" }, Enumerable.Empty<string>());
		}

		public static void AddFromPropertyTestHelper<TType>(TType expected, TType tempValue)
		{
			PersistentSettings settings = new();
			var actualData = Create(expected);
			settings.AddFromProperty(() => actualData.Value);
			settings.Store();
			actualData.Value = tempValue;
			Assert.IsNotNull(tempValue);
			Assert.IsNotNull(expected);
			Assert.IsTrue(tempValue.Equals(actualData.Value));
			//Assert.AreEqual(tempValue, actualData.Value);
			settings.Load();
			//Assert.AreEqual(expected, actualData.Value);
			Assert.IsTrue(expected.Equals(actualData.Value));
		}

		[TestMethod()]
		public void AddFromGetterSetterTest()
		{
			AddFromGetterSetterTestHelper(12.38, 0.0);
			AddFromGetterSetterTestHelper(1, 0);
			AddFromGetterSetterTestHelper(true, false);
			AddFromGetterSetterTestHelper("", "7");
			AddFromGetterSetterTestHelper("testtest", "");
		}

		public static void AddFromGetterSetterTestHelper<TType>(TType expected, TType tempValue)
		{
			PersistentSettings settings = new();
			TType value = expected;
			//Assert.ThrowsException<InvalidOperationException>(() => settings.AddFromProperty(() => value));
			settings.AddFromGetterSetter(nameof(value), () => value, v => value = v);
			settings.Store();
			value = tempValue;
			settings.Load();
			Assert.AreEqual(expected, value);
		}

		struct Input { public float value; };

		[TestMethod()]
		public void AddFromGetterSetterStructTest()
		{
			Input expected = new() { value = 0.4f };
			Input temp = new() { value = -0.9f };
			AddFromGetterSetterTestHelper(expected, temp);
		}

		readonly struct RoInput
		{
			public readonly float value;
			public RoInput(float value) => this.value = value;
		}
		[TestMethod()]
		public void AddFromGetterSetterReadonlyStructTest()
		{
			RoInput expected = new(0.4f);
			RoInput temp = new(-0.9f);
			AddFromGetterSetterTestHelper(expected, temp);
		}

		readonly struct RoInputC
		{
			public readonly float value;
			public RoInputC() => this.value = 4.7f;
			public RoInputC(float value) => this.value = value;
		}
		[TestMethod()]
		public void AddFromGetterSetterReadonlyStructDefaultConstructorTest()
		{
			RoInputC expected = new(0.4f);
			RoInputC temp = new(-0.9f);
			AddFromGetterSetterTestHelper(expected, temp);
		}
	}
}