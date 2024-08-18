using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game_Board : Node2D
{
    [Export] Grid grid;
    [Export] Texture path_Mark_Texture;
    List<Sprite2D> path_Mark_Sprites = new List<Sprite2D>();

    [Export] GridUnit player;
    [Export] Grid_Cursor cursor;

    List<Vector2> player_Path_Cells = new List<Vector2>();

    public override void _Ready()
    {
        cursor.Connect(Grid_Cursor.SignalName.CursorMoved, new Callable(this, MethodName.On_Cursor_Moved));
        cursor.Connect(Grid_Cursor.SignalName.CellInteracted, new Callable(this, MethodName.On_Cell_Interact));

        
    }

    public void On_Cursor_Moved(Vector2 new_Cell){
        GD.Print("Cursor position is: " + new_Cell);
        //If the cell isn't a previous cell or within movement range, return
        if(!(player.moved_Cells < player.move_Range || player_Path_Cells.Contains(new_Cell))){GD.Print("Invalid move"); return;}
        
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

        //If a valid cell has been selected, move the player to is
        if((player_Path_Cells.Count != 0) && player_Path_Cells.Contains(cell)){
            player.create_Curve(player_Path_Cells);
            player_Path_Cells.Clear();

            //Remove records of old path
            foreach(Sprite2D sprite in path_Mark_Sprites){
                sprite.QueueFree();
            }
            path_Mark_Sprites.Clear();
        }

    }

    public void Add_Path_Entry(Vector2 new_Cell){

        if(player_Path_Cells.Count != 0){
            Sprite2D new_Mark = new Sprite2D();
            this.AddChild(new_Mark);

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

        GD.Print(path_Mark_Sprites.Count);
        GD.Print(player_Path_Cells.Count);
    }

    
}
