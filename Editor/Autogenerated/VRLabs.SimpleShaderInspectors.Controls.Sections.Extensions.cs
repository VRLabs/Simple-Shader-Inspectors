namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    public static partial class Chainables
    {
        public static VRLabs.SimpleShaderInspectors.Controls.Sections.ActivatableSection AddActivatableSection(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String activatePropertyName, System.String showPropertyName, System.Single enableValue = 1, System.Single disableValue = 0, System.Single hideValue = 0, System.Single showValue = 1, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.ActivatableSection(activatePropertyName, showPropertyName, enableValue, disableValue, hideValue, showValue);
            container.AddControl(control, appendAfterAlias);
            return control;
        }
        public static VRLabs.SimpleShaderInspectors.Controls.Sections.ActivatableSection AddActivatableSection(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String activatePropertyName, System.Single enableValue = 1, System.Single disableValue = 0, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.ActivatableSection(activatePropertyName, enableValue, disableValue);
            container.AddControl(control, appendAfterAlias);
            return control;
        }

        public static VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSection AddOrderedSection(this VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSectionGroup container, System.String activatePropertyName, System.String showPropertyName, System.Single enableValue = 1, System.Single disableValue = 0, System.Single showValue = 1, System.Single hideValue = 0, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSection(activatePropertyName, showPropertyName, enableValue, disableValue, showValue, hideValue);
            container.AddControl(control, appendAfterAlias);
            return control;
        }
        public static VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSection AddOrderedSection(this VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSectionGroup container, System.String activatePropertyName, System.Single enableValue = 1, System.Single disableValue = 0, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSection(activatePropertyName, enableValue, disableValue);
            container.AddControl(control, appendAfterAlias);
            return control;
        }
        public static T WithUpIcon<T>(this T control, UnityEngine.GUIStyle property) where T : OrderedSection
        {
            control.UpIcon = property;
            return control;
        }
        public static T WithDownIcon<T>(this T control, UnityEngine.GUIStyle property) where T : OrderedSection
        {
            control.DownIcon = property;
            return control;
        }
        public static T WithDeleteIcon<T>(this T control, UnityEngine.GUIStyle property) where T : OrderedSection
        {
            control.DeleteIcon = property;
            return control;
        }
        public static T WithUpColor<T>(this T control, UnityEngine.Color property) where T : OrderedSection
        {
            control.UpColor = property;
            return control;
        }
        public static T WithDownColor<T>(this T control, UnityEngine.Color property) where T : OrderedSection
        {
            control.DownColor = property;
            return control;
        }
        public static T WithDeleteColor<T>(this T control, UnityEngine.Color property) where T : OrderedSection
        {
            control.DeleteColor = property;
            return control;
        }

        public static VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSectionGroup AddOrderedSectionGroup(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String alias, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.OrderedSectionGroup(alias);
            container.AddControl(control, appendAfterAlias);
            return control;
        }

        public static VRLabs.SimpleShaderInspectors.Controls.Sections.Section AddSection(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, System.Single hideValue = 0, System.Single showValue = 1, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.Section(propertyName, hideValue, showValue);
            container.AddControl(control, appendAfterAlias);
            return control;
        }
        public static VRLabs.SimpleShaderInspectors.Controls.Sections.Section AddSection(this VRLabs.SimpleShaderInspectors.IControlContainer container, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.Controls.Sections.Section();
            container.AddControl(control, appendAfterAlias);
            return control;
        }
        public static T WithLabelStyle<T>(this T control, UnityEngine.GUIStyle property) where T : Section
        {
            control.LabelStyle = property;
            return control;
        }
        public static T WithBackgroundStyle<T>(this T control, UnityEngine.GUIStyle property) where T : Section
        {
            control.BackgroundStyle = property;
            return control;
        }
        public static T WithAreControlsInHeader<T>(this T control, System.Boolean property) where T : Section
        {
            control.AreControlsInHeader = property;
            return control;
        }
        public static T WithIsPropertyAnimatable<T>(this T control, System.Boolean property) where T : Section
        {
            control.IsPropertyAnimatable = property;
            return control;
        }
        public static T WithShowFoldoutArrow<T>(this T control, System.Boolean property) where T : Section
        {
            control.ShowFoldoutArrow = property;
            return control;
        }
        public static T WithBackgroundColor<T>(this T control, UnityEngine.Color property) where T : Section
        {
            control.BackgroundColor = property;
            return control;
        }

    }
}
