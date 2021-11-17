using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;

namespace VRLabs.SimpleShaderInspectors.Tools
{
    /// <summary>
    /// Editor window that embeds the Simple Shader Inspectors library into any custom editor folder with customized namespace.
    /// </summary>
    public class ChainableGeneratorWindow : EditorWindow
    {

        [MenuItem("VRLabs/Simple Shader Inspectors/Generate Chainable Methods")]
        private static ChainableGeneratorWindow CreateWindow()
        {
            ChainableGeneratorWindow window = EditorWindow.GetWindow<ChainableGeneratorWindow>();
            window.titleContent = new GUIContent("Chainables");
            window.minSize = new Vector2(400, 280);
            window.maxSize = new Vector2(400, 280);
            return window;
        }

        private int _selectedNamespace;
        private string _controlsNamespace;

        private string[] _namespaces;

        void OnGUI()
        {
            if (_namespaces == null) _namespaces = GetCompatibleNamespaces();
            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            _selectedNamespace = EditorGUILayout.Popup("SSI Namespace", _selectedNamespace, _namespaces);
            _controlsNamespace = EditorGUILayout.TextField("Controls Namespace", _controlsNamespace);
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Generate chainables"))
            {
                string path = EditorUtility.OpenFolderPanel("Select destination folder to use", "Assets", "Editor");
                string destinationPath = path.Length == 0 ? null : path;
                if (!string.IsNullOrWhiteSpace(destinationPath))
                {
                    if (string.IsNullOrWhiteSpace(_controlsNamespace)) _controlsNamespace = _namespaces[_selectedNamespace];
                    GenerateChainables(destinationPath, _namespaces[_selectedNamespace], _controlsNamespace);
                }
            }

            EditorGUI.EndDisabledGroup();
        }

