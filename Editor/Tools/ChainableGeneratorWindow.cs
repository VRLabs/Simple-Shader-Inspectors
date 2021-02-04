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

        private string _destinationPath = null;
        private string _namespace = null;

        void OnGUI()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Select destination folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select destination folder to use", "Assets", "Editor");
                if (path.Length == 0)
                {
                    _destinationPath = null;
                }
                else
                {
                    _destinationPath = path;
                }
            }
            GUILayout.Label("Selected folder: " + _destinationPath, Styles.MultilineLabel);
            if (_destinationPath == null)
            {
                EditorGUILayout.HelpBox("You need to select a folder.", MessageType.Warning);
            }
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            _namespace = EditorGUILayout.TextField("Namespace", _namespace);
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_destinationPath) || string.IsNullOrWhiteSpace(_namespace));
            if (GUILayout.Button("Generate chainables"))
            {
                GenerateChainables(_destinationPath, _namespace);
            }
            EditorGUI.EndDisabledGroup();
        }
        private void GenerateChainables(string destinationPath, string nmsc)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !string.IsNullOrEmpty(x.Namespace) && x.Namespace.Contains(nmsc) && x.IsSubclassOf(typeof(SimpleControl)))
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
                    {
                        BuildConstructorChainable(content, type, constructor, indent);
                    }

                    var chainableProperties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public).Where(
                    y => y.CustomAttributes.Any(
                        z => z.AttributeType == typeof(ChainableAttribute)));

                    foreach (var prop in chainableProperties)
                    {
                        BuildPropertyChainable(content, type, prop, indent);
                    }
                    content.AppendLine();
                }

                indent--;
                content.Append(Indentation(indent))
                    .AppendLine("}");
                indent--;
                content.Append(Indentation(indent))
                    .AppendLine("}");

                File.WriteAllText($"{destinationPath}\\{group.Key}.Chainables.cs", content.ToString().Replace("\r\n", "\n"), System.Text.Encoding.UTF8);
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
        private static void BuildConstructorChainable(StringBuilder content, Type type, System.Reflection.ConstructorInfo constructor, int indentLevel)
        {
            // Check if the constructor has the limitAccessScope attribute to limit the objects that can access the chainable
            LimitAccessScopeAttribute limitScopeAttribute =
                (LimitAccessScopeAttribute)Array.Find(constructor.GetCustomAttributes(false), x => x.GetType() == typeof(LimitAccessScopeAttribute));

            string typeName;
            if (limitScopeAttribute != null)
                typeName = limitScopeAttribute.BaseType.Name;
            else
                typeName = "IControlContainer";


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
            bool needcomma = false;
            foreach (var parameter in constructor.GetParameters())
            {
                if (needcomma)
                    content.Append(", ");
                else
                    needcomma = true;
                content.Append(parameter.Name);
            }
            content.AppendLine(");")
                .Append(Indentation(indentLevel))
                .AppendLine("container.Controls.Add(control);")
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

            foreach (Type arg in t.GetGenericArguments())
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
            {
                builder.Append("    ");
            }
            return builder.ToString();
        }
    }
}