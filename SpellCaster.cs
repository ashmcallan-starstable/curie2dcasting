using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class SpellCaster : Node
{
	List<string> tags = new List<string>();

	List<Component> components = new List<Component>();

	public static SpellTarget target;

	List<Component> queue = new List<Component>();

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        PopulateComponents();
    }

	public List<string> Tags(){
		var tags = ((WorldObject)Player.current.FindNode("Sprite",true,false)).NearbyTags();
		GD.Print("tags:");
		if(tags.Count > 0){
			GD.Print(tags.Aggregate((a,b)=>a+","+b));
		}
		return tags;
	}

	public void Cast(string what){

		GD.Print(target);
	
		foreach(Component component in components){
			if(what.ToLower() == component.tag.ToLower() && component.perform != null){
				component.perform();
				GD.Print("perform :"+what);
			}
		}
		foreach(Component component in components){
			if(what.ToLower() == component.tag.ToLower() && component.getTarget != null){
				component.getTarget();
				GD.Print("target :"+what);

			}
		}
		foreach(Component component in components){
			if(what.ToLower() == component.tag.ToLower() && component.preprocess != null){
				component.preprocess(target,null);
				GD.Print("preprocess :"+what);
				foreach(WorldObject wo in Player.current.sprite.NearbyInfluences()){
					wo.InfluenceCheck(what);
				}
			}
		}
		foreach(Component component in components){
			if(what.ToLower() == component.tag.ToLower() && component.cast != null){
				component.cast(target,null);
				GD.Print("cast :"+what);
				if(SpellCaster.target != null){
					SpellCaster.target.ClearElements();
					SpellCaster.target = null;
				}
				WorldObject.selected = null;
			}
		}
		GD.Print(target);
	}


	public void PopulateComponents(){

		components.Add(new Component(){
			tag="me",
			getTarget=delegate(){
				
				if(SpellCaster.target != null){SpellCaster.target.ClearElements();}
				if(WorldObject.selected == null){return null;}
				SpellCaster.target = GetTree().Root.FindNode("Player",true,false).FindNode("Sprite",true,false) as WorldObject; return SpellCaster.target;}
		});
		components.Add(new Component(){
			tag="that",
			getTarget=delegate(){
				if(SpellCaster.target != null){SpellCaster.target.ClearElements();}
				if(WorldObject.selected == null){return null;}
				SpellCaster.target = WorldObject.selected as WorldObject;
				return SpellCaster.target;}
		});


		components.Add(new Component(){
			tag="here",
			getTarget=delegate(){
				WorldObject.selected = null;
				if(SpellCaster.target != null){SpellCaster.target.ClearElements();}
				Node2D player = (Node2D)GetTree().Root.FindNode("Player",true,false);
				PackedScene scene = ResourceLoader.Load<PackedScene>("res://Prefabs/Rune.tscn");
				SpellCaster.target = scene.Instance() as TargetRune;
				((TargetRune)target).Scale = ((TargetRune)target).Scale*4f;
				((TargetRune)target).GlobalPosition = player.GlobalPosition;
				GetTree().Root.FindNode("TileMap2",true,false).AddChild(((TargetRune)target));
				return SpellCaster.target;
			}
		});

		components.Add(new Component(){
			tag="there",
			getTarget=delegate(){
				if(SpellCaster.target != null){SpellCaster.target.ClearElements();}
				WorldObject.selected = null;
				Node2D player = (Node2D)GetTree().Root.FindNode("Player",true,false);
				PackedScene scene = ResourceLoader.Load<PackedScene>("res://Prefabs/Rune.tscn");
				SpellCaster.target = scene.Instance() as TargetRune;
				((TargetRune)target).Scale = ((TargetRune)target).Scale*4f;
				((TargetRune)target).GlobalPosition = player.GlobalPosition;
				((TargetRune)target).mouseTrack = true;
				GetTree().Root.FindNode("TileMap2",true,false).AddChild(((TargetRune)target));
				return SpellCaster.target;
			}
		});

		components.Add(new Component(){
			tag="appear",
			cast=delegate(object target, object context){
				if(SpellCaster.target != null){
					Element? e = SpellCaster.target.PrimaryElement();
					GD.Print(e);
					if(e != null){
						PackedScene c = SpellTarget.ElementNodes[e.Value];
						Node n = ((PackedScene)c).Instance();

						if(target is WorldObject){
							GD.Print("worldobject target context");
							bool groupshared = false;
							foreach(string group in n.GetGroups()){
								GD.Print("Checkgroup:"+group);
								foreach(string g2 in ((Node)target).GetGroups()){
									GD.Print("hasgroup:"+g2);
								}
								if(((WorldObject)target).IsInGroup(group)){
									groupshared = true;
									GD.Print("SHARED!");
									break;
								}
							}
							if(!groupshared){
								((Node)target).GetParent().AddChild(n);
								((Node2D)n).GlobalPosition = ((Node2D)target).GlobalPosition;
							}else{
								((WorldObject)target).Scale *= 1.5f;
								n.QueueFree();
							}
						}
						if(target is TargetRune){
							Rect2 rect = new Rect2();
							rect.Size = ((TargetRune)target).GetRect().Size*((TargetRune)target).Scale*0.5f;
							rect.Position = ((TargetRune)target).GlobalPosition - (rect.Size/2f);
							for(int x = (int)((Rect2)rect).Position.x;x<=((Rect2)rect).Position.x+((Rect2)rect).Size.x;x+=16){
								for(int y = (int)((Rect2)rect).Position.y;y<=((Rect2)rect).Position.y+((Rect2)rect).Size.y;y+=16){
									Node nn = ((PackedScene)c).Instance();
									GetTree().Root.FindNode("TileMap2",true,false).AddChild(nn);
									((Node2D)nn).Position = new Vector2(x,y);
									
									if(nn.GetGroups().Cast<string>().Intersect(Tags()).Any()){
										((Node2D)nn).Scale *= 1.5f;
									}
									WorldObject firstFamiliar = Player.current.sprite.NearbyInfluences().FirstOrDefault(g=>g.IsInGroup("Creature"));
									GD.Print("familiar?");
									if(firstFamiliar != null){
										GD.Print("Familiar:"+firstFamiliar.Name);
										if(nn is WorldObject){
											Color co = (Color)((ShaderMaterial)firstFamiliar.Material).GetShaderParam("ColorModifier");
											GD.Print(co);
											((WorldObject)nn).store =co;
										}
									}
									GD.Print(x,y);
								}
							}
						}
					}
				}
				return null;
			}
		});
		
		components.Add(new Component(){
			tag="bridge",
			cast=delegate(object target, object context){
				if(SpellCaster.target != null){
					Element? e = SpellCaster.target.PrimaryElement();
					GD.Print(e);
					if(e != null){
						PackedScene c = SpellTarget.ElementNodes[e.Value];


						Vector2 destination = Vector2.Zero;
						if(target is Node2D){
							destination = ((Node2D)target).GlobalPosition;
						}
						Vector2 current = Player.current.GlobalPosition;
						TileMap tm = GetTree().Root.FindNode("TileMap2",true,false) as TileMap;
						float smallest = float.PositiveInfinity;
						while(true){
							if(tm.GetCell((int)(current.x/16f),(int)(current.y/16f)) == tm.TileSet.FindTileByName("greenpool") || tm.GetCell((int)(current.x/16f),(int)(current.y/16f)) == tm.TileSet.FindTileByName("brownpool")){
								Node2D spawn= ((PackedScene)c).Instance() as Node2D;
								spawn.GlobalPosition = current;
								if(spawn.GetGroups().Cast<string>().Intersect(Tags()).Any()){
									((Node2D)spawn).Scale *= 1.5f;
								}
								GetTree().Root.FindNode("TileMap2",true,false).AddChild(spawn);
							}
							current += (destination-current).Normalized()*16f;
							if(current.DistanceTo(destination) < smallest){
								smallest = current.DistanceTo(destination);
							}else{
								break;
							}
						}
					}
				}
				return null;
			}
		});



		components.Add(new Component(){
			tag="vine",
			preprocess = delegate(object target,object context){
					if(SpellCaster.target != null){
						SpellCaster.target.Add(Element.vine);
					}				
				return null;
			}
		});
		components.Add(new Component(){
			tag="creature",
			preprocess = delegate(object target,object context){
					if(SpellCaster.target != null){
						SpellCaster.target.Add(Element.creature);
					}				
				return null;
			}
		});

		components.Add(new Component(){
			tag="transform",
			cast=delegate(object target, object context){
				if(SpellCaster.target != null){
					Element? e = SpellCaster.target.PrimaryElement();
					GD.Print(e);
					if(e != null){
						PackedScene c = SpellTarget.ElementNodes[e.Value];
						Node n = ((PackedScene)c).Instance();
						((Node)target).GetParent().AddChild(n);
						((Node2D)n).GlobalPosition = ((Node2D)target).GlobalPosition;
						if(n.GetGroups().Cast<string>().Intersect(Tags()).Any()){
							((Node2D)n).Scale *= 1.5f;
						}

						((Node)target).QueueFree();
						Status.Log("Turning "+((Node)target).Name+" into "+n.Name);
					}

				}
				return null;
			}
		});
		components.Add(new Component(){
			tag="follow",
			cast=delegate(object target, object context){
				if(SpellCaster.target != null){
					PackedScene packed = ResourceLoader.Load<PackedScene>("res://Prefabs/follower.tscn");
					Follower f = packed.Instance() as Follower;
					SpellCaster.target.AddChild(f);
				}
				return null;
			}
		});
		components.Add(new Component(){
			tag="stay",
			cast=delegate(object target, object context){
				if(SpellCaster.target != null){
					foreach(Node child in SpellCaster.target.GetChildren()){
						if(child is Follower){
							child.QueueFree();
						}
					}
				}
				return null;
			}
		});



		components.Add(new Component(){
			tag="tree",
			preprocess = delegate(object target,object context){
					if(SpellCaster.target != null){
						SpellCaster.target.Add(Element.tree);
					}				
				return null;
			}
		});

		components.Add(new Component(){
			tag="rock",
			preprocess = delegate(object target,object context){
					if(SpellCaster.target != null){
						SpellCaster.target.Add(Element.rock);
					}				
				return null;
			}
		});
	}

	public Rect2 ObjectRect (Sprite node){
		Rect2 rect = new Rect2();
		rect.Size = (node).GetRect().Size*(node).Scale*0.5f;
		rect.Position = (node).GlobalPosition - (rect.Size/2f);
		return rect;
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

public class Spell{
	public List<Element> elements = new List<Element>();
	public string name;
	public Func<object> perform;
}

public class Component{
	public string tag;

	public Func<object> getTarget;
	public Func<object,object,object> preprocess;
	public Func<object,object,object> cast;

	public Func<object> perform;


}
