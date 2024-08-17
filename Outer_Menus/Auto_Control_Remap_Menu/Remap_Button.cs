using Godot;
using System;
using System.Security.Cryptography;

public partial class Remap_Button : Button
{
    [Export]
    public string action;

    public Label label;

    public void Initialise()
    {
        SetProcessUnhandledInput(false);
        Update_Key_Text();
        this.label.Text = action;
    }
    public override void _Toggled(bool toggledOn)
    {
        SetProcessUnhandledInput(toggledOn);
        if (toggledOn){
            this.Text = "Awaiting Input";
            ReleaseFocus();
        }
        else{
            Update_Key_Text();
            GrabFocus();
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event.IsPressed()){
            InputMap.ActionEraseEvents(action);
            InputMap.ActionAddEvent(action, @event);
            this.ButtonPressed = false;
        }
    }

    void Update_Key_Text(){
        //This output includes the name of the key, but also some uneccesary information
        string key_Text = InputMap.ActionGetEvents(action).ToString();

        if (key_Text.Contains("Joypad"))
        {
            //The button/axis number is located after the equals sign
            int button_Start_Index = key_Text.IndexOf("=") + 1;
            //There's a small chance of a double-digit number, so we've got to check for the comma
            int button_End_Index = key_Text.IndexOf(",") ;

            string key_Number = key_Text.Substring(button_Start_Index, button_End_Index - button_Start_Index);

            //Call it an axis if it's labelled as one, a button if not
            if (key_Text.Contains("axis")){key_Text = "Axis " + key_Number;}
            else{key_Text = "Button " + key_Number;}
        }
        else{
            //The key name is inbetween two brackets, so we can find it that way
            //Adding one so we don't include the opening bracket 
            int key_Start_Index = key_Text.IndexOf("(") + 1;
            int key_End_Index = key_Text.IndexOf(")") ;
        
            //Get the substring starting at the first bracket 
            //and the length of the difference between the two brackets
            key_Text = key_Text.Substring(key_Start_Index, key_End_Index - key_Start_Index);
        }
        
        
        this.Text = key_Text;
        
    }
}
