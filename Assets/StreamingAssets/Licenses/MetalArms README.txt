# Metal Arms
This is a non-profit, fan-made recreation of Metal Arms: Glitch in the System created by Swingin Ape Studios.
The creator of this fan-made recreation takes no credit for the production of the assets, sounds, soundtrack.
PCB Productions owns the legal rights to the soundtrack.
Metal Arms' Soundtrack is owned by PCB Productions and is in no way associated with the user who uploaded them.

vis2k Mirror (MIT License): https://github.com/vis2k/Mirror

Unity (Personal License)


TABLE OF CONTENTS
	Input Handler
	Controller
	Multiplayer
	Saved Settings
	DEBUG

INPUT HANDLER
	Script: Input Handler
		Uses InputActions from the Unity InputSystem package.
	This sets up input action callbacks which the PlayerInput script on the player prefab references to invoke unity events.
	This allows for easy revision of control schemes through the InputAction asset.

CONTROLLER
	Script: Controller
		Defines player actions, animations, and properties.
	Spawns SceneObjects when breaking off pieces, breaking/placing blocks, shooting blocks...
	SceneObjects are used to simplify the networked spawning of objects thru Mirror.
	Actions:
		
	Animations:
		Defined procedurally based on 2 imported keyframes for charIdle.ldr and charRun.ldr
		Makes the appearance of animation by toggling these two imported models on/off. Chosen for stylistic reasons as well since
		most brick-films have a destinctive lower fps. Also much simpler to procedurally animate this way than to define bones
		programmatically.
	Variables:
		reach (distance player can shoot/grab, procedurally defined based on imported model)
		collider height (procedurally defined based on charIdle.ldr # pieces)
		collider radius (procedurally defined based on charIdle.ldr # pieces)

MULTIPLAYER
	Online Network Play uses Mirror by vis2k (MIT License)
	LOCAL/SPLITSCREEN
		1. Additional controller players may join using the Xbox “A” button. Players may then leave using the options menu.

	ONLINE MULTIPLAYER
		1. All players must be using the same version of the game.
		2. Hosts must share their IP address, planet number, assets folder, and world folder with other players before playing (must synchronize asset/world data)
   			Public IP address if not on same LAN
   			LAN IP address if same LAN
		3. All hosts must port forward thru port 7777.
		4. Other players must put the shared world file in the save file location and enter the host IP address and planet number.
	NOTE: For best practice, all players should join a multiplayer game at same time (or close to) after sharing world and ldraw files manually.

	SAVE FILE LOCATION
		1. (Windows) C:\Users%userprofile%\AppData\LocalLow\Sam Hsu\Digital Bricks\saves\
		2. (MacOS) ~/Library/Application Support/Sam Hsu/Digital Bricks/saves/
		3. (UWP) %userprofile%\AppData\Local\Packages\Digital Bricks\LocalState\saves\

SAVED SETTINGS
	player preferences for gameplay settings are stored in the settings.cfg file in C:\Digital Bricks\Digital Bricks_Data
	player preferences include in-game options menu selections for
		ip Address
		Volume
		Look Speed
		Look Accel
		FOV
		InvertY
		InvertX

DEBUG
	Please report all bugs and on GitHub and share your Player.log file (please check if the issue was already reported): https://github.com/SamHsuGit/LDPlay/issues

	Player.log FILE LOCATIONS
		Windows: C:\Users%userprofile%\AppData\LocalLow\Sam Hsu\Digital Bricks\Player.log"
		MacOS: "~/Library/Application Support/Sam Hsu/Digital Bricks/Player.log"
		UWP = "%userprofile%\AppData\Local\Packages\Digital BricksLocalState\Player.log"
	