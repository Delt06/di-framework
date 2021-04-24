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
            var functionDeclarations = new StringBuilder();

            foreach (var type in _bakedTypes)
            {
                var bakeCallText = GetBakeCallText(type, index);

                string functionDeclaration;
                var success = PocoInjection.IsPoco(type)
                    ? TryGetInstantiationFunctionDeclaration(type, index, out functionDeclaration)
                    : TryGetInjectionFunctionDeclaration(type, index, out functionDeclaration);
                if (!success) continue;

                bakeCalls.Append(bakeCallText);
                functionDeclarations.Append(functionDeclaration);
                functionDeclarations.AppendLine();
                index++;
            }

            FixLineEndings(bakeCalls);
            FixLineEndings(functionDeclarations);

            var dataText = string.Format(ClassTemplate, Namespace, _className, bakeCalls,
                functionDeclarations
            );
            writer.Write(dataText);
        }

        private static string GetBakeCallText(Type type, int functionIndex)
        {
            var methodName = $"{nameof(BakedInjection)}.{nameof(BakedInjection.Bake)}";
            var fullTypeName = GetFullyQualifiedName(type);
            var typeName = $"typeof({fullTypeName})";
            var functionName = PocoInjection.IsPoco(type)
                ? GetInstantiationFunctionName(functionIndex)
                : GetInjectionFunctionName(functionIndex);

            return new StringBuilder()
                .Append(TripleIndent)
                .Append(methodName)
                .Append("(")
                .Append(typeName)
                .Append(", ")
                .Append(functionName)
                .AppendLine(");")
                .ToString();
        }

        private static string GetInjectionFunctionName(int functionIndex) =>
            $"InjectionFunction_{functionIndex}";

        private static string GetInstantiationFunctionName(int functionIndex) =>
            $"InstantiationFunction_{functionIndex}";

        private static bool TryGetInjectionFunctionDeclaration(Type type, int injectionFunctionIndex, out string result)
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
                    if (parameter.ParameterType.IsConstructedGenericType)
                    {
                        result = default;
                        return false;
                    }

                    AppendResolveExpression(parameter.ParameterType, stringBuilder);
                }

                stringBuilder.AppendLine(");");
            }


            stringBuilder.Append(DoubleIndent)
                .AppendLine("}");

            result = stringBuilder.ToString();
            return true;
        }

        private static bool TryGetInstantiationFunctionDeclaration(Type type, int functionIndex, out string result)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(DoubleIndent)
                .Append("private static object ")
                .Append(GetInstantiationFunctionName(functionIndex))
                .Append($"({nameof(PocoResolutionFunction)} resolve)")
                .AppendLine();

            stringBuilder.Append(DoubleIndent)
                .AppendLine("{");

            if (PocoInjection.TryGetInjectableConstructorParameters(type, out var parameters))
            {
                stringBuilder.Append(TripleIndent)
                    .Append("return new ")
                    .Append(GetFullyQualifiedName(type))
                    .Append("(");

                for (var parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
                {
                    if (parameterIndex > 0)
                        stringBuilder.Append(", ");

                    var parameter = parameters[parameterIndex];
                    if (parameter.ParameterType.IsConstructedGenericType)
                    {
                        result = default;
                        return false;
                    }

                    AppendPocoResolveExpression(parameter.ParameterType, stringBuilder);
                }

                stringBuilder.AppendLine(");");
            }

            stringBuilder.Append(DoubleIndent)
                .AppendLine("}");

            result = stringBuilder.ToString();
            return true;
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

        private static void AppendPocoResolveExpression(Type dependencyType, StringBuilder stringBuilder)
        {
            var typeName = GetFullyQualifiedName(dependencyType);
            stringBuilder.Append("(")
                .Append(typeName)
                .Append(") resolve(typeof(")
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