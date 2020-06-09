using System.Runtime.CompilerServices;

namespace TwitchBot.Common.Extensions
{
    /// <summary>
    /// Helper method that returns calling namespace and method
    /// Typical usage is in logging, to help identify where in the code logging is related to.
    /// </summary>
    public static class ClassNameExtension
    {
        /// <summary>
        /// Returns full namespace (of calling method)
        /// Usage :  ClassNameHelper.GetCallingNamespace(this);
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCallingFullNamespace(this object type)
        {
            return type.GetType().FullName;
        }


        /// <summary>
        /// Returns class name (of calling method)
        /// Usage :  ClassNameHelper.GetCallingClass(this);
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCallingClass(this object type)
        {
            return type.GetType().Name;
        }


        /// <summary>
        /// Returns method name (of calling method)
        /// Usage : ClassNameHelper.GetCallingMethod()
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string GetCallingMethod([CallerMemberName] string caller = null)
        {
            return caller;
        }


        /// <summary>
        /// Returns a compund of method class and name (of calling method)
        /// Usage : ClassNameHelper.GetCallingMethod(this)
        /// </summary>
        /// <param name="caller"></param>
        /// <returns></returns>
        public static string GetCallingClassAndMethod(this object type, [CallerMemberName] string caller = null)
        {
            return $"[{type.GetType().Name}.{caller}] : ";
        }


    }
}