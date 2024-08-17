using Godot;
using System;

public partial class Change_Scene_Button : Button
{
    [Export]
    private string destination_Scene_Path;

    void _pressed(){
        PackedScene destination_Scene = ResourceLoader.Load<PackedScene>(destination_Scene_Path);
        GetTree().ChangeSceneToPacked(destination_Scene);
    }
}
