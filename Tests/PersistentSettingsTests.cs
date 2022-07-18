using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Zenseless.PersistentSettings.Tests
{
	[TestClass()]
	public partial class PersistentSettingsTests
	{
		[TestMethod()]
		public void AddFromGetterSetterTest()
		{
			AddFromGetterSetterTestHelper(12.38, 0.0);
			AddFromGetterSetterTestHelper(1, 0);
			AddFromGetterSetterTestHelper(true, false);
			AddFromGetterSetterTestHelper("", "7");
			AddFromGetterSetterTestHelper("testtest", "");
			AddFromGetterSetterTestHelper(new List<string> { }, new List<string>());
			AddFromGetterSetterTestHelper(new string[] { "testtest" }, Enumerable.Empty<string>());
			AddFromGetterSetterTestHelper(new Dictionary<int, int> { [1] = 2, [5] = 3 }, new Dictionary<int, int>());
		}

		public static void AddFromGetterSetterTestHelper<TType>(TType expected, TType tempValue)
		{
			PersistentSettings settings = new();
			TType value = expected;
			//Assert.ThrowsException<InvalidOperationException>(() => settings.AddFromProperty(() => value));
			settings.AddFromGetterSetter(nameof(value), () => value, v => value = v);
			settings.Store();
			value = tempValue;
			Helper.AreEqual(tempValue, value);
			settings.Load();
			Helper.AreEqual(expected, value);
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
			public RoInputC() => value = 4.7f;
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