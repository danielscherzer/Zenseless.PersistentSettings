﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zenseless.PersistentSettings.Tests
{
	public partial class PersistentSettingsTests
	{
		[DataTestMethod()]
		public void AddFromPropertyTest()
		{
			AddFromPropertyTestHelper(12, 0);
			AddFromPropertyTestHelper(1234.456, 0.0);
			AddFromPropertyTestHelper(true, false);
			AddFromPropertyTestHelper("", "7");
			AddFromPropertyTestHelper("testtest", "");
			AddFromPropertyTestHelper(new List<string> { }, new List<string>());
			AddFromPropertyTestHelper(new string[] { "testtest" }, Enumerable.Empty<string>());
			AddFromPropertyTestHelper(new Dictionary<int, int> { [1] = 2 }, new Dictionary<int, int>());
		}

		public static void AddFromPropertyTestHelper<TType>(TType expected, TType tempValue)
		{
			PersistentSettings settings = new();
			Data<TType> actualData = new(expected);
			settings.AddFromProperty(() => actualData.Value);
			settings.Store();
			actualData.Value = tempValue;
			Assert.IsNotNull(tempValue);
			Assert.IsNotNull(expected);

			Compare(tempValue, actualData.Value);
			settings.Load();
			Compare(expected, actualData.Value);
		}

		private static void Compare<TType>(TType expected, TType actual)
		{
			if (expected is ICollection expectedEnum && actual is ICollection actualEnum)
			{
				CollectionAssert.AreEqual(expectedEnum, actualEnum);
			}
			else
			{
				Assert.AreEqual(expected, actual);
			}
		}
	}
}