using Godot;
using System;

public class LockedChanellingNode : ChanellingNode
{
	[Export] public bool LocksOnCompletion = true;
	public bool isLocked {get; protected set;}

	public virtual void Unlock() {
		this.isLocked = false;
	}

	public virtual void Lock() {
		this.isLocked = true;
	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		if(isLocked) ((ShaderMaterial)Material).SetShaderParam("ColorModifier",store * 0.5f);
	}

	override public bool IsChannellingOkay () {
		if(isLocked) {
			GD.Print($"IsChannellingReady FALSE: Not ready (locked)");
			return false;
		}
		return base.IsChannellingOkay();
	}
	override public void FinishChannelling() {
		base.FinishChannelling();
		if(LocksOnCompletion) Lock();
	}
}
