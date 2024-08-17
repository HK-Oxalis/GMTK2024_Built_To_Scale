using Godot;
using System;
using System.Linq;

public partial class Control_Remapping : VBoxContainer
{
    [Export] PackedScene button_Scene;

    public override void _Ready()
    {
        //Skips the 77 built-in inputs, which appear before game-specific inputs
        //This changes on editor updates, so be aware
        foreach(string action in InputMap.GetActions().Skip(77)){

            var scene_Instance = button_Scene.Instantiate();
            AddChild(scene_Instance);

            string button_Path = "PanelContainer/MarginContainer/HBoxContainer/Remap_Button";
            Remap_Button button_Instance = scene_Instance.GetNode<Remap_Button>(button_Path);
            button_Instance.action = action;

            string label_Path = "PanelContainer/MarginContainer/HBoxContainer/Label";
            button_Instance.label = scene_Instance.GetNode<Label>(label_Path);

            button_Instance.Initialise();
        }
    }
}
