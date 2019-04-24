using System;

namespace SimpleFSM.Utilities
{
    public class LambdaHelper
    {
        /// <summary>
        /// Run the action and return this
        /// </summary>
        /// <param name="action"></param>
        /// <param name="this"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Run<T>(Action action, T @this)
        {
            action();
            return @this;
        }

        /// <summary>
        /// Run the action and return void
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static void Run(Action action)
        {
            action();
        }
    }
}