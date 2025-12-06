using Stride.CommunityToolkit.Rendering.Utilities;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Materials;
using Stride.Rendering.Materials.ComputeColors;
using StrideTerrainGeneration.Core;
using StrideTerrainGeneration.Core.Configuration;
using StrideTerrainGeneration.Core.Generators;
using StrideTerrainGeneration.Core.Noise;

namespace StrideTerrainGeneration.Sample.Components;

/// <summary>
/// Component that generates and displays terrain mesh.
/// </summary>
public class TerrainComponent : SyncScript
{
    private ITerrainGenerator? _generator;
    private TerrainData? _terrainData;
    private ModelComponent? _modelComponent;
    private LayeredTerrainGenerator? _layeredGenerator;

    /// <summary>
    /// Gets or sets the terrain generator type.
    /// </summary>
    public TerrainGeneratorType GeneratorType { get; set; } = TerrainGeneratorType.Flat;

    /// <summary>
    /// Gets or sets the noise scale for Perlin terrain.
    /// </summary>
    public float NoiseScale { get; set; } = 0.05f;

    /// <summary>
    /// Gets the layered noise generator for multi-layer terrain.
    /// </summary>
    public LayeredNoiseGenerator? LayeredNoiseGenerator => _layeredGenerator?.NoiseGenerator;

    /// <summary>
    /// Gets or sets the terrain configuration.
    /// </summary>
    public TerrainConfig Configuration { get; set; } = new();

    /// <summary>
    /// Gets or sets the scale factor for terrain mesh.
    /// </summary>
    public float MeshScale { get; set; } = 0.1f;

    /// <summary>
    /// Gets or sets the height multiplier for visualization.
    /// </summary>
    public float HeightScale { get; set; } = 10.0f;

    /// <summary>
    /// Gets or sets the terrain color.
    /// </summary>
    public Color GrassColor { get; set; } = new Color(34, 139, 34); // Forest green

    /// <summary>
    /// Gets or sets the rock/mountain color for high elevations.
    /// </summary>
    public Color RockColor { get; set; } = new Color(105, 105, 105); // Dark gray

    /// <summary>
    /// Gets or sets the height threshold for transitioning from grass to rock.
    /// </summary>
    public float RockHeightThreshold { get; set; } = 0.6f;

    public override void Start()
    {
        _generator = GeneratorType switch
        {
            TerrainGeneratorType.Flat => new FlatTerrainGenerator(),
            TerrainGeneratorType.Perlin => new PerlinTerrainGenerator
            {
                NoiseScale = this.NoiseScale,
                MinHeight = 0.0f,
                MaxHeight = 1.0f
            },
            TerrainGeneratorType.Layered => CreateLayeredGenerator(),
            _ => new FlatTerrainGenerator()
        };

        _modelComponent = Entity.Get<ModelComponent>();
        
        if (_modelComponent == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: ModelComponent not found on terrain entity!");
            return;
        }
        
        System.Diagnostics.Debug.WriteLine("TerrainComponent.Start() - ModelComponent found");

        GenerateTerrain();
        
        // Debug output
        if (_terrainData != null)
        {
            var maxX = _terrainData.Width * MeshScale;
            var maxZ = _terrainData.Height * MeshScale;
            var avgHeight = _terrainData.GetHeight(0, 0) * HeightScale;
            System.Diagnostics.Debug.WriteLine($"Terrain generated: Size ({maxX}, {avgHeight}, {maxZ})");
        }
    }

    public override void Update()
    {
        // Nothing to update for flat terrain
    }

    /// <summary>
    /// Regenerates the terrain with current configuration.
    /// </summary>
    public void RegenerateTerrain()
    {
        GenerateTerrain();
    }

    private LayeredTerrainGenerator CreateLayeredGenerator()
    {
        _layeredGenerator = new LayeredTerrainGenerator
        {
            MinHeight = 0.0f,
            MaxHeight = 1.0f
        };

        // Start with a default layer if none exist
        if (_layeredGenerator.NoiseGenerator.Layers.Count == 0)
        {
            _layeredGenerator.NoiseGenerator.AddLayer(new NoiseLayer
            {
                Name = "Base Terrain",
                NoiseType = NoiseType.Perlin,
                Operation = NoiseLayerOperation.Replace,
                Scale = 0.05f,
                Amplitude = 1.0f,
                Octaves = 4,
                Persistence = 0.5f,
                Lacunarity = 2.0f
            });
        }

        return _layeredGenerator;
    }

