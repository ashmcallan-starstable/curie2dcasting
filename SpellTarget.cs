using Godot;
using System;
using System.Collections.Generic;
public class SpellTarget : Sprite
{
    // Declare member variables here. Examples:
    // private int a = 2;
	
	Dictionary<Element,Color> elementColors = new Dictionary<Element, Color>(){
		{Element.vine,Colors.Green},
		{Element.creature,Colors.Tan},
		{Element.tree,Colors.DarkGreen},
		{Element.rock,Colors.Gray},
		{Element.grandstone,Colors.DarkGray},
		{Element.grandvine,Colors.SeaGreen}

	};

	[Export]
	Element contribution = Element.none;

	public bool contribute = false;


    // private string b = "text";
	public static Dictionary<Element,PackedScene> ElementNodes;
	AnimationPlayer animationPlayer = null;
	Vector2 lastPos = Vector2.Zero;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if(ElementNodes == null){
			ElementNodes = new Dictionary<Element, PackedScene>{
				{Element.vine, ResourceLoader.Load<PackedScene>("res://Prefabs/vine.tscn")},
				{Element.tree, ResourceLoader.Load<PackedScene>("res://Prefabs/tree.tscn")},
				{Element.grandvine, ResourceLoader.Load<PackedScene>("res://Prefabs/tree.tscn")},
				{Element.creature, ResourceLoader.Load<PackedScene>("res://Prefabs/Familiar.tscn")},
				{Element.rock, ResourceLoader.Load<PackedScene>("res://Prefabs/Rock.tscn")},
				{Element.grandstone, ResourceLoader.Load<PackedScene>("res://Prefabs/Rock.tscn")},
			};
		}
			foreach(Node child in GetChildren()){
				if(child is AnimationPlayer){
					animationPlayer = child as AnimationPlayer;
				}
			}
			if(animationPlayer == null){
				animationPlayer = new AnimationPlayer();
				AddChild(animationPlayer);

				animationPlayer.Stop();
				Animation walk = ResourceLoader.Load<Animation>("res://Walk.anim");
				animationPlayer.RootNode = this.GetPath();
				animationPlayer.AddAnimation("walk",walk);
				animationPlayer.PlaybackSpeed = 2f;
				animationPlayer.Connect("animation_finished",this,"WalkDone");
			}
		
    }

	public void Add (Element e){
		PackedScene orbScene = ResourceLoader.Load<PackedScene>("res://Prefabs/orb.tscn");
		Orb orb = orbScene.Instance() as Orb;
		orb.Scale = orb.Scale/this.Scale;
		orb.Position = new Vector2(0f,-16f);
		orb.element = e;
		AddChild(orb);
		((ShaderMaterial)orb.Material).SetShaderParam("ColorModifier",elementColors[e]);
		AnimationPlayer anim = orb.FindNode("AnimationPlayer",true,false) as AnimationPlayer;
		anim.Play("orbit");
	}

	public List<Element> Elements(){
		List<Element> ret = new List<Element>();
		foreach (var item in GetChildren())
		{
			if(item is Orb){
				ret.Add(((Orb)item).element);
			}
		}
		return ret;
	}

	public Element? PrimaryElement(){
		if(Elements().Count <= 0){
			return null;
		}
		GD.Print("Prime element:");
		GD.Print(Elements()[Elements().Count-1]);
		return Elements()[Elements().Count-1];
	}

	public void ClearElements(){
		foreach (var item in GetChildren())
		{
			if(item is Orb){
				((Orb)item).QueueFree();
			}
		}
	}
	public void RemoveElement(Element e){
		foreach (var item in GetChildren())
		{
			if(item is Orb && ((Orb)item).element == e){
				((Orb)item).QueueFree();
				return;
			}
		}
	}
public void InfluenceCheck(string what){
	contribute = true;
	animationPlayer.Play("walk");
}

public void WalkDone(string anim){
	if(contribute && contribution != Element.none){
		SpellCaster.target.Add(contribution);
	}
	contribute = false;
}

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     if(GlobalPosition != lastPos && lastPos != Vector2.Zero && animationPlayer.HasAnimation("walk")){
		 animationPlayer.Play("walk");
	 }
	 lastPos = GlobalPosition;
 }
}

public enum Element {none,vine,tree,rock,creature,grandstone,grandvine}
