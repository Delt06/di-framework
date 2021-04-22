namespace DELTation.DIFramework.Tests.Runtime.Pocos
{
    public class Poco
    {
        public readonly PocoDep1 Dep1;
        public readonly PocoDep2 Dep2;
        public readonly PocoDep3 Dep3;

        public Poco(PocoDep1 dep1, PocoDep2 dep2, PocoDep3 dep3)
        {
            Dep1 = dep1;
            Dep2 = dep2;
            Dep3 = dep3;
        }
    }

    public class PocoDep2 { }

    public class PocoDep1
    {
        public readonly PocoDep2 Dep;

        public PocoDep1(PocoDep2 dep) => Dep = dep;
    }

    public class PocoDep3
    {
        public readonly PocoDep4 Dep;

        public PocoDep3(PocoDep4 dep) => Dep = dep;
    }

    public class PocoDep4 { }

    public class PocoLoop1
    {
        public readonly PocoLoop2 Dep;

        public PocoLoop1(PocoLoop2 dep) => Dep = dep;
    }

    public class PocoLoop2
    {
        public readonly PocoLoop1 Dep;

        public PocoLoop2(PocoLoop1 dep) => Dep = dep;
    }
}