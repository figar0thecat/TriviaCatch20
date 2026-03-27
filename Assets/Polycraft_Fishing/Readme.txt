Polycraft_Fishing (2021/6/20 - by Fatty War, id3644@gmail.com)

[V1.02 / Jul 2, 2023]
Eliminate build errors in ParabolaString.cs

[V1.01 / May 29, 2023]
Fishing line example scene (line renderer / Demo_FishingRodV2)
-Example of fishing line using line renderer.
-The fishing line expresses only the parabola from the buoy to the tip of the fishing rod.
- A script was used, but we do not guarantee the quality of the script.

cube style pond fishing scene(Demo_FishingExample)
- You can fish in a surreal square pond.
- A script was used, but we do not guarantee the quality of the script.


==This document explains how to use the pack.==

Introduction

    This is a low-poly art set to start your fishing game.
	Features a fishing rod, bait, float, hook, fish and an angler.
	Customize your fishing grounds with terrain blocks, cartoon water, modular cabins and environment props.
	

1. Prefabs
	*Demo
		Contains prefabs used in the demo scene.

	*Fish
		Contains a static fish prefab.
		Animated fish are in the "Anim" folder.

	*FishingPole
		Contains a static fishing rod prefab.
		Animated fishing rods are contained in the "Anim" folder.


2. Water Shader
    This shader can express cartoon-style water. (Assets\Polycraft_Fishing\Shaders\ToonWater.shader)

    Water Shader Manual(google drive : https://docs.google.com/document/d/1KHBy1Ss5PQfxNuop0LA4xL_0048DNRoz2yLbntmpsMY/edit#heading=h.gp9aoxjnbn76)
		
		* Depth blending support (two colors)
        * Water edge support(water foam)
        * Mobile support
        * Surface animation Support
		* Water shader not support orthographic camera
		
		* Note) You need the “CameraDepthTextureMode” script to use Water Shader on your mobile device. (Assets \ ForestStagePack \ Scripts \ CameraDepthTextureMode.cs)


        * To use the camera depth texture, follow these steps:     (URP project is not applicable)
			1Step.Attach the script to the camera.
			2Step.Change Depth Texture Mode to "Depth".

			-URP project is built into the camera, please turn on the depth texture function.(Camera / Rendering / Depth Texture > On)

3.Scripts. (Assets\Polycraft_Fishing\Scripts)
	
	*BobbingBoat.cs
	   -Floating movements are applied.(Not recommended for use. Only for demo)

	*CameraDepthTextureMode.cs
	   -Make the camera use depth textures on mobile devices.

	*FishingController.cs, FishingZone.cs
		-This is a fishing controller for the demo scene.

	*FishingRodCurve.cs
		-Controls the movement of the fishing rod with physics applied.
		-This is for suggestions only and does not guarantee quality.

	*FishMoving.cs
		-Make the fish follow the track.
		-See the "DummyTrack" prefab for the track.

	*FollowTarget.cs
		-This is a camera control script.

	*JoyButton.cs
		-This is the UI script used for the fishing rod demo scene.


4.Example Scene(Assets\ModularTrackKit\Scenes)

	-Demo_FishingRod
       -Includes 12 template tracks.
	   -You can preview the track by pressing the editor play button and using WASD.

	-Demo_PolycraftFishing
		This is an animated fishing rod demo scene.

	-Display_PolycraftFishing
		Prefabs included in the package are displayed.


*If you have any questions or suggestions about the assets, please contact me.(id3644@gmail.com)
Thank you for your purchase.
