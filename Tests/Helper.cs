using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace Zenseless.PersistentSettings.Tests
{
	internal class Helper
	{
		//TODO: private static void Compare<TType>(TType expected, TType actual) where TType : ICollection
		//{
		//	CollectionAssert.AreEqual(expected, actual);
		//}

		internal static void AreEqual<TType>(TType expected, TType actual)
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
