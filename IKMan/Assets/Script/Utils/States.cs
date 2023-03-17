using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class States
{

    private Func<Event, (States, ActionStateMachine)> handleEventFunc;
    
    public string name;

    public States(string name, Func<Event, (States, ActionStateMachine)> handleEventFunc)
    {
        this.handleEventFunc = handleEventFunc;
        this.name = name;
    }

    public (States, ActionStateMachine) handleEvent(Event e)
    {
        return handleEventFunc(e);
    }

}
