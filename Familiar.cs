using Godot;
using System;
using System.Collections.Generic;

public class Familiar : KinematicBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
	public Vector2 lastpos = Vector2.Zero;
	public static Familiar selected = null;
	AnimationPlayer player;
	Sprite sprite;


	Area2D clickable = null;

	[Export]
	public int frame = -1;
	[Export]
	public Color tone = new Color(0.5f,0.5f,0.5f,1f);

	[Export]
	public List<Element> elements;

	public Element? channeling;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
		player = FindNode("AnimationPlayer",true,false) as AnimationPlayer;
		player.PlaybackSpeed = 2f;
		lastpos = GlobalPosition;
		sprite = FindNode("Sprite",true,false) as Sprite;
		if(frame == -1){
			Random r = new Random();
			frame = r.Next(49);
			tone = new Color((float)r.NextDouble(),(float)r.NextDouble(),(float)r.NextDouble(),1f);
		}
		sprite.Frame = frame;
		((WorldObject)sprite).store = tone;
    }

 // Called every frame. 'delta' is the elapsed time since the previous frame.
 public override void _Process(float delta)
 {
     if(Player.current != null && selected == this && Player.current.GlobalPosition.DistanceTo(this.GlobalPosition) > 12f && this.GetParent().Name != "Player"){
		 this.MoveAndSlide((Player.current.GlobalPosition-this.GlobalPosition).Normalized() *delta*30000f);
		 
	 }
	 if(WorldObject.selected == (WorldObject)this.FindNode("Sprite",true,false)){
		 selected = this;
	 }
	 if(lastpos != GlobalPosition){
		 player.Play("Walk");
	 }
	 lastpos = GlobalPosition;

	 if(clickable == null){
		 clickable = FindNode("clickable",true,false) as Area2D;
		 if(clickable != null){
			 clickable.Connect("input_event",this,"Input");
		 }
	 }

 }
	public void ElementSelect(Element e){
		if(SpellCaster.target != null){
			if(channeling != null){SpellCaster.target.RemoveElement(e);}
			SpellCaster.target.Add(e);
		}
		channeling = e;
		Clear();
	}

	public void Clear(){
		foreach(Node n in GetChildren()){
			if(n is VBoxContainer){
				n.QueueFree();
			}
		}
	}

	 public void Input(Node viewport, InputEvent inputEvent, int shapeId){
		if(inputEvent is InputEventMouseButton){
			if( ((InputEventMouseButton)inputEvent).ButtonIndex == 2){
				GD.Print("remap");
				if(sprite.Scale.x > 1f){
					if(GetParent().Name != "Player"){
						this.GetParent().RemoveChild(this);
						Player.current.AddChild(this);
						this.Position = new Vector2(0,12);
					}else{
						Vector2 p = GlobalPosition;
						this.GetParent().RemoveChild(this);
						Player.current.GetTree().Root.FindNode("TileMap2",true,false).AddChild(this);
						GlobalPosition = p+new Vector2(0,16);
					}
				}else{
					player.Play("Walk");	
				}
			}else if(((InputEventMouseButton)inputEvent).ButtonIndex == 1){
				VBoxContainer vbox = new VBoxContainer();
				foreach(Element e in this.elements){
					Button b = new Button();
					b.Text = e.ToString();
					Godot.Collections.Array a = new Godot.Collections.Array();
					a.Add(e);
					b.Connect("pressed",this,"ElementSelect",a);
					vbox.AddChild(b);
					this.AddChild(vbox);
				}
			}
		}
		
	 }

}
