<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# StrideTerrainGeneration - AI Coding Agent Instructions

## Project Overview
A high-performance, procedural terrain generation system for Stride game engine, targeting 4x strategy games. Multi-project solution with Core library, Sample app with UI, Tests, and Benchmarks.

**Status**: Early development - project structure exists but core implementation pending  
**Target Framework**: .NET 10.0  
**Dependencies**: Stride.CommunityToolkit 1.0.0-preview.62, Stride.CommunityToolkit.Windows 1.0.0-preview.62  
**Solution Format**: `.slnx` (modern XML-based solution file)

## Multi-Project Structure

### Projects (to be created)
- **StrideTerrainGeneration.Core** - Core terrain generation library (✓ exists)
- **StrideTerrainGeneration.Sample** - Demo app with UI, camera controls, terrain preview
- **StrideTerrainGeneration.Tests** - xUnit test project (target 70%+ coverage)
- **StrideTerrainGeneration.Benchmarks** - BenchmarkDotNet performance testing

### Folder Organization
```
src/
  StrideTerrainGeneration.Core/       # Core algorithms & systems
    Noise/                             # Noise generators (Perlin, Simplex, Worley, Voronoi)
    Biomes/                            # Biome system & distribution
    Mesh/                              # Chunk-based mesh generation
    Configuration/                     # JSON config models
tests/
  StrideTerrainGeneration.Tests/      # Unit & integration tests
benchmarks/
  StrideTerrainGeneration.Benchmarks/ # Performance benchmarks
samples/
  StrideTerrainGeneration.Sample/     # Interactive demo with UI
```

## Architecture & Design Patterns

### Core Components (to be implemented in `src/StrideTerrainGeneration.Core/`)
- **Noise Generation**: Multi-algorithm support (Perlin, Simplex, Worley, Voronoi tessellation) with seed-based reproducibility; consider Wave Function Collapse for patterns
- **Biome System**: Temperature/moisture-based distribution with smooth transitions; support preconfigured continent patterns; includes Hellscape (blasted terrain with ash pits, lava rivers, demonic features)
- **Mesh Generation**: Chunk-based, LOD-aware terrain meshes for performance
- **Terrain Modification**: Runtime height/slope modification for strategy gameplay
- **Configuration System**: JSON-based presets and runtime parameter adjustment

### Design Principles
- **Interface-based extensibility**: Use interfaces for noise generators, biome providers, terrain modifiers to enable plugin architecture
- **Chunk-based processing**: All terrain operations work on configurable chunks for memory efficiency and streaming
- **Multi-threading ready**: Design for async/await and parallel terrain generation (use `Task.Run` for CPU-bound operations)
- **ECS-friendly**: Architecture should integrate with Stride's Entity Component System patterns
- **4x Strategy optimized**: Grid-aligned data, turn-based compatible, fast pathfinding queries, tile overlay support, terrain modification for terraforming
- **Code style**: Follow `.editorconfig` - prefer `var`, immutability, properties over fields, modern C# patterns

## Performance Requirements (Critical)

### Hard Targets
- 512x512 chunk generation: < 100ms on mid-range hardware
- Memory budget: < 500MB for 2048x2048 maps
- Minimum 60 FPS with 4 active chunks
- **Always profile**: Add performance hooks early in terrain generation pipeline

### Optimization Strategies
- Use `Span<T>` and `Memory<T>` for zero-copy buffer operations
- Implement object pooling for frequently allocated terrain data structures
- Consider SIMD operations (`System.Numerics.Vector`) for noise calculations
- Lazy evaluation where possible, cache expensive computations

## Stride Integration Specifics

### Required NuGet Packages (not yet added)
```xml
<PackageReference Include="Stride.CommunityToolkit" Version="1.0.0-preview.62" />
<PackageReference Include="Stride.CommunityToolkit.Windows" Version="1.0.0-preview.62" />
```

**Note**: Core project uses `Stride.CommunityToolkit`, Sample project uses `Stride.CommunityToolkit.Windows`

### Integration Patterns
- **Material System**: Terrain materials use Stride's shader composition; texture splatting via material layers
- **Mesh Creation**: Use `Stride.Graphics.GeometricPrimitive.Custom` or manual `VertexBuffer`/`IndexBuffer` creation
- **Physics**: Generate collision shapes using `Stride.Physics.ColliderShape.CreateStaticPlaneInfinite` or heightfield colliders
- **Rendering Pipeline**: Integrate with `RenderContext`, support for forward/deferred rendering paths

### Stride Community Toolkit Usage
- Leverage `CameraComponent` extensions for terrain LOD calculations
- Use toolkit's service registry patterns for dependency injection
- Follow toolkit's async loading patterns for chunk streaming

## Code Conventions

