# Flyweight Design Pattern


## Summary
Games can sometimes have __MANY__ objects
> In Minecraft, there are millions of cubes in the scene

We can have all of our objects share data that is identical between them
> In Minecraft, all blocks can share the same texture if they belong to a [Texture Atlas](https://en.wikipedia.org/wiki/Texture_atlas)

> Objects sharing the same mesh and material implement this pattern using [GPU Instancing](https://docs.unity3d.com/540/Documentation/Manual/GPUInstancing.html)


## When To Use
- If creating a massive number of objects with similar properties
> Mesh, Textures, Name, Item ID, Max Health, etc.


## Implementation
### General
- Create a new class and put the shared data in it
> We'll call it `Shared Data` for this example
- Each created object should get a reference to a single instance of `Shared Data`
> If more objects will be created during gameplay, store the instance of `Shared Data` somewhere

### Unity
- `Scriptable Objects`
> Follow the same steps as above, but have `Shared Data` inherit from ScriptableObject

> This will allow you to edit variables in your Assets folder
