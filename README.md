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

Asset Pipeline: 
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
delete blend file from Unity project > 
re assign meshes to copied ones > 
re assign materials to extracted ones > 
assign components like animator, network transform, player input, colliders, physics rigidbody >
assign scripts like inputHandler, controller, health, gun > 
