# ARCHITECTURE

Trash Island is a procedurally generated, mid-apocalyptic island world. You enter a contaminated zone as a disposable employee and most restore the power in the area.

##### 1. WORLD GENERATION
#
The structure of the world is divided into 4 equally sized **Regions**, each containing a number of **Islands**. The look and feel of every Region is driven by it's **Biome**, which is defined in data as a ScriptableObject.

Each biome has a list of parameters that control the characteristics of the islands. This includes the actual size, height and shape of each Island. (See Mesh Generation) In addition, the types of Spawnable objects that can appear on each island are defined by it's associated biome. In the images below, you can see the difference in the generated islands from two different biomes.

[INSERT PICTURE OF HELL BIOME]
Hell Biome (Jagged surface, Dead trees, Rocks)

[INSERT PICTURE OF SWAMP BIOME]
Swamp Biome (Flat, wavy surface, Shrubs)

##### 1. MESH GENERATION
#
The meshes for the islands themselves are generated at runtime from the data-driven parameters of it's associated biome. The central method behind creating this mesh is first creating a height-map for the island. This is implemented as a 2D-array of floats that represent the relative height at any given coordinate of the island's relative space. In the image below, you can see what the generated texture of this height map looks like. 

![Island Texture Map](/Assets/Resources/Materials/MenuIslandMat/MenuIslandTexture.png)
Topographic Map of an Island

An interesting challenge with this project was creating topographic maps of islands that each felt unique. To achieve this, I started by creating a smoothed, noise map - essentially setting each value randomly and smoothing nearby values. This resulted in a noisy, rolling landscape. Not particularly interesting. 

I expanded on this by adding a falloff map, so that the height values towards the edges of each map would approach 0. Assuming that a height=0 is "underwater", the height maps started to appear like islands. However, each Island still felt generally the "same", since they were all being created the same way (from random noise). To make each one more unique, I implemented a point-based approach, where for each map, an array of coordinates act as height modifiers. Essentially, these points are like mountain peaks, and all surrounding points are pulled up based on their respective distance to peak.
