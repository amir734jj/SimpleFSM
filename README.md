## SimpleFSM

This FSM uses Enum to keep track of states and Functions to handler the transitions from one state to another.

[Nuget](https://www.nuget.org/packages/SimpleFSM/)

This Enum keeps track of the state:
```csharp
public enum TestStates
{
    Start,
    Step1,
    Step2,
    Step3,
    End
}
```

Payload class:

```csharp
public class TestPayload
{
    public string Value { get; set; }
}
```

Building the FSM using the builder:

```csharp
var paylod = new TestPayload();

var fsm = SimpleFsmBuilder<TestStates, TestPayload>.New()
    // Set start state
    .SetStartState(TestStates.Start)
    // Set end state
    .SetEndState(TestStates.End)
    // Add un-conditional transition from Start to Step1
    .AddTransition(TestStates.Start, TestStates.Step1)

    // Add Handler from step1 -> Step2
    .AddHandler(TestStates.Step1, (currentState, payload) =>
    {
        payload.Value = "Test1";
        // Destination state
        return TestStates.Step2;
    })
    // Add Handler from Step2 -> Step3
    .AddHandler(TestStates.Step2, (currentState, payload) =>
    {
        payload.Value = "Test2";
        // Destination state
        return TestStates.Step3;
    })
    // Add Handler from Step3 -> End
    .AddHandler(TestStates.Step3, (currentState, payload) =>
    {
        payload.Value = "Test3";
        // Destination state
        return TestStates.End;
    })
    // Add Exception handler
    .SetExceptionHandler((currentState, payload, exception) =>
    {
        // Validate the exception
        Assert.Equal(expectedException, exception);
        // Stop
        return TestStates.End;
    })    
    .Build();

// Run the machine given the payload
fsm.Start(paylod);
// or fsm.StartAsync(paylod)
```
