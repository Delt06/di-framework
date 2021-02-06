using System;

namespace DELTation.DIFramework
{
	internal static class DependencyExceptionFactory
	{
		public static Exception NotRegistered(Type type) =>
			throw new InvalidOperationException($"Dependency of type {type} is not registered.");

		public static Exception AlreadyRegistered(Type type, object registeredDependency) =>
			throw new InvalidOperationException(
				$"Dependency of type {type} is already registered: {registeredDependency}.");
	}
}