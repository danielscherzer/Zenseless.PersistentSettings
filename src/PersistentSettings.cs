using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zenseless.PersistentSettings
{
	public class PersistentSettings
	{
		/// <summary>
		/// Load settings from a file
		/// </summary>
		public void Load()
		{
			if (File.Exists(fileName))
			{
				//load values from file
				var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(fileName));
				if (dic is not null) foreach ((string key, object value) in dic)
					{
						if (settings.TryGetValue(key, out var accessors))
						{
							accessors.Setter(value);
						}
					}
			}
		}

		public void AddFromGetterSetter<TType>(string name, Func<TType> getter, Action<TType> setter)
		{
			//TODO:check if(settings.ContainsKey(name))
			settings.Add(name, Prop.Create(getter, setter));
		}

		public void AddFromProperty<ValueType>(Expression<Func<ValueType>> propertyAccessExpression)
		{
			if (propertyAccessExpression.Body is MemberExpression memberExpression)
			{
				if (memberExpression?.Member is PropertyInfo propertyInfo)
				{
					if (memberExpression.Expression is null) throw new ArgumentException("Invalid expression given");
					var instance = memberExpression.Expression.EvaluateInstance();
					AddFromGetterSetter(propertyInfo.Name,
						() => (ValueType)(propertyInfo.GetValue(instance) ?? throw new ArgumentException("Expression has wrong type")),
						value => propertyInfo.SetValue(instance, value));
					return;
				}
			}
			throw new InvalidOperationException("Please provide a valid property expression, like '() => instance.PropertyName'.");
		}

		/// <summary>
		/// Save the settings to a file
		/// </summary>
		public void Store()
		{
			var dic = settings.ToDictionary(prop => prop.Key, prop => prop.Value.Getter());
			var text = JsonConvert.SerializeObject(dic, Formatting.Indented);
			File.WriteAllText(fileName, text);
		}

		private readonly Dictionary<string, Prop> settings = new();
		private readonly string fileName = Path.ChangeExtension(Assembly.GetCallingAssembly().Location, "settings.json");
	}
}
