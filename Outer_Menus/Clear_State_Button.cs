using Godot;
using System;

public partial class Clear_State_Button : Button
{
    public override void _Pressed()
    {
        Game_Controller.Instance.Clear_Stack();
    }
}
