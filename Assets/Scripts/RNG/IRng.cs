namespace RNG
{
    public interface IRng { int Next(int minIncl, int maxExcl); }
    public class UnityRng : IRng { public int Next(int a, int b) => UnityEngine.Random.Range(a, b); }
}
