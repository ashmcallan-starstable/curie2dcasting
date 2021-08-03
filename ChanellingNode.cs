using Godot;
using System;

public class ChanellingNode : WorldObject
{
	[Export] public float chanellingTime;
	[Export] public float chanellingRadius;
	[Export] public string itemChannelled;
	[Export] public float recoveryTime;

	/// <summary>
	/// Whether this chanelling node is being chanelled or not
	/// </summary>
	public bool isChannelling {get; protected set;}

	/// <summary>
	/// Remaining time (in seconds) for the current channelling to finish
	/// </summary>
	public float remainingChannelTime {get; protected set;}
	/// <summary>
	/// Remaining time (in 0-1) for the current channelling to finish
	/// </summary>
	public float remainingChannelProportion { get {
		if(chanellingTime == 0) return 0; 
		return remainingChannelTime / chanellingTime;
	}}

	/// <summary>
	/// Whether this chanelling node is in the process of recovery and restocking
	/// Call ResetChannelling to immediately bypass this
	/// </summary>
	public bool isRecovering {get; protected set;}
	/// <summary>
	/// Remaining time (in seconds) for the current channel node to recover to usability
	/// </summary>
	public float remainingRecoveryTime {get; protected set;}
	/// <summary>
	/// Remaining time (in 0-1) for the current channel node to recover to usability
	/// </summary>
	public float remainingRecoveryProportion { get {
		if(recoveryTime == 0) return 0; 
		return remainingRecoveryTime / recoveryTime;
	}}

	public override void _Ready()
	{
		// See WorldObject...
		base._Ready();

		// Reset
		FinishRecovery();
		ResetChannelling();
	}

	public new void Input(Node viewport, InputEvent inputEvent, int shapeId){
		// WARNING; Overrides WorldObject!

		if(inputEvent is InputEventMouseButton && ((InputEventMouseButton)inputEvent).Pressed) {

			if(selected == this) {
				if(!isChannelling && IsChannellingOkay()) {
					StartChanelling();
				} else if(isChannelling) {
					CancelChanelling();
				}
			} else {
				GD.Print("Channel Node, {itemChannelled}. Click again to start chanelling");
			}
			
			selected = this;
		}
	}

	public override void _Process(float delta) {

		// See World Object
		base._Process(delta);

		// Handle Channelling script if flagged as chanelling
		if(isChannelling) {
			// Check if chanelling is currently permitted. If so count down, otherwise Reset
			if(IsChannellingOkay()) {

				// rTime temp var just to show to the log every 1/2 second... 
				float rTime = remainingChannelTime;
				remainingChannelTime -= delta;

				if((int)(remainingChannelTime * 2) - (int)(rTime * 2) != 0) {
					GD.Print($"Chanelling {itemChannelled}, {((int)(rTime * 10))/10f} remaining...");
				}

				// if counter finished, Finish, Reset, and begin Recovery
				if(remainingChannelTime <= 0) {
					FinishChannelling();
					ResetChannelling();
					StartRecovery();
				}
			} else {
				// TODO: Report failure some way other than the log...
				ResetChannelling();
			}
		}

		// Handle Recovery script, if flagged as recovery
		if(isRecovering) {
			remainingRecoveryTime -= delta;
			if(remainingRecoveryTime <= 0) FinishRecovery();
		}

		
	}
	/// <summary>
	/// Start Channelling; sets the chanelling flag to true
	/// </summary>
	protected void StartChanelling() {
		GD.Print($"Starting to channel {itemChannelled} [{chanellingTime}s]. Click again to cancel");
		isChannelling = true;
	}

	/// <summary>
	/// Cancel the process of chanelling (user requested)
	/// </summary>
	public void CancelChanelling() {
		GD.Print($"Cancelling channelling {itemChannelled} [{((int)(remainingChannelTime * 10))/10f} was remaining...]");
		ResetChannelling();
	}

	/// <summary>
	/// Finish Chanelling; normally adds the item to the inventory
	/// Can be immediately called from elsewhere to otherwise flag End Chanel operations
	/// Normally should be followed by call to ResetChanelling and Start Recovery
	/// 
	/// Override for more complicated channel nodes
	/// </summary>
	public virtual void FinishChannelling() {
		GD.Print($"Adding 1x {itemChannelled} to Player Inventory...");
		// TODO: Add inventory adjustment here!
	}

	/// <summary>
	/// Reset the Chanelling to a "can be channeled state", deactivating the isChanelling flag and resetting the clock
	/// </summary>
	public void ResetChannelling() {
		GD.Print($"Resetting Chanelling");
		isChannelling = false;
		remainingChannelTime = chanellingTime;
	}

	/// <summary>
	/// Start the recovery phase of this node
	/// </summary>
	public void StartRecovery() {
		isRecovering = true;
		remainingRecoveryTime = recoveryTime;
	}

	/// <summary>
	/// Immediately finish the recovery phase of this node
	/// </summary>
	public void FinishRecovery() {
		isRecovering = false;
		remainingRecoveryTime = 0;
	}

	/// <summary>
	/// If the node can be channelled, returns true
	/// </summary>
	/// <returns></returns>
	public bool IsChannellingOkay() {

		// Check Recovery
		if(isRecovering) {
			GD.Print($"IsChannellingReady FALSE: Not ready (recovering) [{((int)(remainingRecoveryTime * 10))/10f} remaining...]");
			return false;
		}
		// Check selection
		if(selected != this) {
			GD.Print($"IsChannellingReady FALSE: Not selected by player");
			return false;
		}
		// Check range...
		if(Player.current.GlobalPosition.DistanceTo(this.GlobalPosition) > chanellingRadius) {
			isChannelling = false;
			GD.Print($"IsChannellingOkay FALSE: Out of range > {chanellingRadius}px");
			return false;
		}
		return true;
	}
}
