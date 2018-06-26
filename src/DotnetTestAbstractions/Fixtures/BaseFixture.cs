using System;

namespace DotnetTestAbstractions.Fixtures
{
    public class BaseFixture
    {
        static BaseFixture()
        {
            var randTick = DateTime.Now.Ticks & 0x0000FFFF;
            Seed = Int32.Parse(Environment.GetEnvironmentVariable("TEST_SEED") ?? randTick.ToString());
        }

        /// <summary>
        /// A test seed that can be used for data generation.
        /// This seed can be overriden by setting the environment variable 'TEST_SEED'.
        /// </summary>
        /// <example>
        /// ### Using the seed with Bogus
        /// ```
        /// Randomizer.Seed = new Random(Seed);
        /// ```
        /// 
        /// ### Setting the seed to debug failure (OSX)
        /// ```
        /// export TEST_SEED="123456"
        /// ```
        /// </example>
        protected static int Seed { get; private set; }
    }
}