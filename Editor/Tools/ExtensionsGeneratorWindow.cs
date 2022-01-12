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
    public class ExtensionsGeneratorWindow : EditorWindow
    {

        [MenuItem("VRLabs/Simple Shader Inspectors/Generate Chainable Methods")]
        private static ExtensionsGeneratorWindow CreateWindow()
        {
            ExtensionsGeneratorWindow window = EditorWindow.GetWindow<ExtensionsGeneratorWindow>();
            window.titleContent = new GUIContent("Extensions");
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
            
            if (GUILayout.Button("Generate extension methods"))
            {
                string path = EditorUtility.OpenFolderPanel("Select destination folder to use", "Assets", "Editor");
                string destinationPath = path.Length == 0 ? null : path;
                if (!string.IsNullOrWhiteSpace(destinationPath))
                {
                    if (string.IsNullOrWhiteSpace(_controlsNamespace)) _controlsNamespace = _namespaces[_selectedNamespace];
                    GenerateExtensions(destinationPath, _namespaces[_selectedNamespace], _controlsNamespace);
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

        private static void GenerateExtensions(string destinationPath, string nmsc, string controlsNmsc)
        {
            var ssiTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => !string.IsNullOrEmpty(x.Namespace) && x.Namespace.Contains(nmsc))
                .ToList();
            
            var simpleControlType = ssiTypes.FirstOrDefault(x => x.IsClass && x.Name.Equals("SimpleControl"));
            if (simpleControlType == null) return;
            
            var fluentSetAttributeType = ssiTypes.FirstOrDefault(x => x.IsClass && x.Name.Equals("FluentSetAttribute"));
            if (fluentSetAttributeType == null) return;
            
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
                        BuildConstructorExtensions(content, type, limitScopeAttributeType, constructor, indent);

                    var fluentSetProperties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                        .Where(y => y.CustomAttributes.Any(z => z.AttributeType == fluentSetAttributeType));

                    foreach (var prop in fluentSetProperties)
                        BuildPropertyChainable(content, type, prop, indent);
                    
                    content.AppendLine();
                }

                indent--;
                content.Append(Indentation(indent))
                    .AppendLine("}");
                indent--;
                content.Append(Indentation(indent))
                    .AppendLine("}");

                File.WriteAllText($"{destinationPath}\\{group.Key}.Extensions.cs", content.ToString().Replace("\r\n", "\n"), Encoding.UTF8);
            }

            AssetDatabase.Refresh();
        }

        private static void BuildPropertyChainable(StringBuilder content, Type type, System.Reflection.PropertyInfo prop, int indentLevel)
        {
            // Chainable Header
            content.Append(Indentation(indentLevel))
                .AppendLine($"public static T With{prop.Name}<T>(this T control, {GenerateGenericListString(prop.PropertyType, true)} property) where T : {type.Name}")
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
        private static void BuildConstructorExtensions(StringBuilder content, Type type, Type limitScopeAttributeType, ConstructorInfo constructor, int indentLevel)
        {
            // Check if the constructor has the limitAccessScope attribute to limit the objects that can access the chainable
            var limitScopeAttribute = Array.Find(constructor.GetCustomAttributes(false), x => x.GetType() == limitScopeAttributeType);
            var a = limitScopeAttributeType.GetProperty("BaseType");
            Type baseType = limitScopeAttribute== null ? null : (Type) a?.GetValue(limitScopeAttribute);

            string typeName = baseType != null ? baseType.FullName : limitScopeAttributeType.Namespace +".IControlContainer";
            
            // Chainable constructor header
            content.Append(Indentation(indentLevel))
                .Append($"public static {GenerateGenericListString(type, true)} Add{GenerateGenericListString(type)}(this {typeName} container");

            foreach (var parameter in constructor.GetParameters())
            {
                content.Append($", {GenerateGenericListString(parameter.ParameterType, true)} {parameter.Name}");
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
            content.Append(", string appendAfterAlias = \"\")").AppendLine(GenerateGenericConstraintsString(type))
                .Append(Indentation(indentLevel))
                .AppendLine("{");

            // Chainable constructor body
            indentLevel++;
            content.Append(Indentation(indentLevel))
                .Append("var control = new ").Append(GenerateGenericListString(type, true)).Append('(');
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
                .AppendLine("container.AddControl(control, appendAfterAlias);")
                .Append(Indentation(indentLevel))
                .AppendLine("return control;");
            indentLevel--;

            content.Append(Indentation(indentLevel))
                .AppendLine("}");
        }

        //shamelessly taken from: https://stackoverflow.com/questions/17480990/get-name-of-generic-class-without-tilde
        private static string GenerateGenericListString(Type t, bool useFullName = false)
        {
            string name = useFullName ? t.FullName : t.Name;
            if (!t.IsGenericType)
                return name;

            var sb = new StringBuilder();
            sb.Append(name, 0, name.IndexOf('`'));
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