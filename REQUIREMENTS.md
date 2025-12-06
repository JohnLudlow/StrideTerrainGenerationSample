# Terrain Generation Library Requirements

## Project Overview
A high-performance terrain generation library for the Stride game engine, specifically designed for strategy games. This library leverages the Stride Community Toolkit to provide efficient and flexible terrain generation capabilities.

## Target Platform
- **Engine**: Stride Game Engine
- **Framework**: .NET 10.0+
- **Dependencies**: Stride Community Toolkit
- **Target Genre**: 4x Strategy Games

## Core Requirements

### 0. Coding Requirements
- Use the most up to date versions of .NET 10 and the Stride Community Toolkit
- Use Stride Community Toolkit for a code-first approach
- Use current features of those frameworks
  - Use the .slnx solution format rather than .sln
  - Ask if you're not sure how to create a .slnx
- Unit Testing
  - Use xUnit
  - Aim for a high level of test coverage
- Respect the guidelines set by .editorconfig 
  - Prefer `var`
  - Prefer modern C#
  - Prefer non-nullability and immutability
  - Prefer properties over public members
- Make use of the Entity Component Systems pattern that Stride provides

### 1. Terrain Generation
- **Procedural Generation**
  - Support for multiple noise algorithms (Perlin, Simplex, Worley)
  - Configurable terrain parameters (amplitude, frequency, octaves, lacunarity)
  - Seed-based generation for reproducible terrains
  - Multi-layered noise composition
  - Support for preconfigured patterns (e.g 2 continents) which can be combined 
    with noise patterns via layers
  - Consider the following algorithms:
    - Voronoi tessellation
    - Wave function collapse

- **Terrain Types**
  - Heightmap-based terrain
  - Configurable terrain resolution and scale
  - Support for various terrain sizes (small to large-scale maps)
  - LOD (Level of Detail) support for performance optimization

### 2. Biome System
- **Biome Definition**
  - Multiple biome types (grassland, desert, forest, mountain, water)
  - Hellscape: Blasted, ruined terrain with smoking ash pits, rivers of lava, and demonic features
  - Temperature and moisture-based biome distribution
  - Smooth biome transitions and blending
  - Customizable biome parameters

- **Terrain Features**
  - Height-based terrain coloring
  - Texture splatting based on slope and height
  - Support for biome-specific vegetation placement zones

### 3. Performance Requirements
- **Optimization**
  - Efficient mesh generation for real-time rendering
  - Chunk-based terrain loading/unloading
  - Multi-threaded terrain generation where applicable
  - Memory-efficient data structures

- **Strategy Game Specific**
  - Fast terrain queries for pathfinding
  - Grid-aligned terrain data for tactical gameplay
  - Support for terrain modification (leveling, raising, lowering)
  - Tile-based overlay support

### 4. Integration
- **Stride Engine Integration**
  - Seamless integration with Stride's rendering pipeline
  - Compatible with Stride's material system
  - Support for Stride's physics system
  - Entity Component System (ECS) friendly architecture

- **Stride Community Toolkit**
  - Utilize toolkit utilities for common operations
  - Follow toolkit patterns and best practices
  - Leverage toolkit extensions where applicable

### 5. Customization
- **Configuration**
  - JSON-based terrain configuration files
  - Runtime parameter adjustment
  - Preset terrain profiles (flat, hilly, mountainous, islands)
  - Custom terrain modification API

- **Extensibility**
  - Plugin architecture for custom generators
  - Interface-based design for algorithm swapping
  - Event system for terrain generation stages
  - Support for custom post-processing effects

### 6. Developer Experience
- **API Design**
  - Clean, intuitive API
  - Comprehensive XML documentation
  - Example scenes and usage samples
  - Unit tests for core functionality

- **Debugging Tools**
  - Visual debugging of noise functions
  - Terrain generation preview
  - Performance profiling hooks
  - Validation of terrain data

### 7. User Interface
- **Terrain Generation Options**
  - A suitable terrain generation options screen
  - Allow the user to select from a number of map types
    - N Continents (user-configurable: dropdown selection between 2-5 continents)
    - Islands
    - Pangea (a single continent)

  - Allow the user to select the presence and prevalence of multiple terrain types
    - Desert
    - Frozen
    - Forest
    - Rivers and lakes
    - Mountains
    - Plains
    - Hellscape

- **Debugging Console**
  - A troubleshooting UI that shows relevant facts about the system
  - A performance monitoring UI that shows performance stats as the system operates
  - A debugging console that allows the user to query the system and run simple commands

- **Camera Controls**
  - Use an orbital camera, orbiting about the projected mouse position with the 
    middle mouse button
  - WASD should move and pan the camera
  - The mouse wheel should zoom in and out

## Non-Functional Requirements

### Performance Targets
- Generate 512x512 terrain chunk in < 100ms on mid-range hardware
- Support minimum 60 FPS with 4 active terrain chunks
- Memory usage < 500MB for large maps (2048x2048)
- Generate a benchmark project using BenchmarkDotNet
- Source generator for benchmarking unit tests deferred for future consideration

### Code Quality
- Minimum 70% code coverage with unit tests
- Follow C# coding conventions and best practices
- Use async/await for long-running operations
- Proper error handling and logging
  - Use [Compile-time logging source generation](https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator)
  - Use [.NET metrics](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/metrics)
  - Tracy integration deferred for future consideration

### Documentation
- API documentation for all public members
- Getting started guide
- Advanced usage examples
- Performance tuning guide
- "How it works" internal documentation
- Document things I don't understand
  - Use of shaders
  - The materials and other properties attached to entities and components
  - Implementation of the user interface

## Future Considerations
- Real-time terrain deformation
- Water simulation and erosion
- Cave and underground structure generation
- Multiplayer-synchronized terrain generation
- Export/import functionality for external tools
- GPU-accelerated terrain generation

## Attribution
- Do not use any artwork without attribution
- If artwork is required, use a suitable placeholder colour
- Create an attributions list for any prior art used
- Create a placeholder list for anything that needs artwork provided

# Notes
- Ask for clarification where needed
- Both Visual Studio and Visual Studio Code are available and should be supported
- Visual Studio may be useful for its powerful profiling and debugging capabilities
- Implementation and instructions must work correctly in both IDEs