---
uid: cdev-IAdditionalLocalization
title: Using IAdditionalLocalization
---

# Using IAdditionalLocalization

In the last example we could not have localized content for the extra label cause by default each control only has a single `Content` property.

This is quite limiting in case you want to make a control that handles multiple material properties at once (in this case you will need another interface as well), or that needs some extra text for various reasons.

And is here that the `IAdditionalLocalization` comes into play.

This interface is used to let the localization system know that the control that implements it needs more than one localization string.

[!code-csharp[Main](Code/IAdditionalLocalization.cs.txt?range=7-9&highlight=1,3)]

Implementing the interface will add the `AdditionalContent` array of type `AdditionalLocalization`. This type only contains the `Name` string which contains the name of the additional content, and the `GUIContent` we want to retrieve.

To use it, we just need to initialize the array with the number of additional strings we want, and assign the names:

[!code-csharp[Main](Code/IAdditionalLocalization.cs.txt?range=14-20&highlight=5-6)]

>[!TIP]
>If you have questions about the way the AdditionalLocalization object has been initialized here, check [this page](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#object-initializers).

>[!NOTE]
>The localization system when searching for strings of the extra localization will combine the control alias and the extra localization name in this way: `Alias_AdditionalContentName`.
>This is ensure that each control instance has a unique full name if they have different aliases.

>[!CAUTION]
>If you have more than 1 or 2 additional content to fetch, manually instancing each element may become annoying and bloat your constructor a bit too much. in this case you can have a static array of strings with all your names in the order you want to access them, and in the constructor use [`InizializeLocalizationWithNames`](xref:VRLabs.SimpleShaderInspectors.AdditionalContentExtensions.InitializeLocalizationWithNames(VRLabs.SimpleShaderInspectors.IAdditionalLocalization,System.String[]))

then we just need to use the `Content` inside the `AdditionalContent` array we created where we need it, in our case inside the `Label` call:

[!code-csharp[Main](Code/IAdditionalLocalization.cs.txt?range=27-30&highlight=3)]

Now the label field will also have its own localized content.

>[!NOTE]
>With this change `ExtraText` becomes useless, so you can safely remove any reference of it on constructors/methods

With this system you can add as much text as you want in your own controls with full support for localization, and is fairly easy to just plug it in and have it working.
