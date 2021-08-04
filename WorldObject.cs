using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class WorldObject : SpellTarget
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	Area2D area;

	public static WorldObject selected;

	public Color store = new Color(0.5f,0.5f,0.5f,1f);

	[Export]
	public List<string> ValidSpells = new List<string>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		base._Ready();
        area = new Area2D();
		AddChild(area);
		CollisionShape2D shape = new CollisionShape2D();
		RectangleShape2D rect = new RectangleShape2D();
		rect.Extents = this.RegionRect.Size*0.5f;
		shape.Position = this.Offset;
		shape.Shape = rect;
		area.Name = "clickable";
		area.AddChild(shape);
		area.Connect("input_event", this, "Input");
		this.Material = new ShaderMaterial();
		((ShaderMaterial)Material).Shader = ResourceLoader.Load<Shader>("res://ColorUpShader.tres");
		//store = (Color)((ShaderMaterial)Material).GetShaderParam("ColorModifier");
		
			
		
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
	 base._Process(delta);
     this.ZAsRelative = false;
	 this.ZIndex = (int)(this.GlobalPosition.y);
	 if(this == selected){
		 if(store == null || store.a+store.r+store.g+store.b >3.5f){
			store = (Color)((ShaderMaterial)Material).GetShaderParam("ColorModifier");
		 }
		((ShaderMaterial)Material).SetShaderParam("ColorModifier",Colors.White);
	 }else{
		((ShaderMaterial)Material).SetShaderParam("ColorModifier",store);
		
	 }
 }

	 public void Input(Node viewport, InputEvent inputEvent, int shapeId){
		 if(inputEvent is InputEventMouseButton && ((InputEventMouseButton)inputEvent).Pressed){
		 	GD.Print("input!");
			selected = this;
		 }
	 }


	public List<WorldObject> NearbyInfluences(){
		List<WorldObject> ret = new List<WorldObject>();
		foreach(Node node in GetTree().GetNodesInGroup("Influence")){
			if(node is WorldObject && ((WorldObject)node).GlobalPosition.DistanceTo(this.GlobalPosition) < 64f){
				ret.Add(((WorldObject)node));
			}
		}
		return ret;
	}

	public List<string> NearbyTags(){
		List<string> tags = new List<string>();
		foreach(WorldObject wob in NearbyInfluences()){
			foreach(string tag in wob.GetGroups()){
				if(tag != "Influence" && !tags.Contains(tag)){
					tags.Add(tag);
				}
			}
		}
		return tags;
	}
	// public override void _UnhandledInput(InputEvent @event)
	// {
	// 	InputEventMouseButton e = @event as InputEventMouseButton;
	// 	if(e != null){
	// 		if(GetGlobalMousePosition().DistanceTo(GetGlobalPosition()) < 10f){
	// 			GD.Print("INPUT");
	// 		}
	// 	}
	
	// 	//GD.Print(GetGlobalMousePosition());
	// 	base._UnhandledInput(@event);
	// }
}
