using System;
using SimpleFSM.Builders;
using Xunit;

namespace SimpleFSM.Test
{
    public class SimpleFsmTest
    {
        [Fact]
        public void Test__BasicSync()
        {
            // Arrange
            var paylod = new TestPayload();
            var fsm = SimpleFsmBuilder<TestStates, TestPayload>.New()
                .SetStartState(TestStates.Start)
                .SetEndState(TestStates.End)
                .AddTransition(TestStates.Start, TestStates.Step1)
                .AddHandler(TestStates.Step1, (currentState, payload) =>
                {
                    payload.Value = "Test1";
                    return TestStates.Step2;
                })
                .AddHandler(TestStates.Step2, (currentState, payload) =>
                {
                    payload.Value = "Test2";
                    return TestStates.Step3;
                }).AddHandler(TestStates.Step3, (currentState, payload) =>
                {
                    payload.Value = "Test3";
                    return TestStates.End;
                })
                .Build();
            
            // Act
            fsm.Start(paylod);
            
            // Assert
            Assert.Equal("Test3", paylod.Value);
        }
        
        [Fact]
        public async void Test__BasicAsync()
        {
            // Arrange
            var paylod = new TestPayload();
            var fsm = SimpleFsmBuilder<TestStates, TestPayload>.New()
                .AddDelay(50)
                .SetStartState(TestStates.Start)
                .SetEndState(TestStates.End)
                .AddTransition(TestStates.Start, TestStates.Step1)
                .AddHandler(TestStates.Step1, (currentState, payload) =>
                {
                    payload.Value = "Test1";
                    return TestStates.Step2;
                })
                .AddHandler(TestStates.Step2, (currentState, payload) =>
                {
                    payload.Value = "Test2";
                    return TestStates.Step3;
                }).AddHandler(TestStates.Step3, (currentState, payload) =>
                {
                    payload.Value = "Test3";
                    return TestStates.End;
                })
                .Build();
            
            // Act
            await fsm.StartAsync(paylod);
            
            // Assert
            Assert.Equal("Test3", paylod.Value);
        }

        [Fact]
        public void Test__ExceptionHandler()
        {
            // Arrange
            var paylod = new TestPayload();
            var expectedException = new Exception();

            // Act, Assert
            SimpleFsmBuilder<TestStates, TestPayload>.New()
                .SetStartState(TestStates.Start)
                .SetEndState(TestStates.End)
                .AddTransition(TestStates.Start, TestStates.Step1)
                .AddHandler(TestStates.Step1, (currentState, payload) =>
                {
                    payload.Value = "Test1";

                    // Throw some exception
                    throw expectedException;
                })
                .SetExceptionHandler((currentState, payload, exception) =>
                {
                    // Validate the exception
                    Assert.Equal(expectedException, exception);

                    return TestStates.End;
                })
                .Build()
                .Start(paylod);
        }
        
        [Fact]
        public async void Test__CycleStop()
        {
            // Arrange
            var count = 0;

            // Act, Assert
            await SimpleFsmBuilder<TestStates, TestPayload>.New()
                .SetStartState(TestStates.Start)
                .SetEndState(TestStates.End)
                .AddTransition(TestStates.Start, TestStates.Step1)
                .AddHandler(TestStates.Step1, (currentState, payload) => TestStates.Step2)
                .AddTransition(TestStates.Step2, TestStates.Step3)
                .AddHandler(TestStates.Step3, (x, _) => count++ != 10 ? TestStates.Step1 : TestStates.End)
                .Build()
                .StartAsync(null);
        }
    }
}