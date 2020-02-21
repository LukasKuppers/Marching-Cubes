# Marching-Cubes

Marching Cubes algorithm implemented in Unity. The marching Cubes algorithm is used to create 3D iso-surfaces that model density fields, And is commonly used to generate procedurally generated terrain.

### Usage

To use this implementation just download the files and import them into a new project. (note that for mesh colors to work, you'll need to set up a vertex shader, which can be done easily using shadergraph). Also keep in mind that this was developed in Unity 2018.2.8 so there could be compatability issues with newer versions of Unity.

#### To get the algorithm up and running: 
- Attatch a mesh filter, mesh renderer to an empty Gameobject. Also attatch the IsofieldGenerator and DensityFieldGenerator scripts.
- In order to properly render the mesh, you'll need to use a material that uses a vertex shader (as mentioned above)
- Thats it. When you click play, the marching cubes algorithm will start generating a mesh

#### Isofield Generator Interface:
- The current implementation generates the mesh in a cube of width "size", however the actual generateMesh function in the script supports a rectangle of any dimensions
- Resolution controls the size of cells within the mesh, so a lower value will yield higher detail (and worse performance)
- Threshold is the value in the density field where the surface of the mesh should lie
- Color grad is a gradient which determines the color of individual verticies based on their height

#### Density Field Generator Interface:
Any density field could be used here, but this script alows for some simple terrain generation over flat ground or as a planet

- Is Planet simply controls if a planet is being created or not, (if you are creating a planet, you'll need to increase the threshold in IsoField Generator in order to see anything)
- frequency determines the overall frequency of noise sampling (lower frequency creates smoother terrain, I like a value at around 0.05)
- Amplitude determines the amplitude (or strength) of noise sampling. A higher amplitude creates taller, steeper terrain
- Lacurnarity controls the rate at which frequency changes between octaves
- Persistence controls the rate at which amplitude changes between octaves, persistence and lacurnarity should be inversely proportional
- Floor and Ceil height can be used to create a general floor and ceiling level in the terrain (ceil should probably be greater than floor...)
- Num Octaves controls the number of octaves that are layered on eachother, more octaves means more detail

### Technical Details and recommendations

- The current implementation in IsoField Generator is recalculating the mesh every frame, so performance can take a hit pretty fast. If you're not planning on having changing terrain, this should definately be changed so that the mesh is only created at the start of the scene, in Start() for example. Performace can be increased by increased by increasing the resolution parameter (I know that sounds silly, but currently resolution is just being passed as the size of every cell). Also keep in mind that Unity has a max number of verticies that a mesh can support, and going beyond this number will surely cause some unwanted damage.

- The marching cubes algorithm uses lots of lookup tables, so your cache is going to get put under a lot of stress if the mesh is constantly being recalculated (say if this was being used in a game with terraforming)

- The algorithm is currently being run exclusively through the CPU. Since Marchaing Cubes can be pretty parallel in nature, it would massively boost performace to run this algorithm through the GPU, using a compute shader for example.

### Acknowlegements

The lookup tables and general information were sourced from [this paper](http://paulbourke.net/geometry/polygonise/)

Nvidia also provides nice documentation on creating interesting terrain in their [GPU Gems book](https://developer.nvidia.com/gpugems/gpugems3/part-i-geometry/chapter-1-generating-complex-procedural-terrains-using-gpu), which is available for free online!
