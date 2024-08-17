using Godot;
using System;

public partial class SettingsManager : Node
{
    public static SettingsManager Instance { get; private set;}

    public bool audio_First_Boot = true;

    public override void _Ready()
    {
        Instance = this;
    }

    public void update_Audio(){
        audio_First_Boot = false;
    }
}
