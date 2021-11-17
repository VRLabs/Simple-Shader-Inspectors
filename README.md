<div align="center">
  <a href="https://github.com/VRLabs/SimpleShaderInspectors">
    <img alt="Simple Shader Inspectors" width="200" heigth="200" src="https://github.com/VRLabs/SimpleShaderInspectors/blob/master/Editor/Resources/SSI/Textures/Logo/SSILogo.png">
  </a>
  <h1>Simple Shader Inspectors</h1>
  <p>
     Unity Editor library with the objective to make medium to complex shader inspectors easier to create and manage.
  </p>

  <a href="https://github.com/VRLabs/SimpleShaderInspectors/releases/latest">
    <img src="https://img.shields.io/github/v/release/VRLabs/SimpleShaderInspectors.svg?style=flat-square">
  </a>
  <a href="https://github.com/VRLabs/SimpleShaderInspectors/releases/latest">
    <img src="https://img.shields.io/badge/Unity-2019.4-green.svg?style=flat-square">
  </a>
  <br />
  <a href="https://github.com/VRLabs/SimpleShaderInspectors/issues">
    <img src="https://img.shields.io/github/issues-raw/VRLabs/SimpleShaderInspectors.svg?style=flat-square">
  </a>
  <a href="https://github.com/VRLabs/SimpleShaderInspectors/issues">
    <img src="https://img.shields.io/github/issues-closed-raw/VRLabs/SimpleShaderInspectors.svg?style=flat-square">
  </a>
  <a href="https://github.com/VRLabs/SimpleShaderInspectors/issues">
    <img src="https://img.shields.io/github/issues-pr-raw/VRLabs/SimpleShaderInspectors.svg?style=flat-square">
  </a>
  <a href="https://github.com/VRLabs/SimpleShaderInspectors/issues">
    <img src="https://img.shields.io/github/issues-pr-closed-raw/VRLabs/SimpleShaderInspectors.svg?style=flat-square">
  </a>
  <br />
  <br />
</div>

## How to use

To use the library you need to create a shader inspector by substituting `ShaderGUI` with `SimpleShaderInspector`.

Here is a quick example of how a simplistic shader inspector code looks.

```csharp
using UnityEngine;
using UnityEngine;
using VRLabs.SimpleShaderInspectors;

public class TestShaderInspector : SimpleShaderInspector
{
    private PropertyControl _floatControl;
    private LabelControl _labelControl;

    public override void Start()
    {
        _floatControl = this.AddPropertyControl("_FloatProperty").Alias("MyFloatProperty");
        _labelControl = this.AddLabelControl("MyLabel");
    }

    public override void StartChecks(MaterialEditor materialEditor)
    {
        _labelControl.SetEnabled(_floatControl.Property.FloatValue > 1);
    }

    public override void CheckChanges(MaterialEditor materialEditor)
    {
        if(_floatControl.HasPropertyUpdated)
        {
            _labelControl.SetEnabled(_floatControl.Property.FloatValue > 1);
        }
    }
}
```

The above inspector has 2 controls, with the control displaying a label being displayed only when the material property `_FloatProperty` float value is above 1. The full guide that explains how all of this works can be found on the official [Documentation](https://ssi.vrlabs.dev).

We do also provide some more examples available to download in [Releases](https://github.com/VRLabs/SimpleShaderInspectors/releases/latest).

## Current state

Right now Simple Shader Inspectors is in a functional beta state, what does it mean? It means that it is a fully functional library that, if it satisfies your needs, can be used outside of pure testing enviroments, but it is not feature complete and it will for sure have multiple breaking changes before getting out of the beta state.

## License

Simple Shader Inspectors is available as-is under MIT. For more information see [LICENSE](https://github.com/VRLabs/SimpleShaderInspectors/blob/master/LICENSE).

## Downloads

There are 3 packages available in [Releases](https://github.com/VRLabs/SimpleShaderInspectors/releases/latest):

- **Simple Shader Inspectors:** base library for the end user.
- **Simple Shader Inspectors dev:** development vesion for shader creators, contains some extra tools compared to the base library.
- **Example inspectors:** good place to start looking on how the library works, requires one of the 2 packages above in order to work.

You can also download the repository itself, but unless you're looking into contributing in the project it is highly discouraged since it will very likely not end up in the right folder, possibly ending up causing problems later down the road.

## Contributors

You want to help improve the library by adding more controls and/or fixing existing bugs? Feel free to contribute by sending a pr! We are always looking for improvements to the library!

<a href="https://github.com/VRLabs/SimpleShaderInspectors/graphs/contributors">
  <img src="https://contributors-img.web.app/image?repo=VRLabs/SimpleShaderInspectors" />
</a>

Made with [contributors-img](https://contributors-img.web.app).

## Showcase

These Shaders are using Simple Shader Inspectors for their editors, check them out!

* [VRLabs Particle Shader](https://github.com/VRLabs/VRChat-Avatars-3.0) by [@VRLabs](https://github.com/VRLabs), our in house particle shader (at the moment the inspector is available only in the source code on github).

Made an awesome shader using Simple Shader Inspector and want to show it off? Join our [Discord](https://discord.gg/THCRsJc) and show it to us! If we like it we may add it to this list!
