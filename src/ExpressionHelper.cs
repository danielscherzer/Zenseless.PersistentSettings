using System.Linq.Expressions;
using System.Reflection;

namespace Zenseless.PersistentSettings
{
	internal static class ExpressionHelper
	{
		internal static object? EvaluateInstance(this Expression e)
		{
			switch (e.NodeType)
			{
				case ExpressionType.Constant:
					return (e as ConstantExpression)?.Value;
				case ExpressionType.MemberAccess:
					{
						if (e is not MemberExpression propertyExpression) return null;
						var field = propertyExpression.Member as FieldInfo;
						var property = propertyExpression.Member as PropertyInfo;
						var container = propertyExpression.Expression == null ? null : EvaluateInstance(propertyExpression.Expression);
						if (field != null)
							return field.GetValue(container);
						else if (property != null)
							return property.GetValue(container, null);
						else
							return null;
					}
				default:
					return null;
			}
		}
	}
}
