#  ![Logo][logo] Unity DI Framework 
![Test and Build](https://github.com/Delt06/di-framework/workflows/Test%20and%20Build/badge.svg)

[logo]: Packages/com.deltation.di-framework/Assets/DELTation/DIFramework/Editor/Graphics/resolver.png "Logo"

[![Check code formatting](https://github.com/Delt06/di-framework/actions/workflows/linter.yml/badge.svg)](https://github.com/Delt06/di-framework/actions/workflows/linter.yml)

A simple Unity framework to inject dependencies into your components. The project contains a simple demo game to demonstrate the framework in action.

## Table of contents
- [Installation](#installation)
- [Setting Up](#setting-up)
- [Documentation](#documentation)

## Installation
- Open Package Manager through Window/Package Manager
- Click "+" and choose "Add package from git URL..."
- Insert the URL: https://github.com/Delt06/di-framework.git?path=Packages/com.deltation.di-framework

## Setting up
- Create a `GameObject` and attach a `Root Dependency Container` component.
- Using the menu of the attached `Root Dependency Container` (or manually), add the other container types. For detailed info on them, refer to the section [Container types](https://github.com/Delt06/di-framework/wiki/Containers).
- Define an injectable component according to the rules described in the section [Injection rules](https://github.com/Delt06/di-framework/wiki/Injection-Rules).
- Attach the created component to a `GameObject` and add a `Resolver` to it. Configure the `Resolver`, if needed: refer to [Resolvers](https://github.com/Delt06/di-framework/wiki/Resolver).

Example:
- Components structure: ![Resolver Example](Screenshots/resolver_example.jpg)
- `Movement.cs`: 
```c#
using UnityEngine;

public sealed class Movement : MonoBehaviour, IMovement
{
    [SerializeField, Min(0f)] private float _speed = 1f;

    public void Construct(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
    }

    private Rigidbody _rigidbody;
}
```

## Documentation
For more details, refer to the [Wiki](https://github.com/Delt06/di-framework/wiki).