    /// <summary>
    /// Regenerates the terrain with current configuration.
    /// </summary>
    private void GenerateTerrain()
    {
        System.Diagnostics.Debug.WriteLine("GenerateTerrain() called");
        
        _terrainData = _generator?.Generate(Configuration);
        
        if (_terrainData == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: _terrainData is null after generation!");
            return;
        }
        
        if (_modelComponent == null)
        {
            System.Diagnostics.Debug.WriteLine("ERROR: _modelComponent is null in GenerateTerrain!");
            return;
        }
        
        var model = new Model
        {
            CreateTerrainMaterial(),
            CreateTerrainMesh(_terrainData)
        };

        _modelComponent.Model = model;
        
        System.Diagnostics.Debug.WriteLine($"Terrain model assigned, RenderGroup: {_modelComponent.RenderGroup}");
    }

    private MaterialInstance CreateTerrainMaterial()
    {
        // Create a PBR material with height-based color variation
        var materialDescription = new MaterialDescriptor
        {
            Attributes = new MaterialAttributes
            {
                MicroSurface = new MaterialGlossinessMapFeature
                {
                    GlossinessMap = new ComputeFloat(0.2f) // Less glossy for natural terrain
                },
                Diffuse = new MaterialDiffuseMapFeature
                {
                    // Use vertex color for height-based texture splatting
                    DiffuseMap = new ComputeVertexStreamColor()
                },
                DiffuseModel = new MaterialDiffuseLambertModelFeature(),
                Specular = new MaterialMetalnessMapFeature
                {
                    MetalnessMap = new ComputeFloat(0.0f) // Non-metallic
                },
                SpecularModel = new MaterialSpecularMicrofacetModelFeature
                {
                    Environment = new MaterialSpecularMicrofacetEnvironmentGGXPolynomial()
                }
            }
        };

        var material = Material.New(GraphicsDevice, materialDescription);
        
        System.Diagnostics.Debug.WriteLine($"Material created with vertex color splatting");
        return material;
    }

    private Mesh CreateTerrainMesh(TerrainData terrainData)
    {
        var width = terrainData.Width;
        var height = terrainData.Height;

        using var meshBuilder = new MeshBuilder();
        meshBuilder.WithIndexType(IndexingType.Int32);
        meshBuilder.WithPrimitiveType(PrimitiveType.TriangleList);
        
        var position = meshBuilder.WithPosition<Vector3>();
        var normal = meshBuilder.WithNormal<Vector3>();
        var texCoord = meshBuilder.WithTextureCoordinate<Vector2>();
        var color = meshBuilder.WithColor<Color>();

        // Find min/max heights for normalization
        var minHeight = float.MaxValue;
        var maxHeight = float.MinValue;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var h = terrainData.GetHeight(x, y);
                minHeight = Math.Min(minHeight, h);
                maxHeight = Math.Max(maxHeight, h);
            }
        }

        // Generate vertices with color based on height
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var heightValue = terrainData.GetHeight(x, y);
                
                // Normalize height to [0, 1]
                var normalizedHeight = maxHeight > minHeight 
                    ? (heightValue - minHeight) / (maxHeight - minHeight) 
                    : 0.5f;
                
                // Blend between grass and rock based on height
                var blendFactor = Math.Clamp((normalizedHeight - RockHeightThreshold) / 0.2f, 0f, 1f);
                var vertexColor = Color.Lerp(GrassColor, RockColor, blendFactor);
                
                meshBuilder.AddVertex();
                meshBuilder.SetElement(position, new Vector3(x * MeshScale, heightValue * HeightScale, y * MeshScale));
                meshBuilder.SetElement(normal, Vector3.UnitY);
                meshBuilder.SetElement(texCoord, new Vector2((float)x / width, (float)y / height));
                meshBuilder.SetElement(color, vertexColor);
            }
        }

        // Generate indices
        for (var y = 0; y < height - 1; y++)
        {
            for (var x = 0; x < width - 1; x++)
            {
                var topLeft = y * width + x;
                var topRight = topLeft + 1;
                var bottomLeft = (y + 1) * width + x;
                var bottomRight = bottomLeft + 1;

                // First triangle (counter-clockwise)
                meshBuilder.AddIndex(topLeft);
                meshBuilder.AddIndex(topRight);
                meshBuilder.AddIndex(bottomLeft);

                // Second triangle (counter-clockwise)
                meshBuilder.AddIndex(topRight);
                meshBuilder.AddIndex(bottomRight);
                meshBuilder.AddIndex(bottomLeft);
            }
        }

        var mesh = new Mesh
        {
            Draw = meshBuilder.ToMeshDraw(GraphicsDevice),
            MaterialIndex = 0
        };

        System.Diagnostics.Debug.WriteLine($"Mesh created with MeshBuilder: {width * height} vertices, {(width - 1) * (height - 1) * 6} indices");
        return mesh;
    }
}
