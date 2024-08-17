using Godot;
using System;

public partial class Fullscreen_Box : CheckBox
{
    void _ready(){
        this.ButtonPressed= (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen);
    }
    void _pressed(){
        if(this.ButtonPressed){
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
        }
        else{ DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);}
    }
}