        private static string[] GetCompatibleNamespaces()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.Name.Equals("SimpleControl"))
                .Select(x => x.Namespace)
                .ToArray();
        }

        private static void GenerateChainables(string destinationPath, string nmsc, string controlsNmsc)
        {
            var ssiTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.Contains(nmsc))
                .ToList();
            
            var simpleControlType = ssiTypes.FirstOrDefault(x => x.IsClass && x.Name.Equals("SimpleControl"));
            if (simpleControlType == null) return;
            
            var chainableAttributeType = ssiTypes.FirstOrDefault(x => x.IsClass && x.Name.Equals("ChainableAttribute"));
            if (chainableAttributeType == null) return;
            
            var limitScopeAttributeType = ssiTypes.FirstOrDefault(x => x.IsClass && x.Name.Equals("LimitAccessScopeAttribute"));
            if (limitScopeAttributeType == null) return;

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !string.IsNullOrEmpty(x.Namespace) && x.Namespace.Contains(controlsNmsc) && x.IsSubclassOf(simpleControlType))
                .ToList();

            foreach (var group in types.GroupBy(x => x.Namespace))
            {
                int indent = 0;
                var content = new StringBuilder();

                content
                    .AppendLine($"namespace {group.Key}")
                    .Append(Indentation(indent))
                    .AppendLine("{");
                indent++;

                content.Append(Indentation(indent))
                    .AppendLine("public static partial class Chainables")
                    .Append(Indentation(indent))
                    .AppendLine("{");
                indent++;

                foreach (var type in group)
                {
                    foreach (var constructor in type.GetConstructors())
                        BuildConstructorChainable(content, type, limitScopeAttributeType, constructor, indent);

                    var chainableProperties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                        .Where(y => y.CustomAttributes.Any(z => z.AttributeType == chainableAttributeType));

                    foreach (var prop in chainableProperties)
                        BuildPropertyChainable(content, type, prop, indent);
                    
                    content.AppendLine();
                }

                indent--;
                content.Append(Indentation(indent))
                    .AppendLine("}");
                indent--;
                content.Append(Indentation(indent))
                    .AppendLine("}");

                File.WriteAllText($"{destinationPath}\\{group.Key}.Chainables.cs", content.ToString().Replace("\r\n", "\n"), Encoding.UTF8);
            }

            AssetDatabase.Refresh();
        }

        private static void BuildPropertyChainable(StringBuilder content, Type type, System.Reflection.PropertyInfo prop, int indentLevel)
        {
            // Chainable Header
            content.Append(Indentation(indentLevel))
                .AppendLine($"public static T Set{prop.Name}<T>(this T control, {prop.PropertyType.FullName} property) where T : {type.Name}")
                .Append(Indentation(indentLevel))
                .AppendLine("{");

            // Chainable body
            indentLevel++;
            content.Append(Indentation(indentLevel))
                .AppendLine($"control.{prop.Name} = property;")
                .Append(Indentation(indentLevel)).AppendLine("return control;");
            indentLevel--;

            content.Append(Indentation(indentLevel))
                .AppendLine("}");
        }

        // Append a chainable constructor implementation to the given content.
        private static void BuildConstructorChainable(StringBuilder content, Type type, Type limitScopeAttributeType, ConstructorInfo constructor, int indentLevel)
        {
            // Check if the constructor has the limitAccessScope attribute to limit the objects that can access the chainable
            var limitScopeAttribute = Array.Find(constructor.GetCustomAttributes(false), x => x.GetType() == limitScopeAttributeType);
            var a = limitScopeAttributeType.GetProperty("BaseType");
            Type baseType = limitScopeAttribute== null ? null : (Type) a?.GetValue(limitScopeAttribute);

            string typeName = baseType != null ? baseType.FullName : limitScopeAttributeType.Namespace +".IControlContainer";
            
            // Chainable constructor header
            content.Append(Indentation(indentLevel))
                .Append($"public static {GenerateGenericListString(type)} Add{GenerateGenericListString(type)}(this {typeName} container");

            foreach (var parameter in constructor.GetParameters())
            {
                content.Append($", {parameter.ParameterType.FullName} {parameter.Name}");
                if (parameter.HasDefaultValue)
                {
                    if (parameter.DefaultValue is bool b)
                        content.Append(" = ").Append(b ? "true" : "false");
                    else if (parameter.DefaultValue is null)
                        content.Append(" = null");
                    else
                        content.Append($" = {parameter.DefaultValue}");
                }
            }
            content.Append(")").AppendLine(GenerateGenericConstraintsString(type))
                .Append(Indentation(indentLevel))
                .AppendLine("{");

            // Chainable constructor body
            indentLevel++;
            content.Append(Indentation(indentLevel))
                .Append("var control = new ").Append(GenerateGenericListString(type)).Append('(');
            bool needComma = false;
            foreach (var parameter in constructor.GetParameters())
            {
                if (needComma)
                    content.Append(", ");
                else
                    needComma = true;
                content.Append(parameter.Name);
            }
            content.AppendLine(");")
                .Append(Indentation(indentLevel))
                .AppendLine("container.AddControl(control);")
                .Append(Indentation(indentLevel))
                .AppendLine("return control;");
            indentLevel--;

            content.Append(Indentation(indentLevel))
                .AppendLine("}");
        }

        //shamelessly taken from: https://stackoverflow.com/questions/17480990/get-name-of-generic-class-without-tilde
        private static string GenerateGenericListString(Type t)
        {
            if (!t.IsGenericType)
                return t.Name;

            var sb = new StringBuilder();
            sb.Append(t.Name, 0, t.Name.IndexOf('`'));
            sb.Append('<');
            bool appendComma = false;
            foreach (Type arg in t.GetGenericArguments())
            {
                if (appendComma) sb.Append(',');
                sb.Append(GenerateGenericListString(arg));
                appendComma = true;
            }
            sb.Append('>');
            return sb.ToString();
        }

        private static string GenerateGenericConstraintsString(Type t)
        {
            var sb = new StringBuilder();

            foreach (var arg in t.GetGenericArguments())
            {
                sb.Append($" where {arg.Name} : ");
                bool appendComma = false;
                foreach (var tpc in arg.GetGenericParameterConstraints())
                {
                    sb.Append(tpc.FullName);
                    if (appendComma) sb.Append(',');
                    appendComma = true;
                }
            }
            return sb.ToString();
        }

        private static string Indentation(int level)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < level; i++)
                builder.Append("    ");
            
            return builder.ToString();
        }
    }
}