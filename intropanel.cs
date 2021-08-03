using Godot;
using System;

public class intropanel : Control
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

	public override void _Input(InputEvent @event)
	{
		GD.Print("UnhandledInput");
		if(@event is InputEventMouseButton && ((InputEventMouseButton)@event).Pressed){
			//count++;
			((IntroDialogue)FindNode("VBoxContainer",true,false)).count++;
			GD.Print("count now");
		}
		base._UnhandledInput(@event);
	}
//  }
}
