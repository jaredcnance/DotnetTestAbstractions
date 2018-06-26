using System;

namespace DotnetTestAbstractions.Fixtures
{
    public class DotnetTestAbstractionsFixtureException : Exception
    {
        public DotnetTestAbstractionsFixtureException(string message)
            : base(message) { }

        public DotnetTestAbstractionsFixtureException(string message, Exception inner)
            : base(message, inner) { }
    }
}