using Godot;
using System;

public class TargetRune : SpellTarget
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	public bool mouseTrack = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
	 if(mouseTrack){
	 	GlobalPosition = GetGlobalMousePosition();
	 }
     if(this != SpellCaster.target){
		 QueueFree();
	 }
 }

	public override void _UnhandledInput(InputEvent inputEvent)
	{
		base._UnhandledInput(inputEvent);
	
		 if(inputEvent is InputEventMouseButton && ((InputEventMouseButton)inputEvent).Pressed){
			GD.Print("Click!");
			
		 	mouseTrack = false;
		 }
	 }
}
