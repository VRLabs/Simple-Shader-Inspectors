---
uid: idev-CustomTextureGenerator
title: Customizing The Texture Generator
---

# Customizing The Texture Generator

The `TextureGeneratorControl` is a really particular control, not only it is a good example of the level of complexity that a single control can reach in terms of functionality, but it is also an example of how is possible to make controls driven mainly by data and therefore customizable for the specific need.

For example if there's a need to take a color mask and apply a specific color for each channel, we can just make a shader that does that and give it to the texture generator. 

But *what* do you need to do in order to accomplish that?

## Creating the shader

First of all, you need the shader that does what you want.

We won't go too much into the details on how a shader works here since there are better guides elsewhere for that, but we will run down the main things that are important to get the shader to work in our context.

First of all, create the shader and call it however you want, in our case we call it `maskColorizer`.

Then just paste this code:

[!code-glsl[Main](Code/MaskColorizer.txt?range=1-5,12-28,32-33,49,42-45)]

This code template is used for the shader to work with the texture generator, since it has to render to a Custom Render Texture.

> [!NOTE]
> To avoid to clutter up the shader selector in a material, we start the shader path with `Hidden/`, since the shader is not meant to be used for materials anyways.

> [!NOTE]
> for more informations on the basics for a shader for Custom Render Textures, check the official [Unity docs](https://docs.unity3d.com/Manual/class-CustomRenderTexture.html).

From here, you can implement whatever logic you want to make the shader output whatever you want from the properties you pass as inputs.

[!code-glsl[Main](Code/MaskColorizer.txt?range=1-45)]

## Creating the inspector for the generator shader

Now that we have a shader, we can tell the generator to use this shader, and the generator will show a default inspector for the shader. But we can do better than that.

The generator is smart enough to detect if the shader uses a custom `ShaderGUI` (it won't detect if it uses a custom `MaterialEditor` and will revert back to a default inspector in that case) and use said ShaderGUI to show up the properties.

This means that you can customize the editor to whatever you want by just making a custom inspector. 
The library also provides a special type of inspector called `TextureGeneratorShaderInspector` to inherit from for texture generators, which will let the generator to pass on localization from the main shader inspector to the generator's shader inspector, making the localization of the generator shader dependent on the shader it's used from.

[!code-cs[Main](Code/MaskColorizerInspector.cs.txt)]

Now just add `CustomEditor "VRLabs.Examples.MaskColorizerInspector"` to the shader, with that the generator will use the custom inspector.

 ## Using the custom generator

 Now that we have both the shader and the inspector we can use the shader on our `TextureGeneratorControl`.

 The easiest way is to put the shader and inside an `Editor/Resources` folder, so what we can easily load them using `Resources.Load`, but you can use whatever method you want to load it.

 In this example the file will be inside a `Editor/Resources/Shader` folder.

 [!code-csharp[Main](Code/TextureGeneratorExample.cs.txt)]

And now the generator is ready to go and be used, give it a try!

Now this was a fairly simple example of a shader, but you can go with much *much* more complex shaders to do more advanced stuff.