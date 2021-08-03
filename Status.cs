using Godot;
using System;

public class Status : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	public static Status current;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        current = this;
    }

	public static void Log(string text){
		if(current != null){
			current.Text = text+"\n"+current.Text;
		}
	}
//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
