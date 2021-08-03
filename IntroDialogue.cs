using Godot;
using System;

public class IntroDialogue : VBoxContainer
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	public int count = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (Node item in this.GetChildren())
		{
			if(item is Control){
				((Control)item).Visible = false;
				((Control)item).Modulate = new Color(1f,1f,1f,0f);
			}
		}
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     for(int i = 0; i<this.GetChildCount();i++){
		 if(i < count){
			 ((Control)GetChild(i)).Visible = true;
			 ((Control)GetChild(i)).Modulate = new Color(1f,1f,1f,Math.Min(1f,((Control)GetChild(i)).Modulate.a+delta));
		 }
	 }
 }

	public override void _UnhandledInput(InputEvent @event)
	{
		GD.Print("UnhandledInput");
		if(@event is InputEventMouseButton && ((InputEventMouseButton)@event).Pressed){
			count++;
			GD.Print("count now"+count.ToString());
		}
		base._UnhandledInput(@event);
	}
}
