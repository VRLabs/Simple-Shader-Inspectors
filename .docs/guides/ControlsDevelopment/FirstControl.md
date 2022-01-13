---
uid: cdev-FirstControl
title: First control
---

# First Control

Controls are the central piece of Simple Shader Inspectors, so it's only natural that at some point you may need to make your own controls if you're doing something particular with your shader and you want an user friendly way to control it in the inspector.

And luckily making a customized control is not too hard.

>[!CAUTION]
>Unlike just using the API, making custom controls will *require* you to have a basic knowledge on how to manually display stuff in the inspector using `MaterialProperty`, `MaterialEditor` and how the [`IMGUI`](https://docs.unity3d.com/Manual/GUIScriptingGuide.html) systems works.

First things first, let's look at an empty template:

[!code-csharp[Main](Code/FirstControl.cs.txt?range=1-8,31,14,17,19-20,28-30)]

As we can see the class `MyCustomControl` inherits from `PropertyControl`, which is the base class for all controls that use one material property.

>[!NOTE]
>You can inherit from other controls as well if you want to inherit some of their specific functionalities.

>[!CAUTION]
>If you want to make a control that doesn't use any material property, the base class should be `SimpleControl`.
>`PropertyControl` also inherits from this class.

When inheriting from `PropertyControl` you will always need to override the `ControlGUI` method.
This method is what gets called each time the inspector has to draw your control, therefore all the GUI stuff goes there.

The constructor should always call the base constructor to correctly initialize the `PropertyName` string (you can initialize it manually if you want, it's just simpler to pass the string to the base constructor).

After that, you're free to do whatever you want with the constructor.

Let's start customize it to our needs, we will make a control that will only take 1 texture and diplays it, but also had an additional label in the row below for a longer description.

To do that we need an additional string containing the text we wanna show:

[!code-csharp[Main](Code/FirstControl.cs.txt?range=7-9&highlight=3)]

>[!TIP]
>We have the `set` as protected cause we don't want it be modified from the outside, but we still want to it to be usable if someone will ever inherit from this control.

Now we need the constructor to initialize the label as well.

[!code-csharp[Main](Code/FirstControl.cs.txt?range=13-15,17&highlight=1,3)]

Now we need to draw them in the `ControlGUI` method:

[!code-csharp[Main](Code/FirstControl.cs.txt?range=19-23,35,28&highlight=3-6)]

As you see, we did not fetch the material property, cause it gets automatically fetched for us by the inspector, so we get right to the draw part and we do a `BeginChangeCheck` so that everything we do next will be tracked for changes. Then we draw our texture property, end the change check assigning the result to `HasPropertyUpdated`, and draw our additional label.

You can see that we use the `Content` field inherited from `PropertyControl` as a label for our texture. This is cause the localization system fetched the localized control string for us, so we don't need to worry about it.

>[!NOTE]
>In this example the additional label uses a predefined string, this is not optimal if we will have multiple localizations, [here](xref:cdev-IAdditionalLocalization) we will revisit the control to add support for another localized string.

As a final touch let's make the user able to decide whether or not he wants to display the additional label:

[!code-csharp[Main](Code/FirstControl.cs.txt?range=9,11&highlight=2)]

[!code-csharp[Main](Code/FirstControl.cs.txt?range=32,14-15,34,17&highlight=1,4)]

>[!NOTE]
>Doing `bool isExtraLabelVisible = true` makes so the parameter is not required to make the method call, and if the parameter is not given a default value is used instead (in this case `true`).

[!code-csharp[Main](Code/FirstControl.cs.txt?range=19-28&highlight=6-7,9)]

## Adding the New Control extension method

At this state, the control technically already works, but it's a fairly different experience using this compared to the default ones, since we have to manually call the constructor and assign the control to the list in the inspector.

This is because there are no `Extension methods` to create and assign the control.

Simple Shader Inspectors comes with a tool that will automatically generate all extension methods required from a namespace, and saves it into a class. It can be found at `VRLabs/Simple Shader Inspectors/Generate Extension Methods`.

Once opened you will need to select what base library namespace it's going to use from a list (more copies of the library in different namespaces can exist) and which namespace you want to check for controls (for your controls, it should always be a different namespace from the one the library resides).
When clicking `Generate extension methods` you will be prompted to select a folder, select one that is under an `Editor` folder. The norm is to have a folder called `Autogenerated` to where generated scripts will be put.

![inspector](/images/docs/cdev/FirstControl/1-0.3.png)

The tool will generate all extension methods for constructors and properties marked with the `FluentSet` attribute for all controls inside the selected namespace and subnamespaces.

## Generating an extension set method for a property

Usually you want the extension method to exactly match the constructor parameters, but in our case we did not put the boolean for the label. This is intentional, cause now we're gonna move that boolean out of the constructor and add the `FluentSet` attribute to the property definition.

[!code-csharp[Main](Code/FirstControl.cs.txt?range=9-11&highlight=2)]

[!code-csharp[Main](Code/FirstControl.cs.txt?range=13-17&highlight=1,4)]

By adding the `FluentSet` attribute you're telling the generator script to also generate an extension method for this property (you will need to run the generator script again).

Now the question is: when should a field be initialized with a parameter in the constructor vs having an extension method?

It depends, on classes that are not meant to have child classes or has fields where it's required to have a value different from a default in order to work, then initializing them with a dedicated parameter in the constructor makes sense, in other cases you may just give a default value to it in the constructor and let the user decide if he wants to modify it by calling an extension method.

You can also have both at the same time, if you so desire.

## Final example class

[!code-csharp[Main](Code/FirstControl.cs.txt?range=1-30)]
