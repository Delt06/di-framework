using System;
using System.Linq;
using System.Reflection;
using Framework.Dependencies;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Plugins.Framework.Editor
{
	[CustomEditor(typeof(RootDependencyContainer))]
	public sealed class RootDependencyContainerCustomEditor : UnityEditor.Editor
	{
		private void OnEnable()
		{
			_types = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetExportedTypes())
				.Where(FilterTypes)
				.Prepend(null)
				.ToArray();
			
			_typeNames = _types
				.Select(t => t?.Name ?? "Add Container")
				.ToArray();
		}

		private static bool FilterTypes(Type type)
		{
			return typeof(IDependencyContainer).IsAssignableFrom(type) &&
			       typeof(MonoBehaviour).IsAssignableFrom(type) &&
			       type != typeof(RootDependencyContainer) && 
			       !type.IsAbstract;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var selected = EditorGUILayout.Popup(0, _typeNames);
			if (selected == 0) return;
			var container = (RootDependencyContainer) serializedObject.targetObject;
			container.gameObject.AddComponent(_types[selected]);
			EditorUtility.SetDirty(container.gameObject);
		}

		private string[] _typeNames = Array.Empty<string>();
		private Type[] _types = Array.Empty<Type>();
	}
}