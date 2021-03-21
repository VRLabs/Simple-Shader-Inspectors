---
uid: idev-DistributingYourShader
title: Distributing your shader
---

# Distributing Your Shader

If you're making a customized shader inspector you probably belong in 1 of these 2 categories:

- you're making a shader for internal use in your dev team and you need to be sure that's easy to use for your artists.
- you're making a shader with the purpose of distributing it.

In the first case you're good to go, as long as you included the Simple Shader Inspectors package in your team's project it will work fine.

However in the second case you'll have the problem of distributing it, since it has our library as a dependency, and be sure that it won't cause problems in the eventuality that the end user's project already has some shader using our library, which may end up causing duplicate script errors or incompatibilities.

Currently these are the ways we currently encourage to distribute our library with your shaders:

## Reference to our package Repository

The first and simplest way is to not include our library directly but inform the end user that Simple Shader Inspectors is a requirement for your shader in order to work.

Pros:

- Is the easiest method.
- Slimmer package.

Cons:

- Is responsibility of the user to install our library, resulting in higher chance of user error.
- If some breaking changes happen within our library, you will have to update your editor to support it, or you end up having users downloading the latest version of our library and having it not work with your shader.

## Include our code in your unity package

Since Simple Shader Inspector is under MIT, you can freely include it in your unitypackage. If your main distribution method is Github (or equivalents), you can also include it in just the release package without the need to include it in your source code as well (in this case mention it in your README).

>[!WARNING]
>If you choose this method, please be sure to leave it in its default location and that its component's GUID did not change (you can test that by deleting our package and then reimport it, if you get console warning about duplicate GUID it means that unity changed that, in this case please move everything in a new project with our package installed first).

Pros:

- The user doesn't have to do additional steps.
- Doesn't need to be included in the source code itself as long as its included in the downloadable package.

Cons:

- It has the possibility of ending up with duplicate scripts if the user has also installed a shader that did not respected the above warning.
- If the user downloads a shader that uses a newer version of our library it will override the version you have included, and if the newer version has some breaking changes, your inspector won't work, so you need to keep your shader inspector up to date with our library version.

## Embed our code in your editor folder

This is by far the safest way to include our library in your shader, by making a copy of the necessary code into a separate folder inside your shader, under your namespace. We also included a tool to help you in the process by automatically copying the classes in your editor folder and change namespace to one defined by you. Remember to update your inspector's namespace `using` references.

Pros:

- As long as you put your original namespace, you won't have code conflicts even if there are multiple copies of it from different shaders.
- The user doesn't have to do additional steps.

Cons:

- If you want to use newer versions of Simple Shader Inspectors you will need to redo the entire process.
- The user may not understand that your shader is actually using an additional library and may confuse eventual bugs inside our library as bug in your shader inspector.

## Final thoughts

As you can see there are multiple takes on the situation, each with pros and cons, choose one depending on your workflow. For example if you have a large audience not including the library at all and just mention that it has a requirement could end up in receiving a lot help requests from users unaware of the requirement, cause they may have downloaded your shader without consulting the README first.

For example here at VRLabs we include the library inside our shader unity packages without actually embedding it, but we are also the ones developing it, meaning that a good part of the disadvantages of this method won't affect us at all.
