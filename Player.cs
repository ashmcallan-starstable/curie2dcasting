using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	public WorldObject sprite;
	AnimationPlayer animationPlayer;

	protected Dictionary<string, int> inventory; 

	[Signal] public delegate void InventoryChanged();

	public void AddToInventory(string item) {
		if(inventory.ContainsKey(item)) {
			inventory[item] ++;
		} else {
			inventory[item] = 1;
		}
		EmitSignal(nameof(InventoryChanged));
	}

	public bool RemoveFromInventory(string item) {
		if(!inventory.ContainsKey(item)) return false;

		inventory[item] --;
		if(inventory[item] == 0) inventory.Remove(item);

		EmitSignal(nameof(InventoryChanged));
		return true;
	}

	public Dictionary<string, int> Inventory {get {
		Dictionary<string, int> returnV = new Dictionary<string, int>();
		foreach (KeyValuePair<string, int> item in inventory)
		{
			returnV.Add(item.Key, item.Value);
		}
		return returnV;
	}}

	static public Player current;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = (WorldObject)FindNode("Sprite",true,false);
		animationPlayer = FindNode("AnimationPlayer",true,false) as AnimationPlayer;
		animationPlayer.PlaybackSpeed = 2f;

		inventory = new Dictionary<string, int>();
		current = this;
		
		EmitSignal(nameof(InventoryChanged));
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
	 if(sprite == null){
		 foreach(Node child in GetChildren()){
			 if(child is WorldObject){
				 sprite = child as WorldObject;
			 }
		 }
		 if(sprite == null){
			 QueueFree();
			 return;
		 }
	 }
	 try{
     Vector2 movement = new Vector2();
	 if(Input.IsActionPressed("ui_up")){
		 movement.y -= 1;
	 }
	 if(Input.IsActionPressed("ui_down")){
		 movement.y += 1;
	 }
	 if(Input.IsActionPressed("ui_right")){
		 movement.x += 1;
	 }
	 if(Input.IsActionPressed("ui_left")){
		 movement.x -= 1;
	 }
	 if(sprite != null){
		 sprite.FlipH = movement.x > 0;
	 }
	 movement = movement.Normalized();
	 this.MoveAndSlide(movement*100f);
	 if(animationPlayer != null){
		 if(movement.y > 0){
		 	animationPlayer.Play("WalkDown");
		 }else 
		 if(movement.y < 0){
		 	animationPlayer.Play("WalkUp");
		 }else
		 if(movement.x != 0){
		 	animationPlayer.Play("WalkLeft");
		 }
	 }
	 }catch(ObjectDisposedException e){
		 sprite = null;
	 }

	 NodePath n = new NodePath();
	
 }
}
