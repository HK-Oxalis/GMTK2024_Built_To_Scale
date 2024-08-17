using Godot;
using System;

public partial class Audio_Settings_Item : Control
{
    private Audio_Slider slider;
    [Export] string bus_Name = "Master";
    AudioStreamPlayer sound_Player;
    [Export] AudioStream sound_Preview = null;

    Label label;

    void _ready(){
        slider = this.GetChild<Audio_Slider>(1);
        sound_Player = this.GetChild<AudioStreamPlayer>(2);
        label = this.GetChild<Label>(0);

        slider.bus_Name = bus_Name;

        sound_Player.Bus = bus_Name;
        if(sound_Preview != null){sound_Player.Stream = sound_Preview;}

        label.Text = bus_Name;

        //Custom Init called after editor settings have been applied
        slider.Initialise();

    }
}
