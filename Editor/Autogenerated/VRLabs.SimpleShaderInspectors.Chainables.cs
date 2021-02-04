namespace VRLabs.SimpleShaderInspectors
{
    public static partial class Chainables
    {
        public static PropertyControl AddPropertyControl(this IControlContainer container, System.String propertyName)
        {
            var control = new PropertyControl(propertyName);
            container.Controls.Add(control);
            return control;
        }

    }
}
