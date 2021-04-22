namespace DELTation.DIFramework.Tests.Runtime.Pocos
{
    public class InterfaceImpl : IInterface { }

    public interface IInterface { }

    public class InterfaceDependant
    {
        public readonly IInterface Interface;

        public InterfaceDependant(IInterface @interface) => Interface = @interface;
    }
}