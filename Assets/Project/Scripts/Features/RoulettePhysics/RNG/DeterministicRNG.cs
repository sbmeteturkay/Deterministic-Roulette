namespace Game.Roulette
{

    // Simple deterministic RNG based on SplitMix64 seed expansion and xoshiro128+ state
    public class DeterministicRNG
    {
        private uint s0, s1, s2, s3;

        public DeterministicRNG(ulong seed)
        {
            // splitmix64 to initialize four 32-bit states
            ulong z = seed + 0x9e3779b97f4a7c15UL;
            s0 = (uint)(Mix64(ref z) >> 32);
            s1 = (uint)(Mix64(ref z) >> 32);
            s2 = (uint)(Mix64(ref z) >> 32);
            s3 = (uint)(Mix64(ref z) >> 32);
        }

        private static ulong Mix64(ref ulong z)
        {
            z = (z ^ (z >> 30)) * 0xbf58476d1ce4e5b9UL;
            z = (z ^ (z >> 27)) * 0x94d049bb133111ebUL;
            return z ^ (z >> 31);
        }

        // xoshiro128+ next
        private uint Rotl(uint x, int k) => (x << k) | (x >> (32 - k));

        public uint NextUInt()
        {
            uint result = s0 + s3;
            uint t = s1 << 9;
            s2 ^= s0;
            s3 ^= s1;
            s1 ^= s2;
            s0 ^= s3;
            s2 ^= t;
            s3 = Rotl(s3, 11);
            return result;
        }

        public float NextFloat01()
        {
            // [0,1)
            return (NextUInt() >> 8) * (1f / (1 << 24));
        }

        public int RangeInt(int minInclusive, int maxExclusive)
        {
            int span = maxExclusive - minInclusive;
            return minInclusive + (int)(NextFloat01() * span);
        }

        public float RangeFloat(float min, float max)
        {
            return min + NextFloat01() * (max - min);
        }
    }
}
