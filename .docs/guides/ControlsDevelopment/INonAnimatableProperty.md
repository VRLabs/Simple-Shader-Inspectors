---
id: cdev-INonAnimatableProperty
title: Using INonAnimatableProperty
---

# Using INonAnimatableProperty

When you're recording an animation every change you do gets recorded, material property changes included. But sometimes you don't want certain properties to be animated, cause maybe you're using them in a way that animating them would look weird, or simply is not something you want to be recorded if you happen to change it while the animation is recording.

The INonAnimatableProperty interface will help you with that by giving you the possibily separate the code that updates the property in a dedicated method that will never get called when the animation is recording, and notify the inspector that it needs to update a property outside the recording

>[!TIP]
>SimpleShaderInspector keeps track of controls with this interface and when it detects that some of them need to write updates to a property it will stop the animation recording if it was running, updates the non animatable property values, and then restarts the animation recording if it was stopped.

>[!WARNING]
>Due to the need of using [`Reflection`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection) to stop and restart the animation recording, with the consequent performance cost, this system is disabled by default in any inspector and to activate it the user needs to set the `HasNonAnimatableProperties` inspector property to `true` during startup. If a non animatable property control is used when the system is disabled, the update code will run without checking the recording state.

Let's take our template code with INonAnimatableProperty implemented:

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=1-7,10-15,20-23,25-26)]

INonAnimatableProperty requires you to implement 1 property and one method:

`NonAnimatablePropertyChanged` is used to let the inspector know that in this frame a material property needs to update outside of the recording.

`UpdateNonAnimatableProperty` is where we update the value of our MaterialProperty without being recorded.

Let's add a basic slider functionality o the class:

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=1-7,10-16,28,18,20-23,25-26&highlight=14-16)]

In this current state the control will still record the changes into the animation for 2 reasons:

- We don't tell the inspector that we have a property to update outside of it
- We're updating the property inside `ControlGUI`

The first one is easy to solve, we just need to set `NonAnimatablePropertyChanged` to true if the property has changed:

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=18-19&highlight=2)]

As for the second one, things may get a bit trickier.

We cannot rely on `materialEditor` to draw our property, cause the property will get automatically updated, so we need to do it ourself with `EditorGUILayout`. We also need to store the value we get back so that we can apply it to the property afterwards:

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=6-8&highlight=3)]

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=14-20&highlight=4)]

And then we update the material property value inside `UpdateNonAnimatableProperty`:

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=22-25&highlight=3)]

And with this, now our material property should update correctly without being recorded in the animation.

## Finished example class

[!code-csharp[Main](Code/INonAnimatablePropertyExample.cs.txt?range=1-26)]
