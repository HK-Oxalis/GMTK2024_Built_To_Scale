using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


//[Tool]
public partial class GridUnit : Path2D
{
    [Signal] public delegate void MoveFinishedEventHandler();

    [Export] protected Grid grid;
    [Export] PathFollow2D path_Follow;

    [Export] float animation_Speed = 300;
    [Export] public int move_Range = 5;
    public int moved_Cells = 0;

    public Vector2[] cell {get; protected set; }
    public bool is_Moving;

    public override void _Ready(){
        
        if(!Engine.IsEditorHint()){this.Curve = new Curve2D();}

        cell = new Vector2[1];
        cell[0] = grid.Calculate_Grid_Position(this.Position);
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (this.is_Moving){
            path_Follow.Progress += this.animation_Speed * (float)delta;

        if(path_Follow.ProgressRatio >= 1){
            this.is_Moving = false;

            
            Position = grid.Calculate_World_Position(cell[0]);
            this.Curve.ClearPoints();
            path_Follow.Progress = 0;
            path_Follow.Position = Vector2.Zero;

            EmitSignal(SignalName.MoveFinished);
        }
        }
    }

    public void create_Curve(List<Vector2> path_Points){
        if(path_Points.Count == 0) return;

        this.Curve.AddPoint(Vector2.Zero);

        foreach(Vector2 point in path_Points){
            this.Curve.AddPoint(grid.Calculate_World_Position(point) - this.Position);
        }

        
        this.cell[0] = path_Points.Last();

        this.moved_Cells = 0;
        this.is_Moving = true;
    }

}
