readme.txt
Alexander Terp & Samuel Xu

-------- *IMPORTANT* -----------------------------------------------------------------------
At first glance, the light may not seem to be moving with time as specified.
However, it is - but in real time. 
Press ESC to bring up the UI, and you can tweak the speed of time in the top left corner.
Increase this slider to the right, and you'll begin to see the day/night cycle in action.
--------------------------------------------------------------------------------------------

We utilize procedural meshes in the implementation of our terrain. The steps for our project
is as follows:

1. Get/create a Mesh object (part of MeshFilter) for the terrain object.
2. Calculate a (2^n + 1) by (2^n + 1) 2D float[,] array using the Diamond-Square algorithm, 
	each float corresponding to a height.
3. Generate ((2^n + 1) * (2^n + 1)) vertices row by row, each spaced correctly. Assign the
	corresponding heights from the aforementioned float[,] array.
4. Set the mesh triangles properly, row by row.
5. Utilize in-built Unity methods to calculate mesh normals and a mesh collider.
6. Calculate the lowest and highest points on the terrain using the aforementioned float[,]
	heights array.
7. Generate a procedural mesh for the water using a sufficient number of vertices and a 
	similar method as generating the terrain's mesh (except no heights). 
8. Add a mesh collider to the water.
9. Set the height of the water to be the midpoint between the highest and lowest points on
	the terrain. This ensures water and land is always visible each generation.
10. Set the colors of vertices on the terrain. This involves taking into account the height
	of the water to determine the level of sand, as well as taking into account the highest
	point to determine a level for snow. From here, the rest of the colors of mountain and 
	grass are interpolated.

We then utilize a custom shader that performs Phong illumination and shading while also 
rendering the colors of the vertices to give the terrain height-based coloration.

Waves shader:
Additionally, we utilize a custom shader for the waves. This shader implements 
Gerstner-style waves and contains two octaves i.e. large waves and small waves on top
of each other. Vertices are properly displaced, as specified.

Terrain resolution:
The terrain, on default, has n set to equal 7 i.e. it has a total of (2^7 + 1)^2 = 16,641 
vertices. The max that a single mesh in Unity allows is around 65,000. However, if we 
increase n to 8, we get (2^8 + 1)^2 = 66,049 vertices, which is slightly over the limit. 
This causes some artifacting, and so we limit n to be [1, 7].

Camera:
The camera has been implemented as specified. The user can intuitively pitch, yaw, and 
move with the WASD keys. They are bounded within the dimensions of the terrain utilizing
a simple clamping method. We have also attached a sphere collider to the camera to prevent
the user from going underground (recall that both the terrain and water has mesh colliders).
The camera's collider is set to Continuous collision detection to prevent tunneling, which
would be possible if the user tried to "sprint" (see "Extras" section) perpendicularly to 
the water or terrain and it was set to discrete.

Lighting / day/night cycle:
We've implemented a full day/night cycle using two directional lights. The sun and moon are 
modeled by flattened spheres, and the night stars are generated via a particle system. As 
time progresses, these rotate, and lighting is changed accordingly as well. Additional 
details about the day/night cycle addressed in the "Extras" section.

Extras:
We enjoyed the project so much that we wanted to add some additional features, some of 
which affected the specified features in some way.
- We implemented the ability to "sprint" with left shift, which goes 12x normal speed.
- We implemented UI system that lets the user tweak values in the simulation, such as 
	terrain generated parameters, wave parameters, coloring parameters, time, fov, etc.
- We implemented an in-game time clock. This clock goes from 0 seconds to 86399 seconds,
	or 11:59:59 PM, before ticking over and wrapping back to 0. The aforementioned 
	day/night cycle is tied to this clock, which goes at real time. This means that, 
	if left at default speed, it'd take 24 real hours to simulate a day/night cycle.
	However, both the time and time speed can be set in the top left of the UI.
	Also, the waves are tied to this in-game clock. Therefore, if you speed up time,
	the oscillation of the waves speed up too.

Disclaimers:
Water texture provided by SimplyBackgrounds @ Deviant Art
https://www.deviantart.com/simplybackgrounds/art/Water-Texture-49283686