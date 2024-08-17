using Godot;
using System;

public partial class Options_Back_Button : MarginContainer
{
    [Export] State options_State;
    [Export] Button main_Menu_Button;
    [Export] Button back_Button;
    public override void _EnterTree()
    {
        base._EnterTree();

        Button relavent_Button = main_Menu_Button;
        if(Game_Controller.Instance.current_state == options_State){
            relavent_Button = back_Button;
        }
        
        relavent_Button.Show();
    }
}
