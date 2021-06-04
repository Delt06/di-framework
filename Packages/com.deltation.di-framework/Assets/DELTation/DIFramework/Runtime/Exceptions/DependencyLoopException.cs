using System;

namespace DELTation.DIFramework.Exceptions
{
    public class DependencyLoopException : Exception
    {
        public DependencyLoopException() : base("Dependency graph has a loop.") { }
    }
}