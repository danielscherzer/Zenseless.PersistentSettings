using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Zenseless.PersistentSettings
{
	/// <summary>
	/// Code adapted from: https://stackoverflow.com/questions/23017716/json-net-how-to-deserialize-without-using-the-default-constructor
	/// </summary>
	internal class DynamicObjectResolver : DefaultContractResolver
	{
		public string ConstructorAttributeName { get; set; } = "JsonConstructorAttribute";
		public bool IgnoreAttributeConstructor { get; set; } = false;
		public bool IgnoreSinglePrivateConstructor { get; set; } = false;
		public bool IgnoreMostSpecificConstructor { get; set; } = false;

		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			var contract = base.CreateObjectContract(objectType);

			// Use default contract for non-object types.
			if (objectType.IsPrimitive || objectType.IsEnum) return contract;

			// Look for constructor with attribute first, then single private, then most specific.
			var overrideConstructor =
				   (IgnoreAttributeConstructor ? null : GetAttributeConstructor(objectType))
				?? (IgnoreSinglePrivateConstructor ? null : GetSinglePrivateConstructor(objectType))
				?? (IgnoreMostSpecificConstructor ? null : GetMostSpecificConstructor(objectType));

			// Set override constructor if found, otherwise use default contract.
			if (overrideConstructor != null)
			{
				SetOverrideCreator(contract, overrideConstructor);
			}

			return contract;
		}

		private void SetOverrideCreator(JsonObjectContract contract, ConstructorInfo attributeConstructor)
		{
			contract.OverrideCreator = CreateParameterizedConstructor(attributeConstructor);
			contract.CreatorParameters.Clear();
			foreach (var constructorParameter in base.CreateConstructorParameters(attributeConstructor, contract.Properties))
			{
				contract.CreatorParameters.Add(constructorParameter);
			}
		}

		private static ObjectConstructor<object> CreateParameterizedConstructor(MethodBase method)
		{
			return method is ConstructorInfo ci ? (a => ci.Invoke(a)) : (a => method.Invoke(null, a));
		}

		protected virtual ConstructorInfo? GetAttributeConstructor(Type objectType)
		{
			var constructors = objectType
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(c => c.GetCustomAttributes().Any(a => a.GetType().Name == ConstructorAttributeName)).ToList();

			if (constructors.Count == 1) return constructors[0];
			if (constructors.Count > 1)
				throw new JsonException($"Multiple constructors with a {this.ConstructorAttributeName}.");

			return null;
		}

		protected virtual ConstructorInfo? GetSinglePrivateConstructor(Type objectType)
		{
			var constructors = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);

			return constructors.Length == 1 ? constructors[0] : null;
		}

		protected virtual ConstructorInfo? GetMostSpecificConstructor(Type objectType)
		{
			var constructors = objectType
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.OrderBy(e => e.GetParameters().Length);

			return constructors.LastOrDefault();
		}
	}
}
