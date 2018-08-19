using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SimpleFSM.Extensions;
using SimpleFSM.Interfaces;
using SimpleFSM.Utilities;
using static SimpleFSM.Utilities.LambdaHelper;

namespace SimpleFSM.Builders
{
    public class SimpleFsmBuilder<TNode, TPayload> : BaseBuilder<SimpleFsmBuilder<TNode, TPayload>>, ISimpleFsmBuilder<TNode, TPayload>
        where TNode: struct, IConvertible
        where TPayload: class
    {
        /// <summary>
        /// Start state
        /// </summary>
        private TNode _startState;
        
        /// <summary>
        /// End state
        /// </summary>
        private TNode _endState;
        
        /// <summary>
        /// Graph of states
        /// </summary>
        private readonly Dictionary<TNode, Func<TNode, TPayload, TNode>> _graph = new Dictionary<TNode, Func<TNode, TPayload, TNode>>();
        
        /// <summary>
        /// Delay
        /// </summary>
        private (bool _status, int _miliseconds) _delay = (false, 0);

        /// <summary>
        /// Exception handler
        /// </summary>
        private Func<TNode, TPayload, Exception, TNode> _exceptionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="Exception"></exception>
        public SimpleFsmBuilder() => Run(() =>
        {
            // Set default exception handler
            _exceptionHandler = (node, payload, _) => _endState;

            if (!typeof(TNode).IsEnum)
            {
                throw new Exception("`TNode` type should be an Enum type.");
            }
        });

        /// <summary>
        /// Sets the start state
        /// </summary>
        /// <returns></returns>
        public SimpleFsmBuilder<TNode, TPayload> SetStartState(TNode startState) => Run(() => _startState = startState, this);

        /// <summary>
        /// Sets the end state
        /// </summary>
        /// <returns></returns>
        public SimpleFsmBuilder<TNode, TPayload> SetEndState(TNode endState) => Run(() => _endState = endState, this);
        
        /// <summary>
        /// Add Edge handler
        /// </summary>
        /// <param name="node"></param>
        /// <param name="hanlder"></param>
        /// <returns></returns>
        public SimpleFsmBuilder<TNode, TPayload> AddHandler(TNode node, Func<TNode, TPayload, TNode> hanlder) =>
            Run(() => _graph[node] = hanlder, this);

        /// <summary>
        /// Add Edge handler
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public SimpleFsmBuilder<TNode, TPayload> AddTransition(TNode source, TNode destination) =>
            Run(() => _graph[source] = (x, _) => destination, this);

        /// <summary>
        /// Add delay between tasks
        /// </summary>
        /// <returns></returns>
        public SimpleFsmBuilder<TNode, TPayload> AddDelay(int miliseconds = 100) => Run(() =>
        {
            _delay._status = true;
            _delay._miliseconds = miliseconds;
        }, this);

        /// <summary>
        /// Sets the exception handler
        /// </summary>
        /// <param name="hanlder"></param>
        /// <returns></returns>
        public SimpleFsmBuilder<TNode, TPayload> SetExceptionHandler(Func<TNode, TPayload, Exception, TNode> hanlder) =>
            Run(() => _exceptionHandler = hanlder, this);

        /// <summary>
        /// Fill Handlers
        /// </summary>
        /// <returns></returns>
        private void FillHandlers() => Run(() =>
        {
            // Get all values
            var values = Enum.GetValues(typeof(TNode)).Cast<TNode>();

            _graph.Keys.Where(x => !values.Contains(x)).ForEach(x => _graph[x] = (node, payload) => _endState);
        }, this);

        /// <summary>
        /// Handles FSM
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="currentNode"></param>
        private void HandleFsm(TPayload payload, TNode currentNode) => Run(() =>
        {
            // Destination node
            TNode destinationNode;
            
            // Catch the exception if any
            try
            {
                // Run the handler
                destinationNode = _graph[currentNode](currentNode, payload);
            }
            catch (Exception e)
            {
                // Use the exception handler
                destinationNode = _exceptionHandler(currentNode, payload, e);
            }

            // If we have not reached the end
            if (!_endState.Equals(destinationNode))
            {
                // If needed add thread.sleep
                if (_delay._status)
                {
                    Thread.Sleep(_delay._miliseconds);
                }
                
                // Recursive
                HandleFsm(payload, destinationNode);
            }
        });

        /// <summary>
        /// Build the FSM
        /// </summary>
        /// <returns></returns>
        public SimpleFsm<TPayload> Build() =>
            Run(FillHandlers, new SimpleFsm<TPayload>(x => HandleFsm(x, _startState)));
    }
}