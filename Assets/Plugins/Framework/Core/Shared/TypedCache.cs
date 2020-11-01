using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Framework.Core.Shared
{
	internal sealed class TypedCache
	{
		public bool TryGet<T>(out T obj) where T : class
		{
			if (TryGet(typeof(T), out var foundObject))
			{
				obj = (T) foundObject;
				return true;
			}

			obj = default;
			return false;
		}

		public bool TryGet([NotNull] Type type, out object obj)
		{
			obj = default;

			if (!_objects.TryGetValue(type, out var foundObject))
				return false;

			if (!TryFindInSubclasses(type, out foundObject))
				return false;

			obj = foundObject;
			return true;
		}

		private bool TryFindInSubclasses([NotNull] Type type, out object obj)
		{
			foreach (var existingObject in _allObjects)
			{
				var typeOfExistingObject = existingObject.GetType();
				if (!type.IsAssignableFrom(typeOfExistingObject)) continue;

				obj = existingObject;
				_objects[type] = existingObject;
				return true;
			}

			obj = default;
			return false;
		}

		public bool TryRegister([NotNull] object obj, out object existingObject)
		{
			if (obj == null) throw new ArgumentNullException(nameof(obj));

			var type = obj.GetType();
			if (_objects.TryGetValue(type, out existingObject))
				return false;

			_allObjects.Add(obj);
			_objects[type] = obj;
			return true;
		}

		private readonly HashSet<object> _allObjects = new HashSet<object>();
		private readonly IDictionary<Type, object> _objects = new Dictionary<Type, object>();
	}
}