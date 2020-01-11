using System;

namespace StoneFruit.Utility
{
    public static class Assert
    {
        public static void ArgumentNotNull(object arg, string name)
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        public static void ArgumentNotNullOrEmpty(string arg, string name)
        {
            if (string.IsNullOrEmpty(arg))
                throw new ArgumentNullException(name);
        }
    }
}
