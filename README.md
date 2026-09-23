# OSK.Petra.Assets

The project provides a mechanism to handle asset retrieval, instantiation, caching, and more for game modules and game entities.

Core concepts:
- Module Assets - a game module represents an modular, isolated piece of a level (e.g. building) or an entire game level/scene where a load is required in the background. 
- Entity Assets - a game entity represents a game object that is meant to be created and instantiated within a module (e.g. swords, game blocks, characters, etc.).
- Descriptor - describe a module or entity to provide meaningful meta data to a game user within a UI needing to pick between multiple assets (e.g. level screen information, tech/build tree information, etc.)
- Instantiator - the core object that creates and handles generating a game entity during game/application runtime

The main point of entry is the `IAssetService`, and users can access it via DI after calling `AddAssets` on their container.

Several abstractions are generic to offer as much flexibility to integrating systems to utilize this logic while maintaining direct access to their own expected data and other properties.