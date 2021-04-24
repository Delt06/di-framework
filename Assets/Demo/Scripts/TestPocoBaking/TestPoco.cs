namespace Demo.Scripts.TestPocoBaking
{
    public class TestPoco
    {
        public TestPocoDependency Dependency;

        public TestPoco(TestPocoDependency dependency) => Dependency = dependency;
    }

    public class TestPocoDependency { }
}