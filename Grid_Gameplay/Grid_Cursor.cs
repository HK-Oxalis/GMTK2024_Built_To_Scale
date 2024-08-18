using Godot;
using System;

public partial class Grid_Cursor : Node2D
{
    [Signal] public delegate void CellInteractedEventHandler(Vector2 cell);
    [Signal] public delegate void CursorMovedEventHandler(Vector2 new_Cell);
    [Export] State player_Phase_State;
    [Export] Grid grid;
    [Export] float ui_Cooldown = 0.1f;
    private Timer timer;

    private Vector2 _cell;
    public Vector2 cell {get => _cell; set => Set_Cell(value); }
    
    
    public override void _Ready()
    {
        timer = (Timer)this.FindChild("Timer");
        timer.WaitTime = ui_Cooldown;
        
        cell = grid.Calculate_Grid_Position(this.Position);
    }



    public override void _Input(InputEvent @event)
    {

        if(Game_Controller.Instance.current_state != player_Phase_State) return;
        //Check the cooldown on repeated events 
        //(if the event was there last time and the timer is not finished)
        
        if(@event.IsEcho() && !timer.IsStopped()){
            return;
        }
        if(@event is InputEventMouseMotion motion_Event){
            EmitSignal(SignalName.CursorMoved, grid.Calculate_Grid_Position(motion_Event.Position));
        }
        if(@event.IsActionPressed("Accept") || @event.IsActionPressed("ui_accept")){
            EmitSignal(SignalName.CellInteracted, cell);
        }
        if(@event.IsActionPressed("ui_up")) EmitSignal(SignalName.CursorMoved, this.cell + Vector2.Up);
        if(@event.IsActionPressed("ui_right")) EmitSignal(SignalName.CursorMoved, this.cell + Vector2.Right);
        if(@event.IsActionPressed("ui_down")) EmitSignal(SignalName.CursorMoved, this.cell + Vector2.Down);
        if(@event.IsActionPressed("ui_left")) EmitSignal(SignalName.CursorMoved, this.cell + Vector2.Left);
    }

    
    public void Set_Cell(Vector2 new_Cell){
        //new_Cell = grid.Grid_Clamp(new_Cell);


        if(new_Cell.IsEqualApprox(this.cell)){
            return;
        }

        _cell = new_Cell;
        this.Position = grid.Calculate_World_Position(new_Cell);
        timer.Start();
        
    }


}
