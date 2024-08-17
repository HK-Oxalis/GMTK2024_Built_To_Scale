using Godot;
using System;

public partial class Add_State_Button : Button
{
    [Export] private State state;

    public override void _Pressed()
    {
        Game_Controller.Instance.Push_State(state);
    }
}
