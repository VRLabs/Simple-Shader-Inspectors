namespace VRLabs.SimpleShaderInspectors.Controls
{
    public static partial class Chainables
    {
        public static ColorControl AddColorControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.Boolean showAlphaValue = true)
        {
            var control = new ColorControl(propertyName, showAlphaValue);
            container.AddControl(control);
            return control;
        }
        public static T SetShowAlphaValue<T>(this T control, System.Boolean property) where T : ColorControl
        {
            control.ShowAlphaValue = property;
            return control;
        }

        public static ConditionalControlContainer AddConditionalControlContainer(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String conditionalProperty, System.Single enableValue)
        {
            var control = new ConditionalControlContainer(conditionalProperty, enableValue);
            container.AddControl(control);
            return control;
        }
        public static T SetIndent<T>(this T control, System.Boolean property) where T : ConditionalControlContainer
        {
            control.Indent = property;
            return control;
        }

        public static ControlContainer AddControlContainer(this VRLabs.SimpleShaderInspectors.IControlContainer container)
        {
            var control = new ControlContainer();
            container.AddControl(control);
            return control;
        }

        public static EnumControl<TEnum> AddEnumControl<TEnum>(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName) where TEnum : System.Enum
        {
            var control = new EnumControl<TEnum>(propertyName);
            container.AddControl(control);
            return control;
        }

        public static GradientTextureControl AddGradientTextureControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.String colorPropertyName = null)
        {
            var control = new GradientTextureControl(propertyName, colorPropertyName);
            container.AddControl(control);
            return control;
        }
        public static T SetGradientButtonStyle<T>(this T control, UnityEngine.GUIStyle property) where T : GradientTextureControl
        {
            control.GradientButtonStyle = property;
            return control;
        }
        public static T SetGradientSaveButtonStyle<T>(this T control, UnityEngine.GUIStyle property) where T : GradientTextureControl
        {
            control.GradientSaveButtonStyle = property;
            return control;
        }
        public static T SetGradientEditorStyle<T>(this T control, UnityEngine.GUIStyle property) where T : GradientTextureControl
        {
            control.GradientEditorStyle = property;
            return control;
        }
        public static T SetGradientButtonColor<T>(this T control, UnityEngine.Color property) where T : GradientTextureControl
        {
            control.GradientButtonColor = property;
            return control;
        }
        public static T SetGradientSaveButtonColor<T>(this T control, UnityEngine.Color property) where T : GradientTextureControl
        {
            control.GradientSaveButtonColor = property;
            return control;
        }
        public static T SetGradientEditorColor<T>(this T control, UnityEngine.Color property) where T : GradientTextureControl
        {
            control.GradientEditorColor = property;
            return control;
        }

        public static KeywordToggleControl AddKeywordToggleControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String keyword)
        {
            var control = new KeywordToggleControl(keyword);
            container.AddControl(control);
            return control;
        }

        public static KeywordToggleListControl AddKeywordToggleListControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String keyword)
        {
            var control = new KeywordToggleListControl(keyword);
            container.AddControl(control);
            return control;
        }

        public static LabelControl AddLabelControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String alias)
        {
            var control = new LabelControl(alias);
            container.AddControl(control);
            return control;
        }
        public static T SetLabelStyle<T>(this T control, UnityEngine.GUIStyle property) where T : LabelControl
        {
            control.LabelStyle = property;
            return control;
        }

        public static LightmapEmissionControl AddLightmapEmissionControl(this VRLabs.SimpleShaderInspectors.IControlContainer container)
        {
            var control = new LightmapEmissionControl();
            container.AddControl(control);
            return control;
        }

        public static SpaceControl AddSpaceControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.Int32 space = 0)
        {
            var control = new SpaceControl(space);
            container.AddControl(control);
            return control;
        }

        public static TextureControl AddTextureControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.String extraPropertyName1 = null, System.String extraPropertyName2 = null)
        {
            var control = new TextureControl(propertyName, extraPropertyName1, extraPropertyName2);
            container.AddControl(control);
            return control;
        }
        public static T SetShowUvOptions<T>(this T control, System.Boolean property) where T : TextureControl
        {
            control.ShowUvOptions = property;
            return control;
        }
        public static T SetHasHDRColor<T>(this T control, System.Boolean property) where T : TextureControl
        {
            control.HasHDRColor = property;
            return control;
        }
        public static T SetUVButtonStyle<T>(this T control, UnityEngine.GUIStyle property) where T : TextureControl
        {
            control.UVButtonStyle = property;
            return control;
        }
        public static T SetUVAreaStyle<T>(this T control, UnityEngine.GUIStyle property) where T : TextureControl
        {
            control.UVAreaStyle = property;
            return control;
        }
        public static T SetUVButtonColor<T>(this T control, UnityEngine.Color property) where T : TextureControl
        {
            control.UVButtonColor = property;
            return control;
        }
        public static T SetUVAreaColor<T>(this T control, UnityEngine.Color property) where T : TextureControl
        {
            control.UVAreaColor = property;
            return control;
        }

        public static TextureGeneratorControl AddTextureGeneratorControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.String extraPropertyName1 = null, System.String extraPropertyName2 = null)
        {
            var control = new TextureGeneratorControl(propertyName, extraPropertyName1, extraPropertyName2);
            container.AddControl(control);
            return control;
        }
        public static TextureGeneratorControl AddTextureGeneratorControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, UnityEngine.ComputeShader compute, System.String computeOptionsJson, System.String propertyName, System.String extraPropertyName1 = null, System.String extraPropertyName2 = null)
        {
            var control = new TextureGeneratorControl(compute, computeOptionsJson, propertyName, extraPropertyName1, extraPropertyName2);
            container.AddControl(control);
            return control;
        }
        public static T SetGeneratorButtonStyle<T>(this T control, UnityEngine.GUIStyle property) where T : TextureGeneratorControl
        {
            control.GeneratorButtonStyle = property;
            return control;
        }
        public static T SetGeneratorSaveButtonStyle<T>(this T control, UnityEngine.GUIStyle property) where T : TextureGeneratorControl
        {
            control.GeneratorSaveButtonStyle = property;
            return control;
        }
        public static T SetGeneratorStyle<T>(this T control, UnityEngine.GUIStyle property) where T : TextureGeneratorControl
        {
            control.GeneratorStyle = property;
            return control;
        }
        public static T SetGeneratorInputStyle<T>(this T control, UnityEngine.GUIStyle property) where T : TextureGeneratorControl
        {
            control.GeneratorInputStyle = property;
            return control;
        }
        public static T SetGeneratorButtonColor<T>(this T control, UnityEngine.Color property) where T : TextureGeneratorControl
        {
            control.GeneratorButtonColor = property;
            return control;
        }
        public static T SetGeneratorSaveButtonColor<T>(this T control, UnityEngine.Color property) where T : TextureGeneratorControl
        {
            control.GeneratorSaveButtonColor = property;
            return control;
        }
        public static T SetGeneratorColor<T>(this T control, UnityEngine.Color property) where T : TextureGeneratorControl
        {
            control.GeneratorColor = property;
            return control;
        }
        public static T SetGeneratorInputColor<T>(this T control, UnityEngine.Color property) where T : TextureGeneratorControl
        {
            control.GeneratorInputColor = property;
            return control;
        }

        public static TilingAndOffsetControl AddTilingAndOffsetControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName)
        {
            var control = new TilingAndOffsetControl(propertyName);
            container.AddControl(control);
            return control;
        }

        public static ToggleControl AddToggleControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.Single falseValue = 0, System.Single trueValue = 1)
        {
            var control = new ToggleControl(propertyName, falseValue, trueValue);
            container.AddControl(control);
            return control;
        }

        public static ToggleListControl AddToggleListControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.Single falseValue = 0, System.Single trueValue = 1)
        {
            var control = new ToggleListControl(propertyName, falseValue, trueValue);
            container.AddControl(control);
            return control;
        }

        public static VectorControl AddVectorControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.Boolean isXVisible = true, System.Boolean isYVisible = true, System.Boolean isZVisible = true, System.Boolean isWVisible = true)
        {
            var control = new VectorControl(propertyName, isXVisible, isYVisible, isZVisible, isWVisible);
            container.AddControl(control);
            return control;
        }

        public static VertexStreamsControl AddVertexStreamsControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String alias)
        {
            var control = new VertexStreamsControl(alias);
            container.AddControl(control);
            return control;
        }

    }
}
