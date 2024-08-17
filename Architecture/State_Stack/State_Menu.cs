using Godot;
using System;

public partial class State_Menu : State
{
    [Export] private PackedScene pause_Scene;
    private Control pause_Node;
    public override void Enter(Node owner)
    {
        pause_Node = pause_Scene.Instantiate<Control>();

        Game_Controller.Instance.AddChild(pause_Node);
        Game_Controller.Instance.MoveChild(pause_Node, -1);
    }

    public override void Exit(){
        pause_Node.QueueFree();
    }

    public override void Execute(double delta)
    {
        if(!pause_Node.IsProcessing()){
            pause_Node.ProcessMode = Node.ProcessModeEnum.Always;
        }
    }

    public override void Pause_Execution()
    {
        pause_Node.ProcessMode = Node.ProcessModeEnum.Disabled;
    }


}
