namespace VRLabs.SimpleShaderInspectors
{
    public static partial class Chainables
    {
        public static VRLabs.SimpleShaderInspectors.PropertyControl AddPropertyControl(this VRLabs.SimpleShaderInspectors.IControlContainer container, System.String propertyName, string appendAfterAlias = "")
        {
            var control = new VRLabs.SimpleShaderInspectors.PropertyControl(propertyName);
            container.AddControl(control, appendAfterAlias);
            return control;
        }

    }
}
