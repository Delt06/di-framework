#  Unity DI Framework

[![Version](https://img.shields.io/github/v/release/Delt06/di-framework?sort=semver)](https://github.com/Delt06/di-framework/releases)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/48d8273db00a4d93a124ed4e6736d729)](https://www.codacy.com/gh/Delt06/di-framework/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Delt06/di-framework&amp;utm_campaign=Badge_Grade)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)


[![Tests](https://github.com/Delt06/di-framework/actions/workflows/tests.yml/badge.svg)](https://github.com/Delt06/di-framework/actions/workflows/tests.yml)
[![Check code formatting](https://github.com/Delt06/di-framework/actions/workflows/linter.yml/badge.svg)](https://github.com/Delt06/di-framework/actions/workflows/linter.yml)


A simple Unity framework to inject dependencies into your components. The project contains a simple demo game to demonstrate the framework in action.

> Developed and tested with Unity 2019.4.17f1 LTS

## Table of contents
- [Installation](#installation)
- [Setting Up](#setting-up)
- [Projects using DI Framework](#projects-using-di-framework)
- [Documentation](#documentation)

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

## Projects using DI Framework
- https://github.com/Delt06/fps-roguelike

## Documentation
- [Wiki](https://github.com/Delt06/di-framework/wiki)  
- [API](https://delt06.github.io/di-framework/)
