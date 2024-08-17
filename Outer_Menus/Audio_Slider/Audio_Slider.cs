using Godot;
using System;


public partial class Audio_Slider : HSlider
{
    
    
    public string bus_Name = "Master";
    [Export]
    private AudioStreamPlayer sample_Player;

    private int bus;
    private bool ready_To_Play = false;
    

    
    ///Putting this in a custom init so it activates after parent has applied the editor settings
    public void Initialise(){

        bus = AudioServer.GetBusIndex(bus_Name);
        float current_Volume = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(bus));

        if(SettingsManager.Instance.audio_First_Boot){
            this.MaxValue = current_Volume * 1.5;
        }

        this.Value = current_Volume;

        
    }

    async void _value_changed(float value){
        if(SettingsManager.Instance.audio_First_Boot){SettingsManager.Instance.CallDeferred("update_Audio");}

        AudioServer.SetBusVolumeDb(bus, Mathf.LinearToDb(value));

        

        if(ready_To_Play){
            ready_To_Play = false;
            sample_Player.Play();

            await ToSignal(GetTree().CreateTimer(0.2), SceneTreeTimer.SignalName.Timeout);
            
        }
        ready_To_Play = true;


        
    }
}
