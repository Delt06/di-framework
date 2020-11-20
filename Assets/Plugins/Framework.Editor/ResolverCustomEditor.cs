using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework;
using UnityEditor;
using UnityEngine;
using static Framework.Resolution;

namespace Plugins.Framework.Editor
{
	[CustomEditor(typeof(Resolver))]
	public class ResolverCustomEditor : UnityEditor.Editor
	{
		private GUIStyle _headerStyle;
		private bool _foldout;

		private void OnEnable()
		{
			_headerStyle = new GUIStyle
			{
				normal = new GUIStyleState
				{
					textColor = Color.white
				},
				richText = true
			};
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var resolver = (Resolver) serializedObject.targetObject;
			var components = GetAffectedComponents(resolver.transform).ToArray();

			EditorGUILayout.Space();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("<size=14><b>Dependencies</b></size>:", _headerStyle);
			GUILayout.FlexibleSpace();
			var resolutionText = GetResolutionText(resolver, components.Select(c => c.component));
			GUILayout.Box(resolutionText, _headerStyle);
			GUILayout.EndHorizontal();

			_foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_foldout, "Components");

			if (_foldout)
				foreach (var (component, depth) in components)
				{
					DrawComponent(resolver, component, depth);
				}

			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private static string GetResolutionText(Resolver resolver, IEnumerable<MonoBehaviour> components)
		{
			var (resolved, notResolved, notInjectable) = GetResolutionStatistics(resolver, components);

			return new StringBuilder()
				.Append(notResolved > 0 || notInjectable > 0 ? "<color=red>" : "<color=green>")
				.AppendFormat("<b>{0} resolved, {1} failed to resolve, {2} not injectable</b>", resolved.ToString(),
					notResolved.ToString(), notInjectable.ToString())
				.Append("</color>")
				.ToString();
		}

		private static (int resolved, int notResolved, int notInjectable) GetResolutionStatistics(Resolver resolver,
			IEnumerable<MonoBehaviour> components)
		{
			var types = components
				.Select(component => (component, GetAllDependenciesOf(component)))
				.ToArray();

			var resolved = Sum(types, (c, t) => CanBeResolved(resolver, t, c));
			var notResolved = Sum(types, (c, t) => IsInjectable(c) && !CanBeResolved(resolver, t, c));
			var notInjectable = types.Count(t => !IsInjectable(t.component));
			return (resolved, notResolved, notInjectable);
		}

		private static int Sum(IEnumerable<(MonoBehaviour component, IEnumerable<Type> types)> dependencies,
			Func<MonoBehaviour, Type, bool> predicate)
		{
			return dependencies.Sum(d => d.types.Count(t => predicate(d.component, t)));
		}

		private static void DrawComponent(Resolver resolver, MonoBehaviour component, int depth)
		{
			var color = GUI.color;
			GUI.color = Color.white;

			var dependencies = GetAllDependenciesOf(component).ToArray();

			GUILayout.BeginHorizontal();

			if (dependencies.Length > 0)
			{
				const int indentPerLevel = 20;
				GUILayout.Space(depth * indentPerLevel);
				GUILayout.Label("-", GUILayout.Width(10));
				DrawReadonlyField(component);
				GUILayout.FlexibleSpace();
			}

			if (IsInjectable(component))
			{
				if (dependencies.Length > 0)
					DrawDependencies(resolver, component);
			}
			else
			{
				DrawBox("Not injectable", false);
			}

			GUILayout.EndHorizontal();

			GUI.color = color;
		}

		private static void DrawReadonlyField(MonoBehaviour component)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField(component, typeof(Component), true, GUILayout.Width(200));
			EditorGUI.EndDisabledGroup();
		}

		private static void DrawDependencies(Resolver resolver, MonoBehaviour component)
		{
			var dependencies = GetAllDependenciesOf(component);

			foreach (var dependency in dependencies)
			{
				var dependencyText = dependency.Name;

				var canBeResolved = CanBeResolved(resolver, dependency, component);
				DrawBox(dependencyText, canBeResolved);
			}
		}

		private static void DrawBox(string text, bool success)
		{
			GUI.color = success ? Color.green : Color.red;
			GUILayout.Box(text);
		}

		private static bool CanBeResolved(Resolver resolver, Type dependency, MonoBehaviour component) =>
			resolver.CabBeResolvedSafe(component, dependency);
	}
}