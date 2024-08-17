using Godot;
using System;

public partial class Grid_Cursor : Node2D
{
    [Signal] public delegate void CellInteractedEventHandler(Vector2 cell);
    [Signal] public delegate void CursorMovedEventHandler(Vector2 new_Cell);
    [Export] Grid grid;
    [Export] float ui_Cooldown = 0.1f;
    private Timer timer;
    public Vector2 cell {get => cell; set => Set_Cell(value); }
    
    
    public override void _Ready()
    {
        GD.Print("cursor ready");
        timer = (Timer)this.FindChild("Timer");
        timer.WaitTime = ui_Cooldown;
        
    }



    public override void _Input(InputEvent @event)
    {
        //Check the cooldown on repeated events 
        //(if the event was there last time and the timer is not finished)
        if(@event.IsEcho() && !timer.IsStopped()){
            return;
        }
        if(@event is InputEventMouseMotion motion_Event){
    
            cell = grid.Calculate_Grid_Position(motion_Event.Position);
        }
        if(@event.IsActionPressed("Accept") || @event.IsActionPressed("ui_accept")){
            EmitSignal(SignalName.CellInteracted, cell);
        }
        if(@event.IsActionPressed("ui_up")) this.cell += Vector2.Up;
        if(@event.IsActionPressed("ui_right")) this.cell += Vector2.Right;
        if(@event.IsActionPressed("ui_down")) this.cell += Vector2.Down;
        if(@event.IsActionPressed("ui_left")) this.cell += Vector2.Left;
    }

    
    public void Set_Cell(Vector2 new_Cell){
        new_Cell = grid.Grid_Clamp(new_Cell);

        if(new_Cell.IsEqualApprox(this.cell)){
            return;
        }

        cell = new_Cell;
        this.Position = grid.Calculate_World_Position(cell);

        timer.Start();
    }


}
