using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Zenseless.PersistentSettings
{
	/// <summary>
	/// Class for persisting (saving permanently) settings
	/// </summary>
	public class PersistentSettings
	{
		/// <summary>
		/// Load settings from a file
		/// </summary>
		/// <param name="fileName">The path to the file the settings are read from.</param>
		public void Load(string fileName = "")
		{
			fileName = string.IsNullOrWhiteSpace(fileName) ? defaultFileName : fileName;
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

		/// <summary>
		/// Add a new setting form a given setter and getter
		/// </summary>
		/// <typeparam name="TType"></typeparam>
		/// <param name="key">THe unique name of the setting.</param>
		/// <param name="getter">The getter used to store the setting.</param>
		/// <param name="setter">The setter used to load the setting.</param>
		/// <exception cref="ArgumentException"></exception>
		public void AddFromGetterSetter<TType>(string key, Func<TType> getter, Action<TType> setter)
		{
			if (settings.ContainsKey(key)) throw new ArgumentException($"Setting with key:{key} already exists.");
			settings.Add(key, Prop.Create(getter, setter));
		}

		/// <summary>
		/// Add a new setting form a given property. The property name needs to be unique among the settings or you have to specify a new key.
		/// </summary>
		/// <typeparam name="ValueType"></typeparam>
		/// <param name="propertyAccessExpression"></param>
		/// <param name="key">The unique name of the setting. If left empty the property name is used as key.</param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public void AddFromProperty<ValueType>(Expression<Func<ValueType>> propertyAccessExpression, string key = "")
		{
			if (propertyAccessExpression.Body is MemberExpression memberExpression)
			{
				if (memberExpression?.Member is PropertyInfo propertyInfo)
				{
					if (memberExpression.Expression is null) throw new ArgumentException("Invalid expression given");
					var instance = memberExpression.Expression.EvaluateInstance();
					key = string.IsNullOrWhiteSpace(key) ? propertyInfo.Name : key;
					AddFromGetterSetter(key,
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
		/// <param name="fileName">The path to the file the settings are written to. If none is provided the settings are stored to %appdata%/Roaming/<callingAssemblyName></callingAssemblyName>/"settings.json"</param>
		public void Store(string fileName = "")
		{
			var dic = settings.ToDictionary(prop => prop.Key, prop => prop.Value.Getter());
			var text = JsonConvert.SerializeObject(dic, Formatting.Indented);

			fileName = string.IsNullOrWhiteSpace(fileName) ? defaultFileName : fileName;
			Directory.CreateDirectory(Path.GetDirectoryName(fileName) ?? string.Empty);
			File.WriteAllText(fileName, text);
		}

		private readonly Dictionary<string, Prop> settings = new();
		private readonly string defaultFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetCallingAssembly()?.GetName().Name ?? string.Empty, "settings.json");
	}
}
