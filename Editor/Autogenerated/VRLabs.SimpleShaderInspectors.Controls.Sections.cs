namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    public static partial class Chainables
    {
        public static ActivatableSection AddActivatableSection(this IControlContainer container, System.String activatePropertyName, System.String showPropertyName, System.Single enableValue = 1, System.Single disableValue = 0, System.Single hideValue = 0, System.Single showValue = 1)
        {
            var control = new ActivatableSection(activatePropertyName, showPropertyName, enableValue, disableValue, hideValue, showValue);
            container.Controls.Add(control);
            return control;
        }
        public static ActivatableSection AddActivatableSection(this IControlContainer container, System.String activatePropertyName, System.Single enableValue = 1, System.Single disableValue = 0)
        {
            var control = new ActivatableSection(activatePropertyName, enableValue, disableValue);
            container.Controls.Add(control);
            return control;
        }

        public static OrderedSection AddOrderedSection(this OrderedSectionGroup container, System.String activatePropertyName, System.String showPropertyName, System.Single hideValue = 0, System.Single showValue = 1)
        {
            var control = new OrderedSection(activatePropertyName, showPropertyName, hideValue, showValue);
            container.Controls.Add(control);
            return control;
        }
        public static OrderedSection AddOrderedSection(this OrderedSectionGroup container, System.String activatePropertyName)
        {
            var control = new OrderedSection(activatePropertyName);
            container.Controls.Add(control);
            return control;
        }
        public static T SetUpIcon<T>(this T control, UnityEngine.GUIStyle property) where T : OrderedSection
        {
            control.UpIcon = property;
            return control;
        }
        public static T SetDownIcon<T>(this T control, UnityEngine.GUIStyle property) where T : OrderedSection
        {
            control.DownIcon = property;
            return control;
        }
        public static T SetDeleteIcon<T>(this T control, UnityEngine.GUIStyle property) where T : OrderedSection
        {
            control.DeleteIcon = property;
            return control;
        }
        public static T SetUpColor<T>(this T control, UnityEngine.Color property) where T : OrderedSection
        {
            control.UpColor = property;
            return control;
        }
        public static T SetDownColor<T>(this T control, UnityEngine.Color property) where T : OrderedSection
        {
            control.DownColor = property;
            return control;
        }
        public static T SetDeleteColor<T>(this T control, UnityEngine.Color property) where T : OrderedSection
        {
            control.DeleteColor = property;
            return control;
        }

        public static OrderedSectionGroup AddOrderedSectionGroup(this IControlContainer container, System.String alias)
        {
            var control = new OrderedSectionGroup(alias);
            container.Controls.Add(control);
            return control;
        }

        public static Section AddSection(this IControlContainer container, System.String propertyName, System.Single hideValue = 0, System.Single showValue = 1)
        {
            var control = new Section(propertyName, hideValue, showValue);
            container.Controls.Add(control);
            return control;
        }
        public static Section AddSection(this IControlContainer container)
        {
            var control = new Section();
            container.Controls.Add(control);
            return control;
        }
        public static T SetLabelStyle<T>(this T control, UnityEngine.GUIStyle property) where T : Section
        {
            control.LabelStyle = property;
            return control;
        }
        public static T SetBackgroundStyle<T>(this T control, UnityEngine.GUIStyle property) where T : Section
        {
            control.BackgroundStyle = property;
            return control;
        }
        public static T SetAreControlsInside<T>(this T control, System.Boolean property) where T : Section
        {
            control.AreControlsInside = property;
            return control;
        }
        public static T SetIsPropertyAnimatable<T>(this T control, System.Boolean property) where T : Section
        {
            control.IsPropertyAnimatable = property;
            return control;
        }
        public static T SetShowFoldoutArrow<T>(this T control, System.Boolean property) where T : Section
        {
            control.ShowFoldoutArrow = property;
            return control;
        }
        public static T SetBackgroundColor<T>(this T control, UnityEngine.Color property) where T : Section
        {
            control.BackgroundColor = property;
            return control;
        }

    }
}
