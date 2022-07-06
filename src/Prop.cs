using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Zenseless.PersistentSettings
{
	internal struct Prop
	{
		public Func<object?> Getter { get; }
		public Action<object> Setter { get; }

		public Prop(Func<object?> getter, Action<object> setter) : this()
		{
			Getter = getter;
			Setter = setter;
		}

		public static Prop Create<TType>(Func<TType> getter, Action<TType> setter)
		{
			return new Prop(() => getter(), value =>
			{
				TType? result;
				if (value is JToken token)
				{
					result = token.ToObject<TType>(serializer);
					if (result is not null) setter(result);
				}
				else
				{
					result = (TType)Convert.ChangeType(value, typeof(TType), CultureInfo.InvariantCulture);
				}
				if (result is not null) setter(result);
			});
		}

		private static readonly JsonSerializer serializer = new() { CheckAdditionalContent = true, ContractResolver = new DynamicObjectResolver() };
	}
}
