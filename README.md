### OVERVIEW

Trash Island is a procedurally generated, mid-apocalyptic island world. You enter a contaminated zone as a disposable employee and must restore the power in the area.

##### 1. WORLD GENERATION
#
The structure of the world is divided into 4 equally sized **Regions**, each containing a number of **Islands**. The look and feel of every Region is driven by it's **Biome**, which is defined in data as a ScriptableObject.

Each biome has a list of parameters that control the characteristics of the islands. This includes the actual size, height and shape of each Island. (See Mesh Generation) In addition, the types of Spawnable objects that can appear on each island are defined by it's associated biome.

##### 2. MESH GENERATION
#
The meshes for the islands themselves are generated at runtime from the data-driven parameters of it's associated biome. The central method behind creating this mesh is first creating a height-map for the island. This is implemented as a 2D-array of floats that represent the relative height at any given coordinate of the island's relative space. An interesting challenge with this project was creating topographic maps of islands that each felt unique. To achieve this, I started by creating a smoothed, noise map - essentially setting each value randomly and smoothing nearby values. This resulted in a noisy, rolling landscape. Not particularly interesting. 

I expanded on this by adding a falloff map, so that the height values towards the edges of each map would approach 0. Assuming that a height=0 is "underwater", the height maps started to appear like islands. However, each Island still felt generally the "same", since they were all being created the same way (from random noise). To make each one more unique, I implemented a point-based approach, where for each map, an array of coordinates act as height modifiers. Essentially, these points are like mountain peaks, and all surrounding points are raised based on their respective distance to peak. In figure 1, the points are placed randomly within the island's bounds;in figure 2 the points are placed along an animation curve. Once the height maps are created, a mesh is generated from this data at runtime. (See Figure 3)

![Hell Texture Map](/Assets/Resources/Etc/Hell_p75_s200.jpg)
#
**Figure 1** (Hell Biome)
#
![Swamp Texture Map](/Assets/Resources/Etc/Swamp_p75_s200.jpg)
#
**Figure 2** (Swamp Biome)
#
![Generated Mesh](/Assets/Resources/Etc/island_mesh_example.jpg)
**Figure 3** (Generated Mesh)
#