### Namespace Organization
- `StrideTerrainGeneration.Core` - Core algorithms (noise, generation)
- `StrideTerrainGeneration.Core.Noise` - Noise generators (Perlin, Simplex, Worley, Voronoi)
- `StrideTerrainGeneration.Core.Biomes` - Biome system
- `StrideTerrainGeneration.Core.Mesh` - Mesh generation
- `StrideTerrainGeneration.Core.Configuration` - JSON config models and loaders
- `StrideTerrainGeneration.Sample` - Demo application, UI components
- `StrideTerrainGeneration.Tests` - Test namespaces mirror source structure

### Naming Patterns
- Generators: `*Generator` (e.g., `PerlinNoiseGenerator`, `BiomeGenerator`)
- Providers: `*Provider` (e.g., `IBiomeProvider`, `INoiseProvider`)
- Configuration: `*Config` or `*Settings` (e.g., `TerrainConfig`, `BiomeSettings`)
- Async methods: Always suffix with `Async` (e.g., `GenerateChunkAsync`)

### Code Quality Standards
- **XML docs required**: All public APIs must have `<summary>`, `<param>`, `<returns>`
- **Nullable annotations**: Enabled project-wide, use `?` and null-forgiving `!` appropriately
- **Implicit usings**: Enabled (see generated `GlobalUsings.g.cs`)
- **Test coverage**: Target 70% minimum; use xUnit for unit tests
- **Logging**: Use compile-time logging source generation (LoggerMessage attribute)
- **Metrics**: Instrument performance-critical paths with .NET metrics API

## Development Workflow

### Build & Test
```powershell
# Build solution (.slnx format)
dotnet build StrideTerrainGeneration.slnx

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run benchmarks
dotnet run --project benchmarks/StrideTerrainGeneration.Benchmarks -c Release

# Build specific configuration for Stride
dotnet build -c Release
```

### Common Tasks
- **Adding Stride packages**: Update `StrideTerrainGeneration.Core.csproj`, restore with `dotnet restore`
- **Creating new components**: Follow namespace conventions, implement interfaces first
- **Testing noise generators**: Create visual debug output (export heightmaps as images)
- **Profiling**: Use BenchmarkDotNet for micro-benchmarks, Stride's profiler for runtime performance

## Key Reference Files
- `REQUIREMENTS.md` - Comprehensive feature specifications, performance targets, API requirements, UI specifications
- `.editorconfig` - Code style rules (var preference, immutability, modern C#)
- `StrideTerrainGeneration.slnx` - Modern XML-based solution file
- `src/StrideTerrainGeneration.Core/StrideTerrainGeneration.Core.csproj` - Core library project configuration

## Critical Context for AI Agents

### What's NOT implemented yet
- Any concrete terrain generation logic (only `Class1.cs` placeholder exists)
- Sample project UI implementation (terrain generation options, debugging console, camera controls)
- Test implementations
- Benchmark implementations
- Configuration JSON schemas

### Priority Implementation Order (from REQUIREMENTS.md)
1. **Project scaffolding**: Create Tests, Benchmarks, Sample projects and add to .slnx
2. **Core noise generation**: Implement Perlin, Simplex, Worley, Voronoi with seed support
3. **Heightmap terrain mesh**: Basic chunk-based mesh generation
4. **Chunk loading system**: LOD-aware terrain streaming
5. **Biome distribution**: Temperature/moisture system with smooth transitions
6. **Sample UI**: Terrain generation options, map type selection (N Continents with dropdown 2-5, Islands, Pangea); terrain type prevalence controls (Desert, Frozen, Forest, Rivers/Lakes, Mountains, Plains, Hellscape)
7. **Camera controls**: Orbital camera with WASD + mouse wheel in Sample project
8. **Configuration system**: JSON serialization and preset profiles
9. **Debugging tools**: Visual noise preview, performance monitoring UI

### Breaking Changes to Avoid
- Changing chunk size after serialization format is defined
- Modifying noise algorithm outputs (breaks save compatibility)
- Altering public API surfaces without deprecation path

### Deferred Features (Future Consideration)
- Tracy profiler integration (C++ native interop complexity)
- Source generator for benchmarking unit tests (would enable reusing tests as benchmark exercises)

## IDE Support
- **Both Visual Studio and VS Code supported**: All code and instructions must work in both IDEs
- **Visual Studio advantages**: Advanced profiling, debugging, better performance analysis tools
- **VS Code advantages**: Lightweight, extensible, cross-platform
- **No IDE-specific dependencies**: Avoid features exclusive to one IDE unless explicitly required

## External Resources
- [Stride Documentation](https://doc.stride3d.net/)
- [Stride Community Toolkit GitHub](https://github.com/stride3d/stride-community-toolkit)
- See REQUIREMENTS.md sections 3-6 for detailed performance, integration, and customization specifications
