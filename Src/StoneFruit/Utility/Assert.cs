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
    }
}
