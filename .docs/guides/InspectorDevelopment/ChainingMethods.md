---
uid: idev-ChainingMethods
title: Chaining methods
---

# Chaining Methods

Let's take the following line from our example inspector in [getting started](xref:idev-GettingStarted):

```csharp
    _floatControl = this.AddPropertyControl("_FloatProperty").Alias("MyFloatProperty");
```

In this single line there's quite a bit going on. first it creates a new `PropertyControl` for our `_FloatProperty` and assigns it to an internal list.
Then it takes this control and change its `Alias` to `"MyFloatProperty"`.
Finally the control is assigned to our local `_floatControl` field.

If you ever used the [method syntax in LINQ](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/query-syntax-and-method-syntax-in-linq) you are probably already familiar with this, but if you never used it you may be slightly confused by the above line, since usually you would be more used to something like this:

```csharp
    _floatControl = new PropertyControl("_FloatProperty");
    _floatControl.PropertyAlias = "MyFloatProperty";
    this.Controls.Add(_floatControl);
```

In terms of what they do they are equivalent, but in the first case we use methods that give the control itself as a return value, giving you the possibility to "chain up" another method call to do something else, or to assign that control to a variable to use later.

By manipulating controls this way we can reduce the code needed to do the same amount of things, and keep it relatively simple to read.

The advantages of it become more apparent on more complex controls where you want to optionally modify different stuff on the spot:

```csharp
    _section = this.AddSection("MySectionAlias").SetBackgroundColor(Color.red)
        .IncludeControlsInHeader(true).ShowFoldoutArrow(false).SetEnabled(false);
```

```csharp
    _section = new Section();
    _section.PropertyAlias = "MySectionAlias";
    _section.BackgroundColor = Color.red;
    _section.AreControlsInside = true;
    _section.ShowFoldoutArrow = false;
    _section.IsEnabled = false;
    this.Controls.Add(_section);
```

As you can see, in this case we saved the need to write up a fairly sizeable amount of code just to initialize our `_section` control by chaining up method calls.

>[!TIP]
>The `Section` control is a particular control that can help you a lot to organize an inspector, we talk more in depth about it [here](xref:idev-Sections).


Every control can have its own dedicated chainable methods, and controls derived from other controls also inherit them. If you want to know what chainable methods a control has you can check up the API.

## Default chainable methods

These chainable methods are available to all controls since they are inherited from the `SimpleControl` base class:

```csharp
    Alias(string alias)
```

Set a custom alias for control localization.

This is like a unique identifier that the inspector can use when looking for localization data.

```csharp
    SetVisible(bool visible)
```

Set if the control should be visible or not.

```csharp
    SetEnabled(bool enabled)
```

Set if the control should be enabled or not.

## Chainable constructor methods

As we saw until now, to avoid having to manually assing a control to the internal list of the inspector we use `this.Add*YourControlNameHere*`, this internally creates a new control and assings it to the object in which you called the method from (in our case `this`, or in other words out inspector).

This is not limited to the inspector itself, since these methods will work on anything that implements the `IControlContainer` interface (`SimpleShaderInspector` itself implements this interface). This means that controls themself can contain more controls inside themself, and they will control how to display them.

>[!TIP]
>These methods, like all chainable methods in the API, are [`Extension methods`](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods), meaning that they're technically not part of the class itself, but can be applied to that class as if it was part of it.
>The main limitation is that since extension methods are not part of the class they can only access to public properties or methods of the class they're targeting.
>This is also the reason to why we need to use `this.` when calling one of these methods in the inspector.

For example if we have a `ToggleDropdownControl` called `toggle` we can add a `PropertyControl` in it by doing:

```csharp
    toggle.AddPropertyControl("_MyPropertyName");
```

>[!TIP]
>`ToggleDropdownControl` is a toggle that when enabled will diplay other controls underneath itself, is useful when you need to make some properties visible only if something is enabled.

