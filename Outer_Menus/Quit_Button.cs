using Godot;
using System;

public partial class Quit_Button : Button
{
    public override void _Pressed()
    {
        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
        GetTree().Quit(0);
    }
}
