using Godot;
using System;

public class Orb : AnimatedSprite
{

	public Element element;
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Color c = (Color)((ShaderMaterial)Material).GetShaderParam("ColorModifier");
		Material = new ShaderMaterial();
		((ShaderMaterial)Material).Shader = ResourceLoader.Load<Shader>("res://ColorUpShader.tres");
		((ShaderMaterial)Material).SetShaderParam("ColorModifier",c);
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
