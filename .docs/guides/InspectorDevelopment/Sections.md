---
uid: idev-Sections
title: Sections
---

# Sections

Sections are a family of controls that help organizing your inspector by grouping controls in dropdowns with nice headers, and they're so awesome that we needed to make a dedicated section just for them (no pun intended, ok maybe a bit).

The main concept is simple: you create a section based on your needs, and then you add a number of controls to it with the usual `Add*YourControlNameHere*`, but applied to your section control instead of `this`. You can even add a section inside your section if you want.

Right now there are 3 types of sections available and they each have their own use case.

## Section

```csharp
    AddSection(string alias);
```

This is the basic section with no fancy extras to it, and, as we already seen with the `LabelControl`, since it doesn't have any material property it will have a default `Alias` that will be the same for every section control, so it's advised to assign a custom one.

It also is the base class of the other 2 types of sections, so everything that is available here will be available on those as well.

Practically speaking it doesn't have too much in terms of functionality, click on the header to toggle the visibility on and off, and *that's it*.

You do however have quite a bit of appearance customizability with its chainable methods.
For example you can change the color of the header background with `.SetBackgroundColor` or change the label style with `.SetLabelStyle`, or you can change the background itself by changing its style with `.SetBackgroundStyle`.

## ActivatableSection

```csharp
    AddActivatableSection( string activatePropertyName, float enableValue = 0, float disableValue = 1);
```

This type of section on top of having everything the basic `Section` has it also has an additional checkbox in its header, and this checkbox is driven by the property you pass it during creation.

Also unlike the basic `Section` this one has a property and as a consequence it won't be required to set an `Alias`.

The checkbox will control if the content of the section will be enabled or not, and in case it is disabled all the controls under this section will be disabled and not modifiable.

Currently this section does not have chainable methods on top of what is already available from `Section`.

## OrderedSection

```csharp
   AddOrderedSection(string activatePropertyName, float enableValue = 1, float disableValue = 0);
```

This type of section could be interpreted as a different implementation of `ActivatableSection` since it works similarly on the surface, but there's one problem, you *can't* create it with the usual `this.AddOrderedSection` this time.

>[!CAUTION]
>You could create a new `OrderedSection` using its default constructor, but you should not do that. You would not get a working section out of that anyways.

The only legitimate way to create an `OrderedSection` is to call the `.AddOrderedSection` from an `OrderedSectionGroup` control that will manage its lifecycle.

So the question now is: what is an `OrderedSectionGroup`?

First of all, it's a control, and therefore is used like all other controls. That said when first created without any `OrderedSection` assigned to it will just do nothing, but as soon as you add an `OrderedSection` to it a button will appear, and clicking it will generate a popup where you can select any disabled `OrderedSection` it has.

Any enabled `OrderedSection` will be above this button.

The section itself looks mostly like the basic `Section`, but it has some additional buttons on the right side.

The 2 arrows allow you to reorded the section relative to the other sections inside the `OrderedSectionGroup`, meanwhile the x button will disable the section, making it completely disappear from the inspector until you re-enable it from the section group button.

All of these icons can be customized by making a new style for them with a different background image.

## Using properties for Visibility

All the sections we've seen keep memorized their visibility state per each material internally with a [Dictionary](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2), while this is good enough for the majority of use cases, it will lose the memorized state when unity closes, resulting on all sections being closed the next time you open unity.

As an alternative solution for that (or in case you want to keep the visibility state tracked in a `MaterialProperty`) you can use pass another property to the constructor.

This property will be used to keep track of the visibility state, and you can also add 2 float values to tell the section what value should set when visible and what value set when not visible.

>[!NOTE]
>A basic `Section` created this way will use the visibility property `Alias` and not rely on the default one.

>[!CAUTION]
>You can create float properties in you shader property list without using them in the shader code, just to keep track of the visibility state of section. Just beware that is usually not considered best practice to do that.

When using this method to create sections you also can decide if the property used for the visibility state will be recorded or not when you're [recording an animation](https://docs.unity3d.com/Manual/animeditor-AnimatingAGameObject.html) by using `SetPropertyAnimatable`.
