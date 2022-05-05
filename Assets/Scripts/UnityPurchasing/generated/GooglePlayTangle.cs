// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("SNG++0AgVvUpYCPWit8lM422Co1W+hv2M/tq6NsfWDhFq5O/vUVHjws288ySjyOJ4eZkO1Mo7f0cUtBMN7AwKqTxg7tyDxxvfi2sHcLblpj0maSXhHl2y3JP5eA9v099Z3l0jY33ibiuJNy5Pobj/bewYfc7IN8WU9De0eFT0NvTU9DQ0WXZ8B5yZgUw0mG6mAS9bZBzI5AZ/i4BEwehlne404Y32imDvekNQKdbIM5Jv+fUCrNEETFivw6kC5OzfaZ7wL7YyUThU9Dz4dzX2PtXmVcm3NDQ0NTR0iiNOHYuS2TXXIhZY9OpLJh4qdCaliL5G2wxO6lvwBwDMpaYWk3UUHB1UbsbsR2yfFvOtXU46Ds5q+y8pp03x7zzyX8biNPS0NHQ");
        private static int[] order = new int[] { 6,13,10,5,4,7,13,11,11,10,13,11,12,13,14 };
        private static int key = 209;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
