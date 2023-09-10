# Project Glitch
This is a non-profit, fan-made recreation.
The creators of this fan-made recreation take no credit for the production of the assets, sounds, soundtrack.
PCB Productions owns the legal rights to the soundtrack.
The Soundtrack is owned by PCB Productions and is in no way associated with the user who uploaded them.

Play the Game:
https://sam-hsu.itch.io/project-glitch
Password: "exavolt"

Controls:
Move = WASD (Left Stick)
Look = Mouse (Right Stick)
Jump = Space (Button East)
Shoot = Left Click (Right Trigger)
Grenade = Right Click (Left Trigger)
Melee = F Key (Right Bumper)
Interact Y Key (Button North)
Change Primary = Hold E + Scroll Wheel (Hold Button South + D Pad)
Change Secondary = Hold Q + Scroll Wheel (Hold Button West + D Pad)
Reload = Tap E (Tap Button West)

Build the game yourself:
Unity 2021.3.16f1

Contributing Guidelines:

My goals for this Project:
1. All Weapons from original game
2. One multiplayer map: Tanks alot
3. Bot Possession Mechanic
4. Vehicles
5. Enemy Bot AI
6. Splitscreen Multiplayer
7. Online Host-Client Multiplayer (up to 16)

ALL CHANGES MUST BE PLAYTESTED BEFORE COMMIT

Scripting:
The main script is the Controller Script which uses Mirror for networking (see Mirror documentation)

Assets/Materials:
Character Asset Pipeline: 
blender > 
split sungle mesh into individual parts > 
rigging > 
animation > 
drop into Unity Assets folder structure > 
extract materials into materials subfolder > 
importer setup for animations (start/end frames) > 
mark animation event triggers for animation event script > 
set skeleton as humanoid derive from model with torso as root >
import Asset into Unity/create prefab >
copy meshes into mesh subfolder > 
delete blend file from Unity project (to be able to upload to GitHub) > 
re assign meshes to copied ones > 
re assign materials to extracted ones > 
assign components like animator, network transform, player input, colliders, physics rigidbody >
assign scripts like inputHandler, controller, health, gun > 
add tags and layers (for collisions) > 

Map Asset Pipeline: 
blender > 
drop into Unity Assets folder structure > 
extract materials into materials subfolder > 
import Asset into Unity/create prefab >
copy meshes into mesh subfolder > 
delete blend file from Unity project (to be able to upload to GitHub) > 
re assign meshes to copied ones > 
re assign materials to extracted ones > 
assign components like mesh colliders > 
set all objects as static > 
add bots, vehicles, pickups >
add tags and layers (for collisions) > 
add map to new scene with pickups added to World script to be respawned > 

vis2k Mirror (MIT License): https://github.com/vis2k/Mirror

Unity (Personal License)

Asset Files:
https://drive.google.com/drive/folders/1pyk0Zvp2rV6Oi3VI_Pq_EvOhh8XvEtPu?usp=drive_link

Resources Used:
https://archive.org/details/malevelkitv-2.7z
https://www.models-resource.com/gamecube/metalarmsglitchinthesystem/
https://steamcommunity.com/sharedfiles/filedetails/?id=1840135546
