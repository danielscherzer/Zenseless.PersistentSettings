using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zenseless.PersistentSettings.Tests
{
	public partial class PersistentSettingsTests
	{
		[TestMethod()]
		public void BigDataTest()
		{
			PersistentSettings settings = new();

			BigData expected = new("myFile", new ObservableCollection<string>() { "oldFile", "olderFile" }, 1234.456, true, new List<string>() { "item1", });
			BigData actual = new(expected);
			Assert.AreEqual(expected, actual);

			settings.AddFromProperty(() => actual.Left);
			settings.AddFromProperty(() => actual.Items);
			settings.AddFromProperty(() => actual.ObservableItems);
			settings.AddFromProperty(() => actual.CurrentFile);
			settings.AddFromProperty(() => actual.TopMost);

			settings.Store();

			actual.Clear();
			Assert.AreEqual(new BigData(), actual);

			settings.Load();
			Assert.AreEqual(expected, actual);
		}
	}
}