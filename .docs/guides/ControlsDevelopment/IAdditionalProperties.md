---
uid: cdev-IAdditionalProperties
title: Using IAdditionalProperties
---

# Using IAdditionalProperties

In some cases you may need to use multiple material properties in a single control.

An example is making a texture control similar in functionality to `MaterialEditor.TexturePropertySingleLine`.

Just like the `IAdditionalLocalization` interface can make you get multiple localized strings, `IAdditionalProperties` can be used for material properties themself. The biggest difference is that in this case you need a way to get the material property name of the additional properties from the inspector.

This time let's check the `TextureControl` code that Simple Shader Inspectors comes with, since it uses this interface to have 2 extra properties. The file can be found inside `"VRLabs/SimpleShaderInspectors/Editor/Controls/TextureControl.cs"`.

>[!TIP]
>This is also a good time to see a real control use case instead of examples that are done for the sake of an example.


First let's look at the implementation of the `IAdditionalProperties` interface.

[!code-csharp[Main](Code/TextureControl.cs.txt?range=6-7,11-13)]

As you can see, to implement the interface we need to add an array of type `AdditionalProperty[]`, which will contain both our property name string and out material property that will automatically be fetched by the inspector.

let's now check in the constructor:

[!code-csharp[Main](Code/TextureControl.cs.txt?range=26-35)]

Here the array is initialized with an array length of 2, and then we initialize both of them by giving them the name of the material property they need to fetch. (we are purposely ignoring the 2 extra checks since they're out of the scope of this page).

Now during `ControlGUI` we can use them as we wish (the example below is taken from the `DrawTextureSingleLine` method, but that method is called inside `ControlGUI` so for our example is the same thing).

[!code-csharp[Main](Code/TextureControl.cs.txt?range=56-70)]

## Overuse of IAdditionalProperties interface

While this is incredibly useful in a lot of cases, a serious risk of abusing this interface is creating giant controls that handle half of the inspector alone.

Let's take in consideration an hypotetical control that has 2 textures, one for a color map and one for a normal map, at first look it would make sense to merge them in a single control, due to their frequent use together, but it's really not.

 Before including more properties to a control, ask yourself the following questions:

- Are these properties directly correlated to each other in some way?
- Do they lose their overall meaning if split apart in 2 completely different areas of the inspector?

If the answer is no to one of them, you probably should consider other options to handle them instead of including them in a single control.

In case of the official texture control the answer to them is:

- Yes, because the second and third properties in this control should be used for properties that directly manipulate the texture (for example a texture color, or a normal map intensity).
- Yes, because if, for example, you put your texture field at the beginning and its color at the end of the inspector, when you reach the color you have no real reference to what that color modifies, maybe wrongly implying that the color is for standalone effects when in reality it just applies a tint to the texture.

If we take into consideration our dual texture control the answers differ a bit:

- Not really, the 2 textures are used together really often, but they represent really different informations.
- No, if they are in 2 different areas of the inspector they still have their full meaning, and having them separated marks even more their independent functions. At worst the inspector would look a bit messy if they're not in the same general area.

As third general rule: if you have more than 5-6 properties in a single control, there's a good chance you're doing something conceptually wrong. And if all those properties are really that intertwined between each other, let us know what the hell you're doing with that shader, cause we're definitely curious to see it.
