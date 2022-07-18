namespace Zenseless.PersistentSettings.Tests
{
	internal class Data<TType>
	{
		public Data(TType value) => Value = value;

		public TType Value { get; internal set; }
		public override string ToString() => $"{Value}";
	}
}