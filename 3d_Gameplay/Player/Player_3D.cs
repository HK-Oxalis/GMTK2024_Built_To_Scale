using Godot;
using System;

public partial class Player_3D : CharacterBody3D
{
	[Export]
	public float Speed = 5.0f;
	[Export]
	public float JumpVelocity = 4.5f;

	[Export] State pause_State;
	[Export] State playing_State;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _PhysicsProcess(double delta)
	{
		if(Game_Controller.Instance.current_state != playing_State){return;}

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor()){		
			velocity.Y -= gravity * (float)delta;
		}

		// Handle Jump.
		//if (Input.IsActionJustPressed("Jump") && IsOnFloor())
			//velocity.Y = JumpVelocity;

		if (Input.IsActionJustPressed("Pause")){
			Game_Controller.Instance.Push_State(pause_State);
			
		}
		/*
		// Get the input direction and handle the movement/deceleration.
		Vector2 inputDir = Input.GetVector("Move Left", "Move Right", "Move Forward", "Move Back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		*/

		MoveAndSlide();
	}

	
}
