using System;

namespace Helper
{
    public class RandomHelper
    {
        public static string CreateNum()
        {
            long i = 1;
            foreach (var b in Guid.NewGuid().ToByteArray())
            {
                i *= (b + 1);
            }
            return $"{i - DateTime.Now.Ticks:x}";
        }
    }
}