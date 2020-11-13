using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Framework.Dependencies
{
	public static class InjectionChecking
	{
		public static bool AreInjectable([NotNull] this ParameterInfo[] parameters)
		{
			if (parameters == null) throw new ArgumentNullException(nameof(parameters));

			foreach (var parameter in parameters)
			{
				if (!parameter.IsInjectable())
					return false;
			}

			return true;
		}

		private static bool IsInjectable([NotNull] this ParameterInfo parameter)
		{
			if (parameter == null) throw new ArgumentNullException(nameof(parameter));
			var parameterType = parameter.ParameterType;
			return !parameterType.IsValueType && !parameter.IsOut &&
			       !parameter.IsIn && !parameterType.IsByRef;
		}
	}
}