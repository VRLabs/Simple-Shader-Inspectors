using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    public interface ISimpleShaderInspector : IControlContainer
    {
        Material[] Materials { get; }

        Shader Shader { get; }
    }
}