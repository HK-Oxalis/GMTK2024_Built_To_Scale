using Godot;
using System;

public partial class Settings : Node
{
	public static Settings Instance { get; private set; }

	public bool[] palettisation_Settings = new bool[] { true, true, true};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}
}
