#  Unity DI Framework

[![Version](https://img.shields.io/github/v/release/Delt06/di-framework?sort=semver)](https://github.com/Delt06/di-framework/releases)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/48d8273db00a4d93a124ed4e6736d729)](https://www.codacy.com/gh/Delt06/di-framework/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Delt06/di-framework&amp;utm_campaign=Badge_Grade)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)


[![Tests](https://github.com/Delt06/di-framework/actions/workflows/tests.yml/badge.svg)](https://github.com/Delt06/di-framework/actions/workflows/tests.yml)

A Unity framework to inject dependencies into your components. The project contains a simple demo game to demonstrate the framework in action.

> Developed and tested with Unity 2019.4.17f1 LTS

## Table of contents
- [Key Features and Concepts](#key-features-and-concepts)
- [Motivation](#motivation)
- [Installation](#installation)
- [Setting Up](#setting-up)
- [Extensions](#extensions)
- [Projects using DI Framework](#projects-using-di-framework)
- [Documentation](#documentation)

## Key Features and Concepts
- Simple and consistent injection mechanisms:
    -  Method injection for `MonoBehaviour`s (through all suitable methods named `Construct`).
    -  Constructor injection for standard C# classes (a.k.a. POCO).
- Can inject anytime, not only on scene start.
- The framework is designed for writing code without direct dependencies on the framework itself. You will not have to include framework's namespaces everywhere.
- Context-aware injection: dependencies can be automatically fulfilled with components found in children and parents.
- Potential problems are exposed even before running the game: `Resolver` has a custom Inspector, which shows all dependencies of the object and specifies whether they may be fulfilled.
- Baking: to avoid reflection (and to improve performance), injection can be baked via automatic code generation.
- General-purpose API: `Di` class contains convenience methods that provide access to explicit injection and object instantiation.

## Motivation
When working on complex objects in Unity (e.g. characters in an open world game), there is frequently a need for one system of the object to reference another. For example, animation component may require a reference to a component that is responsible for vehicle entering/exiting. It may look like this:
```c#
public class VehicleEnteringAnimation : MonoBehaviour
{
    public VehicleInteraction Interaction;
    
    private void Update()
    {
        // do work with Interaction, e.g. react to its events
    }
}
```
However, with this approach there are at least 2 problems:
- A need to always assign fields in Inspector (and maybe validate that they are assigned)
- Cannot use interfaces: Unity does not serialize them

There is an alternative approach:
```c#
public class VehicleEnteringAnimation : MonoBehaviour
{
    private VehicleInteraction _interaction;
    
    private void Update()
    {
        // do work with _interaction, e.g. react to its events
    }
    
    private void Awake()
    {
        // find the component like this:
        // _interaction = GetComponentInParent<VehicleInteraction>(); // can use interfaces now!
        // or this:
        // _interaction = FindObjectOfType<VechileInteraction(); // but here still cannot...
    }
}
```

This may work, but new problems have arisen:
- The source of dependency is hardwired into component's code, not flexible
- Sometimes requires to write lots of `GetComponent` calls

DI Framework was created to solve exactly this problem. It supports the following notation:
```c#
public class VehicleEnteringAnimation : MonoBehaviour
{
    // interaction can come from anywhere, and finally can be an interface
    public void Construct(VehicleInteraction interaction)
    {
        _interaction = interaction;
    }

    private VehicleInteraction _interaction;
    
    private void Update()
    {
        // do work with _interaction, e.g. react to its events
    }
```

The final step is to add a component called `Resolver` to the game object to inject the dependency. One `Resolver` will take care of the game object it is attached to and of all its children.

The benefits of the approach are:
- Explicit dependencies: they are always visible at the top of the class definition
- Testability: can call `Construct` method manually to inject anything there (useful for mocking)
- Inversion of control: components no longer specify where the dependencies come from, this responsibility is offloaded to the `Resolver`
- Validation: inability to resolve a dependency will be displayed in the Inspector of the object (at `Resolver`)

## Installation
### Option 1
- Open Package Manager through Window/Package Manager
- Click "+" and choose "Add package from git URL..."
- Insert the URL: https://github.com/Delt06/di-framework.git?path=Packages/com.deltation.di-framework

### Option 2  
Add the following line to `Packages/manifest.json`:
```
"com.deltation.di-framework": "https://github.com/Delt06/di-framework.git?path=Packages/com.deltation.di-framework",
```

## Setting up
- Create a `GameObject` and attach a `Root Dependency Container` component.
- Using the menu of the attached `Root Dependency Container` (or manually), add the other container types. For detailed info on them, refer to the section [Container types](https://github.com/Delt06/di-framework/wiki/Containers).
- Define an injectable component according to the rules described in the section [Injection rules](https://github.com/Delt06/di-framework/wiki/Injection-Rules).
- Attach the created component to a `GameObject` and add a `Resolver` to it. Configure the `Resolver`, if needed: refer to [Resolvers](https://github.com/Delt06/di-framework/wiki/Resolver).

Example:
- Components structure:  
![Resolver Example](https://github.com/Delt06/di-framework/blob/master/Screenshots/resolver_example.jpg?raw=true)
- `Movement.cs`: 
```c#
using UnityEngine;

public sealed class Movement : MonoBehaviour
{
    public void Construct(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
    }

    private Rigidbody _rigidbody;
}
```

## Extensions
- [Event System](https://github.com/Delt06/di-events)

## Projects using DI Framework
- https://github.com/Delt06/fps-roguelike
- https://github.com/Delt06/leo-ecs-extensions

## Documentation
- [Wiki](https://github.com/Delt06/di-framework/wiki)  
- [API](https://delt06.github.io/di-framework/)
