# Project Glitch
This is a non-profit, fan-made recreation.
The creators of this fan-made recreation take no credit for the production of the assets, sounds, soundtrack.
PCB Productions owns the legal rights to the soundtrack.
The Soundtrack is owned by PCB Productions and is in no way associated with the user who uploaded them.

Installer:
https://sam-hsu.itch.io/project-glitch
Password: "exavolt"

Build the game yourself:
Unity 2021.3.16f1

vis2k Mirror (MIT License): https://github.com/vis2k/Mirror

Unity (Personal License)

Asset Files:
https://drive.google.com/drive/folders/1pyk0Zvp2rV6Oi3VI_Pq_EvOhh8XvEtPu?usp=drive_link

Resources Used:
https://archive.org/details/malevelkitv-2.7z
https://www.models-resource.com/gamecube/metalarmsglitchinthesystem/
https://steamcommunity.com/sharedfiles/filedetails/?id=1840135546

Contribute:

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
