using DELTation.DIFramework;
using DELTation.DIFramework.Reporting;
using UnityEditor;
using UnityEngine;

namespace DELTation.DIFramework.Editor
{
	public static class GlobalResolverReportGenerator
	{
		[MenuItem("DI/Generate Report")]
		public static void GenerateReport()
		{
			var resolvers = Object.FindObjectsOfType<Resolver>();
			int totalResolved = 0, totalNotResolved = 0, totalNotInjectable = 0;
			string message;
			LogType logType;

			LogLine();

			foreach (var resolver in resolvers)
			{
				var report = new ResolverReport(resolver);
				report.Generate();
				message = FormatMessage(resolver.gameObject.name, report.Resolved, report.NotResolved,
					report.NotInjectable);
				logType = report.NotResolved > 0 || report.NotInjectable > 0 ? LogType.Error : LogType.Log;
				Log(logType, message, resolver.gameObject);

				totalResolved += report.Resolved;
				totalNotResolved += report.NotResolved;
				totalNotInjectable += report.NotInjectable;
			}

			message = FormatMessage("Overall", totalResolved, totalNotResolved, totalNotInjectable);
			logType = totalNotResolved > 0 || totalNotInjectable > 0 ? LogType.Error : LogType.Log;
			LogLine();
			Log(logType, message);
		}

		private static void LogLine()
		{
			Debug.Log(new string('-', 50));
		}

		private static void Log(LogType logType, string message, GameObject context = null)
		{
			Debug.unityLogger.logHandler.LogFormat(logType, context, message);
		}

		private static string FormatMessage(string prefix, int resolved, int notResolved, int notInjectable) =>
			$"{prefix}: {resolved} resolved, {notResolved} not resolved, {notInjectable} not injectable";
	}
}