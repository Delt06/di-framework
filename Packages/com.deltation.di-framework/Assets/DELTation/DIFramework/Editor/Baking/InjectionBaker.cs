using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DELTation.DIFramework.Baking;
using DELTation.DIFramework.Resolution;
using JetBrains.Annotations;
using UnityEngine;
using static DELTation.DIFramework.Editor.Baking.InjectionBakingData;
using static DELTation.DIFramework.Editor.Baking.InjectionBakingTypesUtils;

namespace DELTation.DIFramework.Editor.Baking
{
    internal sealed class InjectionBaker
    {
        public InjectionBaker([NotNull] string className, [NotNull] IEnumerable<Type> bakedTypes)
        {
            if (bakedTypes == null) throw new ArgumentNullException(nameof(bakedTypes));
            _className = className ?? throw new ArgumentNullException(nameof(className));
            _bakedTypes = bakedTypes.Distinct().ToArray();
        }

        public int BakedTypesCount => _bakedTypes.Length;

        public void BakeData([NotNull] TextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            var index = 0;
            var bakeCalls = new StringBuilder();
            var injectionFunctionDeclarations = new StringBuilder();

            foreach (var type in _bakedTypes)
            {
                bakeCalls.Append(GetBakeCallText(type, index));
                injectionFunctionDeclarations.Append(GetInjectionFunctionDeclaration(type, index));
                injectionFunctionDeclarations.AppendLine();
                index++;
            }

            FixLineEndings(bakeCalls);
            FixLineEndings(injectionFunctionDeclarations);

            var dataText = string.Format(ClassTemplate, Namespace, _className, bakeCalls,
                injectionFunctionDeclarations
            );
            writer.Write(dataText);
        }

        private static string GetBakeCallText(Type type, int injectionFunctionIndex)
        {
            var methodName = $"{nameof(BakedInjection)}.{nameof(BakedInjection.Bake)}";
            var fullTypeName = GetFullyQualifiedName(type);
            var typeName = $"typeof({fullTypeName})";
            var injectionFunctionName = GetInjectionFunctionName(injectionFunctionIndex);

            return new StringBuilder()
                .Append(TripleIndent)
                .Append(methodName)
                .Append("(")
                .Append(typeName)
                .Append(", ")
                .Append(injectionFunctionName)
                .AppendLine(");")
                .ToString();
        }

        private static string GetInjectionFunctionName(int injectionFunctionIndex) =>
            $"InjectionFunction_{injectionFunctionIndex}";

        private static string GetInjectionFunctionDeclaration(Type type, int injectionFunctionIndex)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(DoubleIndent)
                .Append("private static void ")
                .Append(GetInjectionFunctionName(injectionFunctionIndex))
                .Append($"({nameof(MonoBehaviour)} component, {nameof(ResolutionFunction)} resolve)")
                .AppendLine();

            stringBuilder.Append(DoubleIndent)
                .AppendLine("{");

            stringBuilder.Append(TripleIndent)
                .AppendLine($"var obj = ({GetFullyQualifiedName(type)}) component;");

            foreach (var constructMethod in Injection.GetConstructMethods(type))
            {
                if (!Injection.TryGetInjectableParameters(constructMethod, out var parameters)) continue;

                stringBuilder.Append(TripleIndent)
                    .Append("obj.")
                    .Append(Injection.Constructor)
                    .Append("(");

                for (var parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    if (parameterIndex > 0)
                        stringBuilder.Append(", ");

                    var parameter = parameters[parameterIndex];
                    AppendResolveExpression(parameter.ParameterType, stringBuilder);
                }

                stringBuilder.AppendLine(");");
            }


            stringBuilder.Append(DoubleIndent)
                .AppendLine("}");

            return stringBuilder.ToString();
        }

        private static void AppendResolveExpression(Type dependencyType, StringBuilder stringBuilder)
        {
            var typeName = GetFullyQualifiedName(dependencyType);
            stringBuilder.Append("(")
                .Append(typeName)
                .Append(") resolve(component, typeof(")
                .Append(typeName)
                .Append("))");
        }

        private static void FixLineEndings(StringBuilder stringBuilder)
        {
            const string crlf = "\r\n";
            const string lf = "\n";
            stringBuilder.Replace(crlf, lf);
        }

        private readonly string _className;
        private readonly Type[] _bakedTypes;
    }
}