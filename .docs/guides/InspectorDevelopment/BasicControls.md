---
uid: idev-BasicControls
title: Basic controls
---

# Basic Controls

On Simple Shader Inspectors you show fields using different types of controls based on what you need to display. Need to show a texture? use `TextureControl`, want a color instead? use `ColorControl` and so on.

If you check the API you will see that you have a fair amount of options regarding controls to use, included some really specific (but powerful) ones, but here we summarize some of the most basics ones that you will probably end up using a lot.

## PropertyControl

```csharp
AddPropertyControl(propertyName);
```

This is the definition of basic, will just display the material property like the inspector would do automatically (it uses `MaterialEditor.ShaderProperty`).
It can come really useful when a property doesn't need anything fancy to display and is already nicely done automatically, for example a range property.
This is also the base control that all controls that use material properties inherit from.

## TextureControl

```csharp
AddTextureControl(texturePropertyName, extraPropertyName1, extraPropertyName2);
```

`TextureControl`, as the name says, is used to display textures. Internally uses `materialEditor.TexturePropertySingleLine` to display its properties, and like the latter can take up to 2 extra properties to show in the same line as the texture.

>[!TIP]
>Despite having up to 3 properties, you still only have access to  the usual `HasPropertyUpdated` bool to check if any of the 3 properties changed.

## ColorControl

```csharp
AddColorControl(string colorPropertyName);
```

While you can show color properties just fine with a `PropertyControl` you end up having the color box taking up the entirety of the row, and that doesn't match with the color box that is being shown with a `TextureControl`. So this control fixes that. It can also take an additional boolean value to indicate if the color box (and color picker) needs to handle the alpha channel (by default it's set to `true`).

## LabelControl

```csharp
AddLabelControl(string alias);
```

It's one of those weird controls that does not use any property and instead requires you to insert an `Alias` to get its own localization.

In this case it draws a simple label with nothing more going on.
