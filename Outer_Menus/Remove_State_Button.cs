using Godot;
using System;

public partial class Remove_State_Button : Button
{
    [Export] int layers = 1;

    public override void _Pressed()
    {
        Game_Controller.Instance.Pop_State();
    }
}
