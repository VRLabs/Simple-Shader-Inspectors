namespace VRLabs.SimpleShaderInspectors
{
    public static partial class Chainables
    {
        public static PropertyControl AddPropertyControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, string appendAfterAlias = "")
        {
            var control = new PropertyControl(propertyName);
            container.AddControl(control, appendAfterAlias);
            return control;
        }

    }
}
