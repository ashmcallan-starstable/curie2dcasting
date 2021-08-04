using Godot;
using System;
using System.Collections.Generic;

public class WorldControl : Node2D
{
	[Export] public NodePath tileMapRoot;
	[Export] public Dictionary<string, int> channellingObjects;

	protected List<ChanellingNode> chanellingNodes;

	public override void _Ready()
	{
		// Setup local varaibles
		chanellingNodes = new List<ChanellingNode>();

		// Create channelling nodes based on channellingObjects list, and add them to the list
		TileMap tileMapRootNode = GetNode<TileMap>(tileMapRoot);
		PackedScene scene = ResourceLoader.Load<PackedScene>("res://Prefabs/ChanellingNode.tscn");
		foreach(KeyValuePair<string, int> kvp in channellingObjects) {
			string type = kvp.Key;
			int numberOfNodes = kvp.Value;

			for(int i = 0; i < numberOfNodes; i++) {
				ChanellingNode node = scene.Instance<ChanellingNode>();
				tileMapRootNode.AddChild(node);

				node.itemChannelled = type;
				chanellingNodes.Add(node);
			}
		}

		RefreshAllChanellingNodes();

	}

	public void RefreshAllChanellingNodes() {
		Random randomVal = new Random();

		foreach(ChanellingNode node in chanellingNodes) {

			// Random reposition
			node.Position = 8 * new Vector2(
				24 + (int)(randomVal.NextDouble() * 1024 / 8),
				24 + (int)(randomVal.NextDouble() * 560 / 8)
			);
			
			// force recharge
			node.FinishRecovery();
		}
	}
}
