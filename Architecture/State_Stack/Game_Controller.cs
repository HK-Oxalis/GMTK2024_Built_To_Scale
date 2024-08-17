using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game_Controller : Node
{
	public static Game_Controller Instance { get; private set;}
	public Stack<State> state_Stack = new Stack<State>();
	public State current_state;

	private State none_State = GD.Load<State>("res://Architecture/State_Stack/State_None.tres");
	private State start_State  =  GD.Load<State>("res://Architecture/State_Stack/State_None.tres");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		Push_State(none_State);
		Push_State(start_State);
	}

    public override void _Process(double delta)
    {
        current_state?.Execute(delta);
    }

	public void Push_State(State state){
		//So we don't stick duplicates of the same state on top of each other
		if(current_state == state) return;
		current_state?.Pause_Execution();

		state_Stack.Push(state);
		current_state = state_Stack.Peek();
		current_state.Enter(this);

		
	}

	public void Pop_State(){
		state_Stack.Pop();
		current_state.Exit();
		current_state = state_Stack.Peek();
	}


	public void Change_State(State newstate){
		if(current_state != none_State){
			state_Stack.Pop();
			current_state.Exit();
		}
		state_Stack.Push(newstate);
		current_state = newstate;
		current_state.Enter(this);
	}

	public State Get_Previous_State(){
		return state_Stack.ElementAt(1);
	}

	public void Clear_Stack(){
		int states_Count = state_Stack.Count;
		for(int i = 1; i < states_Count; i++){
			this.Pop_State();
		}
		this.Push_State(none_State);
	}

}



