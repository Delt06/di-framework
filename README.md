# Unity DI Framework
A simple Unity framework to inject dependencies into your components.

## Installation
- Download the latest version from the `Releases` section.
- Open the `.unitypackage` in the desired Unity project and import everything.

## Setting up
- Create a `GameObject` and attach a `Root Dependency Container` component.
- Using the menu of the attached `Root Dependency Container` (or manually), add the other container types. For detailed info on them, refer to the section [Container types](#Container types).
- Define an injectable component according to the rules described in the section [Injection rules](#Injection rules).
- Attach the created component to a `GameObject` and add a `Resolver` to it. Configure the `Resolver`, if needed: refer to [Resolvers](#Resolvers). 

## Container types
*Note*: There is an order in which containers are queried. That is, if a container was not able to resolve the dependency, the next one will be queried. The order of containers is the same as in the Inspector. 

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

## Injection rules
- General rules:
    - Value types, and `in`, `out`, `ref` parameters are not supported. 
- Normal C# classes:
    - injected through public constructors.
- `MonoBehaviour`'s, `ScriptableObject`'s:
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

## "Unused" code stripping 
The plugin was verified to work with `Managed Stripping Level` set to `Low`.   
In case of any problems related to code stripping, refer to the [official manual](https://docs.unity3d.com/Manual/ManagedCodeStripping.html).

## Notes
- Developed with Unity 2019 LTS