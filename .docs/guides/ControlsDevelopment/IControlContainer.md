---
uid: cdev-IControlContainer
title: Using IControlContainer
---

# Using IControlContainer

Sometimes you may want to have a control being able to manage additional controls under it, for example you want a control that will show another control if its property value is above a certain threshold.

This would technically be possible by just checking the value change in the inspector and enabling/disabling the controls that should be affected, but that requires to write code to handle that for every occasion, and we want it to be automatic.

To do that we will need to use the IControlContainer interface.

Let's take our custom control template and implement the IControlContainer interface:

[!code-csharp[Main](Code/IControlContainerExample.cs.txt?range=1-8,11-12,14-17,28,35-42,33-34)]


The implement the interface you need to implement the `AddControl` and `GetControlList` methods in you class.

The first method is generally used in the generated extension methods to add a new control under this one, meanwhile the second one is used when the inspector fetches properties.

How you store those controls is up to you, in this case we will use a list.

And in the meantime, we also add a simple range property and get the control to do something:

[!code-csharp[Main](Code/IControlContainerExample.cs.txt?range=1-20,28-34&highlight=9,13,18-20,23,25)]

Some notable mentions here:
- The 2 methods implementation being done in an unusual way, this is for the most part just a "shortcut" that can be done when the method implementation consist in only one operation (if you're not comfortable with it, you can just write it like you're used to, the result is the same).
- We use `Controls.AddControl(control, alias)` to add the control in the list. `AddControl` applied to this list is a method that automatically adds the control after a control with the given alias, this should be the default implementation of an IControlContainer `AddControl`, and eventual custom implementations (if you don't use something that can be cast into an [`IList`](https://docs.microsoft.com/en-us/dotnet/api/system.collections.ilist)) should be implemented in a similar way.
- The `RangeProperty` method for some reason doesn't have an overload that accepts a `GUIContent`, so we had to pass just the `text` string of it (unity pls fix this thanks).

Now our control properly displays any float property that is being given to it, but if you add any control to this one they will simply not display, that's cause it's our responsibility to handle that:

[!code-csharp[Main](Code/IControlContainerExample.cs.txt?range=19-28&highlight=3-9)]

We first check if the property has its value above `0.5` and cycle all controls inside our list to draw them.

If you try to move the slider around you will see that other controls added to this control will show up as soon as you surpass `0.5`.

>[!TIP]
>As you probably noticed, we use the `DrawControl` method to display a control's content, but we usually override `ControlGUI` when deciding what to display in our controls. This is cause `DrawControl` internally calls `ControlGUI`, but it also does additional checks, like checking if the control should be visible to begin with, or if should be in a disabled state.

## Finished example class

[!code-csharp[Main](Code/IControlContainerExample.cs.txt?range=1-34)]
