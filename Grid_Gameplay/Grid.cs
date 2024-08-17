using Godot;
using System;
using System.Reflection.Metadata;

public partial class Grid : Resource
{
    [Export] Vector2 grid_Size = new Vector2(20, 20);
    [Export] Vector2 cell_Size = new Vector2(80, 80);

    
    //Return pixel position from grid coordinates
    public Vector2 Calculate_World_Position(Vector2 grid_Position){
        return grid_Position * cell_Size + (cell_Size /2);
    }

    public Vector2 Calculate_Grid_Position(Vector2 world_Position){
        return (world_Position/cell_Size).Floor(); 
    }

    public bool Is_Within_Bounds(Vector2 cell_Coords){
        bool is_Horizontal_Bounds = cell_Coords.X < grid_Size.X && cell_Coords.X >= 0;
        bool is_Vertical_Bounds = cell_Coords.Y < grid_Size.Y && cell_Coords.Y >=0;
        return is_Horizontal_Bounds && is_Vertical_Bounds;
    }

    public Vector2 Grid_Clamp(Vector2 grid_Position){
        Vector2 clamped_Position;
        clamped_Position.X = Math.Clamp(grid_Position.X, 0, grid_Size.X - 1);
        clamped_Position.Y = Math.Clamp(grid_Position.Y, 0, grid_Size.Y - 1);
        return clamped_Position;
    }

    public int Grid_Index(Vector2 cell){
        return (int)(cell.X + grid_Size.X * cell_Size.Y);
    }
    
}
