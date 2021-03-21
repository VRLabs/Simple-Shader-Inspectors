---
uid: idev-CustomTextureGenerator
title: Customizing The Texture Generator
---

# Customizing The Texture Generator

The `TextureGeneratorControl` is a really particular control, not only it is a good example of the level of complexity that a single control can reach in terms of functionality, but it is also an example of how is possible to make controls driven mainly by data and therefore customizable for the specific need.

For example if there's a need to take a color mask and apply a specific color for each channel, we can just make a compute shader that does that, give it to the texture generator, and tell him to show 1 texture input and 4 color inputs to feed the compute shader on. 

But *what* do you need to do in order to accomplish that?

## Creating the compute shader

First of all, you need the compute shader that does what you want, and you also need it to have what the control expects to find to feed the data on.

We won't go too much into the details on how a compute shader works here since there are better guides elsewhere for that, but we will run down the main things that are important to get the compute shader to work in our context.

First of all, create the file for the compute shader and call it however you want, in our case we call it `maskColorizer.compute`, but as long as the extension is right the name doesn't matter too much.

Then just paste this code:

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=1-21)]

This code is required for each compute shader to work with the texture generator, since most of the input data will be fed here. But let's look a bit more into it:

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=1)]

This is a basic kernel definition for a compute shader, it says which function will be used as a valid entry point for the compute shader, in our case it will be `MaskColorizer`, you can edit the name to whatever you want, but the entry function will need to have the same name, also the name will also be used later to tell the control which entry point to use.

>[!WARNING] 
>You can have more than one kernel in a single file, so theoretically you could have all your compute functions in a single file and tell the generator which one to use each time, we advise to NOT do that, mostly because it can become very messy very quickly.

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=4-6)]

The first variable is our end result, here we will be saving the final texture generated, and is where the generator will take the texture to save it to a file.

The other 2 floats are, as you can guess, the width and height the result texture.

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=8-15)]

This structure is the informations that the generator will pass for each texture it feeds in (outside of the texture itself).

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=17-18)]

These 2 buffers are arrays containing the texture metadata of all textures and all colors that the generator feeds to the compute shader, the order they are fed in is the same order of display in the generator itself.

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=21)]

Here each texture the generator has as an input has to be declared by its own, this is due to how textures need to be fed to the compute shader. In this case the generator only has a single texture, so only one variable is declared. You can name them however you want but keep their name in mind since you will need to use them later on.

Outside of the declaring function with the same name of the kernel, this is all you need in the compute shader to make it work with the texture generator, *but* there is one more thing that you *should* add to the compute shader, and while it isn't always necessary, it makes some checks easier and more consistent with the options the generator provides with its inputs (and in this example you will need one of the functions in here).

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=23-72)]

As you can see they're mostly selection or color space conversion functions, which go along very well with the metadata provided by the generator for each texture.

Now is finally time to do what we want the generator to do:

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=74-92)]

So, first of all, if you have no idea what `[numthreads(16,16,1)]` means, just leave it like that, it's actually needed to be exactly like that for the generator to work property.

That said, the function declaration needs to have the same name of the kernel, in this case `MaskColorizer`.

The `id` passed as a uint3 (3 ints packed into one structure) will be used as a way to get the textures uvs.

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=78)]

Unlike normal shaders "uvs" here are not normalized 0-1 values, but instead are indexes of the pixels of the textures. This means that for each texture you need to do a conversion to get the pixel at the same relative position, which is what is done here.

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=81)]

When working in compute shaders texture fed to it may be either in linear or gamma color space, so here we check the color space and convert it to linear in case it's in gamma space.

[!code-glsl[Main](Code/MaskColorizer.compute.txt?range=84-88)]

If you come from normal shaders should be fairly simple to understand what is done here to get the color masking done, if you don't come from normal shader we're not sure why you're in this site in the first place.

The only notable thing is the `Colors` array: this is the array of colors fed by the generator, and in this case the first color is the base color, meanwhile the other 4 are the colors for each channel.

[!code-csharp[Main](Code/MaskColorizer.compute.txt?range=91)]

As last thing, we set the final value of the fragment with the color we calculated.

## Creating the generator input settings json

Now that we have a compute shader, we need to tell the generator which data it has to feed. This is done by giving it a json file with the required data (in this example the json will be saved as `maskColorizerSettings.json`).

[!code-json[Main](Code/MaskColorizerSettings.json.txt)]

Simple enough right? The json is laid out the following way:

 - KernelName: name of the kernel that the generator has to use, must be the same name of the function of the compute.
 - Inputs: array of all the inputs the generator has to use.
   - Type: type of input, 0 = Texture, 1 = Color.
   - InputName: Name of the input field, for textures it has to be the same of the name used in the compute shader, for colors it only matters for the localization string used by the generator
   - Settings: Array of int values that indicates some additional settings for the input field

Texture input settings:
 - [0] : Show texture channel selector (0 = hidden, 1 = shown)
 - [1] : Show reverse option (0 = hidden, 1 = shown)

Color input settings:
 - [0] : Show colorspace selector (0 = hidden, 1 = shown)

 ## Using the custom generator

 Now that we have both the compute shader and the input settings json we can use them on our `TextureGeneratorControl`.

 The easiest way is to put both the compute shader and the settings json inside a `Editor/Resources` folder, so what we can easily load them using `Resources.Load`, but you can use whatever method you want to load them.

 In this example the files will be inside a `Editor/Resources/Compute` folder.

 [!code-csharp[Main](Code/TextureGeneratorExample.cs.txt)]

And now the generator is ready to go and be used, give it a try!

Now this was a fairly simple example of compute shader, but, just like normal shaders, you can go with much *much* more complex shaders to do more advanced stuff.