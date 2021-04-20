#  ![Logo][logo] Unity DI Framework 
[logo]: Packages/com.deltation.di-framework/Assets/DELTation/DIFramework/Editor/Graphics/resolver.png "Logo"

![Test and Build](https://github.com/Delt06/di-framework/workflows/Test%20and%20Build/badge.svg)
[![Check code formatting](https://github.com/Delt06/di-framework/actions/workflows/linter.yml/badge.svg)](https://github.com/Delt06/di-framework/actions/workflows/linter.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/48d8273db00a4d93a124ed4e6736d729)](https://www.codacy.com/gh/Delt06/di-framework/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Delt06/di-framework&amp;utm_campaign=Badge_Grade)

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

public sealed class Movement : MonoBehaviour
{
    public void Construct(Rigidbody rigidbody)
    {
        _rigidbody = rigidbody;
    }

    private Rigidbody _rigidbody;
}
```

## Documentation
For more details, refer to the [Wiki](https://github.com/Delt06/di-framework/wiki).
