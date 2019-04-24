using System;
using System.Threading.Tasks;
using SimpleFSM.Interfaces;

namespace SimpleFSM
{
    public class SimpleFsm<TPayload> : ISimpleFsm<TPayload> where TPayload: class
    {
        private readonly Action<TPayload> _recipe;

        /// <summary>
        /// Constructor that takes in the recipe action to execute
        /// </summary>
        /// <param name="recipe"></param>
        public SimpleFsm(Action<TPayload> recipe)
        {
            _recipe = recipe;
        }

        /// <summary>
        /// Run the machine
        /// </summary>
        /// <param name="payload"></param>
        public void Start(TPayload payload)
        {
            _recipe(payload);
        }

        /// <summary>
        /// Run the machine in async fashion
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public Task StartAsync(TPayload payload)
        {
            return Task.Factory.StartNew(() => Start(payload));
        }
    }
}