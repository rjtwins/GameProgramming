using System;
namespace Game1.Generators
{
    public static class NameGenerator
    {
        private static Random random = new Random();

        // Arrays of prefixes and suffixes for star names
        private static string[] prefixes = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta", "Iota", "Kappa" };
        private static string[] suffixes = { "Centauri", "Cygni", "Orion", "Pegasus", "Draco", "Aquarii", "Ursae", "Corvi", "Lyrae", "Arietis" };

        // Method to generate a random star name
        public static string GenerateStarName()
        {
            string prefix = prefixes[random.Next(0, prefixes.Length)];
            string suffix = suffixes[random.Next(0, suffixes.Length)];
            return prefix + " " + suffix;
        }
    }
}