using Godot;
using System;

public partial class State : Resource
{
    public virtual void Enter(Node owner){}
    public virtual void Execute(double delta){}
    public virtual void Pause_Execution(){}
    public virtual void Exit(){}
}
