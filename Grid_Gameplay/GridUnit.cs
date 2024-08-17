using Godot;
using System;


//[Tool]
public partial class GridUnit : Path2D
{
    [Signal] public delegate void MoveFinishedEventHandler();

    [Export] Grid grid;
    [Export] PathFollow2D path_Follow;
    [Export] float animation_Speed = 300;

    public Vector2 cell {get; private set; }
    public bool is_Moving;

    public override void _Ready(){
        
        if(!Engine.IsEditorHint()){this.Curve = new Curve2D();}
        Vector2[] debug_Points = {new Vector2(0,2), new Vector2(2,2), new Vector2(2,5), new Vector2(8,5)};
        this.create_Curve(debug_Points);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (this.is_Moving){
            path_Follow.Progress += this.animation_Speed * (float)delta;

        if(path_Follow.ProgressRatio >= 1.0){
            this.is_Moving = false;

            path_Follow.Progress = 0;
            Position = grid.Calculate_World_Position(cell);
            this.Curve.ClearPoints();

            EmitSignal(SignalName.MoveFinished);
        }
        }
    }

    public void create_Curve(Vector2[] path_Points){
        if(path_Points.IsEmpty()) return;

        this.Curve.AddPoint(Vector2.Zero);

        foreach(Vector2 point in path_Points){
            this.Curve.AddPoint(grid.Calculate_World_Position(point) - this.Position);
        }

        this.cell = path_Points[path_Points.Length - 1];

        this.is_Moving = true;
    }
}
