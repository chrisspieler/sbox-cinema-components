﻿global using System.Linq;
global using System;
using Sandbox;


public static class Program
{
	public static void Main()
	{
		//
		// In the future this won't exist, we'll have an option for "default scene"
		// and when you press play it'll load that.
		//

		GameManager.ActiveScene = new Scene();
		GameManager.ActiveScene.LoadFromFile( "scenes/projector_test.scene" );

		GameManager.IsPlaying = true;
	}
}
