using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public partial class Game_Board : Node2D
{   
    [Export] State movement_Phase_State;
    
    [Export] State player_Phase_State;
    [Export] Grid grid;
    [Export] Texture path_Mark_Texture;
    List<Sprite2D> path_Mark_Sprites = new List<Sprite2D>();

    [Export] GridUnit player;
    [Export] Grid_Cursor cursor;

    private GridUnit[] grid_Units;
    private int finished_Units = 0;
    List<Vector2> player_Path_Cells = new List<Vector2>();

    public override void _Ready()
    {
        cursor.Connect(Grid_Cursor.SignalName.CursorMoved, new Callable(this, MethodName.On_Cursor_Moved));
        cursor.Connect(Grid_Cursor.SignalName.CellInteracted, new Callable(this, MethodName.On_Cell_Interact));

        List<GridUnit> units = new List<GridUnit>();
        foreach(Node child in this.GetChildren()){
            if(child.GetType() == typeof(GridUnit)){
                units.Add((GridUnit)child);
                child.Connect(GridUnit.SignalName.MoveFinished, new Callable(this, MethodName.On_Unit_Finished_Move));
            }
        }
        grid_Units = units.ToArray();
        
        player_Path_Cells.Add(grid.Calculate_Grid_Position(player.Position));


    }

    public void On_Cursor_Moved(Vector2 new_Cell){
        //GD.Print("Cursor position is: " + new_Cell);

        Vector2 current_Cell = player_Path_Cells.Last();
        bool is_Orthogonal = (current_Cell.X == new_Cell.X) || (current_Cell.Y == new_Cell.Y);
        bool is_Adjacent = (Math.Abs(current_Cell.X - new_Cell.X) <= 1) && (Math.Abs(current_Cell.Y - new_Cell.Y) <= 1);

        bool valid_Cell = player.moved_Cells < player.move_Range && is_Orthogonal && is_Adjacent;
        
        //If the cell isn't a previous cell or within movement range and orthogonal, return
        if(!(valid_Cell || player_Path_Cells.Contains(new_Cell))){ return;}
        
        //If the cell is already on the path, remove all cells after it on the path
        if(player_Path_Cells.Contains(new_Cell)){
            int index = player_Path_Cells.IndexOf(new_Cell);
            int removed_Cells = player_Path_Cells.Count - index;
            
            player_Path_Cells.RemoveRange(index, removed_Cells);
            Update_Marks();
            player.moved_Cells -= removed_Cells;
        }
        if(player.moved_Cells < player.move_Range){
            Add_Path_Entry(new_Cell);
        }
        
        cursor.cell = new_Cell;
    }

    public void On_Cell_Interact(Vector2 cell){

        //If a valid cell has been selected, move the player to it
        if((player_Path_Cells.Count != 0) && player_Path_Cells.Contains(cell)){
            Begin_Movement_Phase();
        }

    }

    private void Begin_Movement_Phase(){
        Vector2 destination_Cell = player_Path_Cells.Last();

        player.create_Curve(player_Path_Cells);
        player_Path_Cells.Clear();

        player_Path_Cells.Add(destination_Cell);  

        //Remove records of old path
        foreach(Sprite2D sprite in path_Mark_Sprites){
            sprite.QueueFree();
        }
        path_Mark_Sprites.Clear();

        Game_Controller.Instance.Change_State(movement_Phase_State);
    }

    public void Add_Path_Entry(Vector2 new_Cell){

        if(player_Path_Cells.Count != 0){
            Sprite2D new_Mark = new Sprite2D();
            this.AddChild(new_Mark);
            this.MoveChild(new_Mark, 0);

            new_Mark.Texture = (Texture2D)path_Mark_Texture;
            new_Mark.Position = grid.Calculate_World_Position(player_Path_Cells.Last());
        
            path_Mark_Sprites.Add(new_Mark);
        }
        
        
        player_Path_Cells.Add(new_Cell);
        player.moved_Cells ++;
    }

    private void Update_Marks(){

        for(int i = path_Mark_Sprites.Count - 1; i >= 0; i--){
            Sprite2D sprite = path_Mark_Sprites[i];
            bool is_In_Path = false;
            foreach(Vector2 cell in player_Path_Cells){
                Vector2 cell_World_Position = grid.Calculate_World_Position(cell);
                is_In_Path = is_In_Path || sprite.Position.IsEqualApprox(cell_World_Position);
            }
            if(!is_In_Path){
                path_Mark_Sprites.Remove(sprite);
                sprite.QueueFree();
            }
        }

    }

    private void On_Unit_Finished_Move(){
        finished_Units ++;

        if(finished_Units >= grid_Units.Length){
            finished_Units = 0;
            Game_Controller.Instance.Change_State(player_Phase_State);
        }  
    }

    
}
