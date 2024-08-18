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

    [Export]GridUnit player;
    [Export] Grid_Cursor cursor;

    public int turn_Count { get; private set; } = 0;

    private GridUnit[] grid_Units;
    private List<GridUnit> obstacle_Units = new List<GridUnit>();
    private List<Composite_GridUnit> movement_Queue = new List<Composite_GridUnit>();
    private bool is_Player_Moving = false;
    List<Vector2> player_Path_Cells = new List<Vector2>();

    public override void _Ready()
    {

        cursor.Connect(Grid_Cursor.SignalName.CursorMoved, new Callable(this, MethodName.On_Cursor_Moved));
        cursor.Connect(Grid_Cursor.SignalName.CellInteracted, new Callable(this, MethodName.On_Cell_Interact));

        List<GridUnit> units = new List<GridUnit>();
        foreach(Node child in this.GetChildren()){
            if(child.GetType() == typeof(GridUnit) || child.GetType().IsSubclassOf(typeof(GridUnit))){
                units.Add((GridUnit)child);
                child.Connect(GridUnit.SignalName.MoveFinished, new Callable(this, MethodName.On_Unit_Finished_Move));
            }
        }
        grid_Units = units.ToArray();


        obstacle_Units.AddRange(grid_Units);
        obstacle_Units.Remove(player);


        player_Path_Cells.Add(grid.Calculate_Grid_Position(player.Position));


    }

    public void On_Cursor_Moved(Vector2 new_Cell){
        //GD.Print("Cursor position is: " + new_Cell);


        
        //If the cell isn't a previous cell or within movement range and orthogonal, return
        if(!Is_Valid_Cell(new_Cell)){ return;}
        
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

    private bool Is_Valid_Cell(Vector2 new_Cell){

        Vector2 current_Cell = player_Path_Cells.Last();
        
        bool is_Orthogonal = (current_Cell.X == new_Cell.X) || (current_Cell.Y == new_Cell.Y);
        bool is_Adjacent = (Math.Abs(current_Cell.X - new_Cell.X) <= 1) && (Math.Abs(current_Cell.Y - new_Cell.Y) <= 1);

        //Is the movement to an orthogonally adjacent cell within the move range
        bool valid_Movement = player.moved_Cells < player.move_Range && is_Orthogonal && is_Adjacent;


        //If it's a valid movement or it's a previous movement
        bool valid_Path = valid_Movement || player_Path_Cells.Contains(new_Cell);
        //If the cell isn't occupied
        GridUnit cell_Occupant = Is_Cell_Occupied(new_Cell);
        bool valid_Cell = (cell_Occupant == null) || cell_Occupant == player;

        //Only if these are both true
        return valid_Path && valid_Cell;
    }

    public GridUnit Is_Cell_Occupied(Vector2 cell){
        foreach (GridUnit unit in grid_Units) {
            foreach(Vector2 unit_Cell in unit.cell){
                if(unit_Cell.IsEqualApprox(cell)) {
                    return unit;}

            }
           
        }
        return null;
    }

    public void On_Cell_Interact(Vector2 cell){

        //If a valid cell has been selected, move the player to it
        if((player_Path_Cells.Count != 0) && player_Path_Cells.Contains(cell)){
            Begin_Movement_Phase();
        }

    }

    private void Begin_Movement_Phase(){
        
        foreach (GridUnit unit in obstacle_Units){
            Composite_GridUnit obstacle = (Composite_GridUnit)unit;
            bool should_Move = obstacle.Get_Next_Position(turn_Count).Z == 1;

            if(should_Move){ movement_Queue.Add(obstacle);}

        }

        if(movement_Queue.Count > 0){
            Vector2 next_Position = new Vector2(movement_Queue[0].Get_Next_Position(turn_Count).X, movement_Queue[0].Get_Next_Position(turn_Count).Y);
            movement_Queue[0].create_Curve(new List<Vector2>{next_Position});
            }
        else Move_Player();
         
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

        if(is_Player_Moving){
            is_Player_Moving = false;
            
            //If it was the player just finishing movement, the turn is over

            GD.Print("Player moved, turn over");
            foreach(Sprite2D sprite in path_Mark_Sprites){
                sprite.QueueFree();
            }
            path_Mark_Sprites.Clear();

            //This should be empty, but just in case
            movement_Queue.Clear();

            turn_Count ++;
            Game_Controller.Instance.Change_State(player_Phase_State);

        }

        //If theres at least something left in the movement queue, update its collisions
        if(movement_Queue.Count >= 1){
            Composite_GridUnit finished_Obstacle = movement_Queue[0];
            finished_Obstacle.Update_Collision(turn_Count);
            movement_Queue.Remove(finished_Obstacle);
        }

        //If the movement queue is now empty
        if(movement_Queue.Count <= 0 && !is_Player_Moving){

            //Move player
            Move_Player();

        }
        //There's still stuff ledt in the queue, move it
        else{ 
            Composite_GridUnit obstacle = movement_Queue[0];
            Vector2 next_Position =   new Vector2(obstacle.Get_Next_Position(turn_Count).X, obstacle.Get_Next_Position(turn_Count).Y);
            obstacle.create_Curve(new List<Vector2>{next_Position});
        }

        
         
    }

    private void Move_Player(){
        GD.Print("Moving player");

            is_Player_Moving = true;
            Vector2 destination_Cell = player_Path_Cells.Last();

            player.create_Curve(player_Path_Cells);
            player_Path_Cells.Clear();

            player_Path_Cells.Add(destination_Cell);
    }

    
}
