using Libplanet.Action;

namespace Lib9c.DevExtensions
{
    public class RandomImpl : IRandom
    {
        private readonly System.Random _random;

        public RandomImpl(int seed = default)
        {
            _random = new System.Random(seed);
        }

        public int Seed => 0;

        public int Next()
        {
            return _random.Next();
        }

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public void NextBytes(byte[] buffer)
        {
            _random.NextBytes(buffer);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }
    }
}
