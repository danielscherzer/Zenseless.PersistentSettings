//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using OpenTK.Mathematics;
//using System.Collections.Generic;

//namespace Zenseless.OpenTK.Tests
//{
//	[TestClass()]
//	public class Box2ExtensionsTests
//	{
//		[TestMethod()]
//		public void CreateFromMinSizeTest()
//		{
//			var min = new Vector2(-1, 1f);
//			var max = new Vector2(0f, 2f);
//			var result = Box2Extensions.CreateFromMinSize(min, max - min);
//			var expected = new Box2(min, max);
//			Assert.AreEqual(expected, result);
//		}

//		[TestMethod()]
//		public void CreateFromMinSize2Test()
//		{
//			var min = new Vector2(-1, 1f);
//			var max = new Vector2(0f, 2f);
//			var size = max - min;
//			var result = Box2Extensions.CreateFromMinSize(min.X, min.Y, size.X, size.Y);
//			var expected = new Box2(min, max);
//			Assert.AreEqual(expected, result);
//		}

//		[DataTestMethod()]
//		[DataRow(-1, -1, -1, -1, -1, -1, -1, -1, false)] //same empty box
//		[DataRow(-1, -1, 0, 0, -1, -1, 0, 0, true)] //same box
//		[DataRow(-1, -1, 1, 2, -1, -1, 1, 2, true)] // overlapping
//		[DataRow(-1, -1, 1, 3, 1, -2, 4, -1, false)] // share single point
//		[DataRow(-1, -1, 1, 3, 1, -1, 4, 0, false)] // share single line
//		[DataRow(-1, -1, 1, 3, 0.99f, -1, 4, 0, true)] // slightly overlapping
//		[DataRow(-1, -1, 1, 3, 0f, -1, 4, -0.99f, true)] // slightly overlapping
//		public void OverlapTest(float aminX, float aminY, float amaxX, float amaxY, float bminX, float bminY, float bmaxX, float bmaxY, bool expected)
//		{
//			Box2 boxA = new(aminX, aminY, amaxX, amaxY);
//			Box2 boxB = new(bminX, bminY, bmaxX, bmaxY);
//			Assert.AreEqual(expected, boxA.Overlaps(boxB));
//			Assert.AreEqual(expected, boxB.Overlaps(boxA));
//		}

//		public static IEnumerable<object[]> GetData()
//		{
//			var a = Box2Extensions.CreateFromMinSize(0f, 0.5f, 1f, 1f);

//			yield return new object[] { a, a.Translated(1, 0), a };
//			yield return new object[] { a, a.Translated(0.5f, 0), a.Translated(-0.5f, 0) };
//			yield return new object[] { a, a.Translated(0, 0.5f), a.Translated(0, -0.5f) };
//			yield return new object[] { a, a.Translated(0.5f, 0.5f), a.Translated(-0.5f, 0) };
//			yield return new object[] { a, a.Translated(-0.5f, 0), a.Translated(0.5f, 0) };
//			yield return new object[] { a, a.Translated(0, -0.5f), a.Translated(0, 0.5f) };
//			yield return new object[] { a, a.Translated(-0.5f, -0.5f), a.Translated(0.5f, 0) };
//		}

//		[DataTestMethod()]
//		[DynamicData(nameof(GetData), DynamicDataSourceType.Method)]
//		public void UndoOberlapTest(Box2 boxA, Box2 boxB, Box2 expected)
//		{
//			var actual = Box2Extensions.UndoOberlap(boxA, boxB);
//			Assert.AreEqual(expected, actual);
//		}
//	}
//}