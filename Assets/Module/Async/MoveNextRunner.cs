﻿using System.Runtime.CompilerServices;

internal class MoveNextRunner<TStateMachine> where TStateMachine : IAsyncStateMachine
{
    public TStateMachine StateMachine;

    //[DebuggerHidden]
    public void Run()
    {
        StateMachine.MoveNext();
    }
}