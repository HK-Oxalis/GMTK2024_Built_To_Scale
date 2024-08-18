using Godot;
using System;
using System.Linq;
using System.Collections.Generic;


public partial class Composite_GridUnit : GridUnit
{
    public enum Direction{
        Up,
        Down,
        Left,
        Right,
    }
    [Export] TileMapLayer tile_Map;
    [Export] Sprite2D[] sprites;
    [Export]  Godot.Collections.Array<Direction> directions_Input;
    Vector2[] directions;

    public override void _Ready()
    {
        base._Ready();

        List<Sprite2D> sprites_List = new List<Sprite2D>();

        foreach(Node child in this.GetChildren()){
            if(child is Sprite2D){
                sprites_List.Add((Sprite2D)child);
            }
        }

        sprites = sprites_List.ToArray();

        

        directions = new Vector2[directions_Input.Count];
        for(int i = 0; i < directions_Input.Count; i++){
            int direction = (int)directions_Input[i];


            if(direction == 0)directions[i] = Vector2.Up;
            if(direction == 1)directions[i] = Vector2.Down;
            if(direction == 2)directions[i] = Vector2.Left;
            if(direction == 3)directions[i] = Vector2.Right;
        }
        
        Update_Collision(-1);

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }



    public void Update_Collision(int turn){
        List<Vector2> sprite_Cells = new List<Vector2>();

  
        foreach (Vector2I cell in tile_Map.GetUsedCells()){
            Vector2 tile_Position = tile_Map.MapToLocal(cell);
            tile_Position = ToGlobal(tile_Position);
            sprite_Cells.Add(base.grid.Calculate_Grid_Position(tile_Position));
        }

        base.cell = sprite_Cells.ToArray();

        Update_Arrows(turn);

    }

    public Vector3 Get_Next_Position(int turn){
        Vector2 current_Position = base.grid.Calculate_Grid_Position(this.Position);
        if(turn >= directions.Length) return new Vector3(current_Position.X, current_Position.Y, 0);

        Vector2 next_Position = current_Position + directions[turn];
        return new Vector3(next_Position.X, next_Position.Y, 1);
    }

    private void Update_Arrows(int turn){
        if(turn + 2 >= directions.Length) return;
        Direction direction = directions_Input[turn + 1];

        foreach(Sprite2D sprite in this.sprites){
            if(direction == Direction.Up){ sprite.RotationDegrees = 0;}
            else if(direction == Direction.Down){ sprite.RotationDegrees = 180;}
            else if(direction == Direction.Left){ sprite.RotationDegrees = 270;}
            else if(direction == Direction.Right){ sprite.RotationDegrees = 90;}
        }

        

    }
}
