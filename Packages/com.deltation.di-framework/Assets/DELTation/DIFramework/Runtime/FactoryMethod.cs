namespace DELTation.DIFramework
{
    public delegate TR FactoryMethod<out TR>()
        where TR : class;

    public delegate TR FactoryMethod<out TR, in T1>(T1 a1)
        where TR : class
        where T1 : class;

    public delegate TR FactoryMethod<out TR, in T1, in T2>(T1 a1, T2 a2)
        where TR : class
        where T1 : class
        where T2 : class;

    public delegate TR FactoryMethod<out TR, in T1, in T2, in T3>(T1 a1, T2 a2, T3 a3)
        where TR : class
        where T1 : class
        where T2 : class
        where T3 : class;

    public delegate TR FactoryMethod<out TR, in T1, in T2, in T3, in T4>(T1 a1, T2 a2, T3 a3, T4 a4)
        where TR : class
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class;

    public delegate TR FactoryMethod<out TR, in T1, in T2, in T3, in T4, in T5>(T1 a1, T2 a2, T3 a3, T4 a4, T5 t5)
        where TR : class
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class;
}