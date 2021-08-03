using Godot;
using System;

public class Follower : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

	Node2D player = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     if(player == null){
		 player = GetTree().Root.FindNode("Player",true,false) as Node2D;
	 }
	if(player != null && ((Node2D)GetParent()).GlobalPosition.DistanceTo(player.GlobalPosition) > 16){
		((Node2D)this.GetParent()).Translate((((Node2D)GetParent()).GlobalPosition - player.GlobalPosition).Normalized() * delta * -50f);
	}
 }
}
