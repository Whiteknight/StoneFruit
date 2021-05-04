using System;

namespace StoneFruit.Utility
{
    public static class Assert
    {
        /// <summary>
        /// Throws an exception if the argument value is null
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="name"></param>
        public static void ArgumentNotNull(object arg, string name)
        {
            if (arg == null)
                throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Throws an exception if the argument string value is null or empty.
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="name"></param>
        public static void ArgumentNotNullOrEmpty(string arg, string name)
        {
            if (string.IsNullOrEmpty(arg))
                throw new ArgumentNullException(name);
        }
    }
}
