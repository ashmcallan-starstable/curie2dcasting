using Godot;
using System;
using System.Collections.Generic;

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

	public void OnPlayerInventoryChanged() {
		Dictionary<string, int> inventory = Player.current.Inventory;
		string tx = "";
		foreach (KeyValuePair<string, int> item in inventory)
		{
			tx += $"{item.Key}:\t{item.Value}";
		}
		Text = tx;
	}
}
