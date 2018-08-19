using System;
using SimpleFSM.Builders;

namespace SimpleFSM.Interfaces
{
    public interface ISimpleFsmBuilder<TNode, TPayload>
        where TNode: struct, IConvertible
        where TPayload: class
    {
        SimpleFsmBuilder<TNode, TPayload> SetStartState(TNode startState);

        SimpleFsmBuilder<TNode, TPayload> SetEndState(TNode endState);

        SimpleFsmBuilder<TNode, TPayload> AddHandler(TNode node, Func<TNode, TPayload, TNode> hanlder);

        SimpleFsmBuilder<TNode, TPayload> AddTransition(TNode source, TNode destination);
        
        SimpleFsmBuilder<TNode, TPayload> AddDelay(int miliseconds = 100);
        
        SimpleFsmBuilder<TNode, TPayload> SetExceptionHandler(Func<TNode, TPayload, Exception, TNode> hanlder);
        
        SimpleFsm<TPayload> Build();
    }
}