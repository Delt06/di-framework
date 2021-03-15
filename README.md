#  ![Logo][logo] Unity DI Framework 
![Test and Build](https://github.com/Delt06/di-framework/workflows/Test%20and%20Build/badge.svg)

[logo]: ./Packages/com.deltation.di-framework/Assets/DELTation/DIFramework/Editor/Graphics/resolver.png "Logo"

[![Check code formatting](https://github.com/Delt06/di-framework/actions/workflows/linter.yml/badge.svg)](https://github.com/Delt06/di-framework/actions/workflows/linter.yml)

A simple Unity framework to inject dependencies into your components.

## Installation
- Open Package Manager through Window/Package Manager
- Click "+" and choose "Add package from git URL..."
- Insert the URL: https://github.com/Delt06/di-framework.git?path=Packages/com.deltation.di-framework

## Setting up
- Create a `GameObject` and attach a `Root Dependency Container` component.
- Using the menu of the attached `Root Dependency Container` (or manually), add the other container types. For detailed info on them, refer to the section [Container types](#container-types).
- Define an injectable component according to the rules described in the section [Injection rules](#injection-rules).
- Attach the created component to a `GameObject` and add a `Resolver` to it. Configure the `Resolver`, if needed: refer to [Resolvers](#resolvers). 

## Container types
 

- `Children Dependency Container`: registers all active children.
- `List Dependency Container`: registers all the objects specified via the Inspector. Given a `GameObject` is selected, it allows to specify the exact component on it.
- `Fallback Dependency Container`: scans for all conforming objects on the active scenes. Since it relies on `FindObjectsOfType()`, it can only return objects deriving from `UnityEngine.Object`. 

- Defining custom containers: allows to register any dependencies from code. Very useful if the registered types do not derive from `UnityEngine.Object`. 
```c#
public sealed class CompositionRoot : DependencyContainerBase
{
    protected override void ComposeDependencies(ContainerBuilder builder)
    {
        // Compose your dependencies here:
        // builder.Register(new T());
        // builder.Register<T>();
    }
}
```

*Note 1*: There is an order in which containers are queried. That is, if a container was not able to resolve the dependency, the next one will be queried. The order of containers is the same as in the Inspector.  
*Note 2*: Multiple root containers **are** supported. Their priority is based on their registration time: the latter the higher. Root containers get registered and unregistered in `OnEnable` and `OnDisable` respectively.  

## Injection rules
- General rules:
    - Value types, and `in`, `out`, `ref` parameters are not supported. 
- Normal C# classes:
    - injected through public constructors.
- `MonoBehaviour`'s:
    - injected through *ALL* public methods named `Construct`. For example:
```c#
public class Example : MonoBehaviour 
{
    public void Construct(Camera cam) 
    {
        _cam = cam;
    }

    private Camera _cam;
}
```

## Resolvers
A `Resolver` is a component that injects the dependencies of the `GameObject` (and its children) that it is attached to.  

### Resolver Inspector
The Inspector menu of the Resolver allows to specify where to get the dependencies from.  
Additionally, all the resolved/not resolved dependencies are displayed.

![Resolver Inspector](Screenshots/resolver.jpg)

## Rider Templates
Useful Rider Templates (macros) can be found [here](./Macros).

## "Unused" code stripping 
- The plugin was verified to work with `Managed Stripping Level` set to `Low`.  
- A `link.xml` file in the `DI` folder root prevents the default Unity assembly (`Assembly-CSharp`) from stripping. 
- If you use assembly definitions in your code, you should create extra `link.xml` files to ensure nothing would get stripped. 
- In case of any problems related to code stripping, refer to the [official manual](https://docs.unity3d.com/Manual/ManagedCodeStripping.html).

## Notes
- Developed with Unity 2019 LTS (2019.4.17f1)
