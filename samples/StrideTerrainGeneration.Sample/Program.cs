using Stride.CommunityToolkit.Engine;
using Stride.CommunityToolkit.Games;
using Stride.CommunityToolkit.Rendering.Compositing;
using Stride.CommunityToolkit.Skyboxes;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Lights;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using StrideTerrainGeneration.Core.Configuration;
using StrideTerrainGeneration.Sample.Components;
using StrideTerrainGeneration.Sample.UI;

namespace StrideTerrainGeneration.Sample;

internal sealed class Program
{
    private static void Main()
    {
        var game = new Game();
        
        game.Run(start: rootScene =>
        {
            game.AddGraphicsCompositor().AddCleanUIStage();
            game.Add3DCamera().Add3DCameraController();
            
            // Add directional light with shadows enabled
            var mainLight = game.AddDirectionalLight();
            var mainLightComponent = mainLight.Get<LightComponent>();
            if (mainLightComponent != null)
            {
                mainLightComponent.Intensity = 1.5f;
                if (mainLightComponent.Type is LightDirectional directional)
                {
                    directional.Shadow.Enabled = true;
                    directional.Shadow.Size = LightShadowMapSize.Large;
                    directional.Shadow.CascadeCount = LightShadowMapCascadeCount.FourCascades;
                    directional.Shadow.StabilizationMode = LightShadowMapStabilizationMode.ProjectionSnapping;
                    directional.Shadow.BiasParameters.DepthBias = 0.001f;
                }
            }
            
            game.AddSkybox();
            
            game.AddProfiler();
            game.SetMaxFPS(60);

            var primitive = game.Create3DPrimitive(
                Stride.CommunityToolkit.Rendering.ProceduralModels.PrimitiveModelType.Cube, 
                new Primitive3DEntityOptions { Material = game.CreateMaterial(Color.Red) }
            );

            primitive.Scene = rootScene;
            primitive.Transform.Position = new Vector3(0, 1, 0);

            // Create terrain entity - positioned to be visible alongside the cube
            var terrainEntity = new Entity("Terrain")
            {
                new TerrainComponent
                {
                    GeneratorType = TerrainGeneratorType.Perlin, // Try Perlin noise terrain
                    Configuration = new TerrainConfig
                    {
                        Width = 64,
                        Height = 64,
                        BaseHeight = 0.5f,
                        Seed = 12345
                    },
                    MeshScale = 0.2f,
                    HeightScale = 5.0f // Increased for visible Perlin variation
                },
                new ModelComponent()
            };

            terrainEntity.Scene = rootScene;
            terrainEntity.Transform.Position = new Vector3(0, 0, 5); // Move terrain 5 units in front of cube
            
            System.Diagnostics.Debug.WriteLine($"Terrain entity created at position: {terrainEntity.Transform.Position}");

            // Add UI for terrain controls
            var font = game.Content.Load<SpriteFont>("StrideDefaultFont");
            
            var terrainControlPanel = new TerrainControlPanel { Font = font };
            var uiEntity = new Entity("TerrainUI")
            {
                new UIComponent
                {
                    Page = null, // Will be set by TerrainControlPanel
                    Resolution = new Vector3(1920, 1080, 500),
                    IsFullScreen = true,
                    RenderGroup = RenderGroup.Group31
                },
                terrainControlPanel
            };
            uiEntity.Scene = rootScene;

            // Add layer manager UI (right side)
            var layerManagerPanel = new LayerManagerPanel 
            { 
                Font = font,
                TerrainComponent = terrainEntity.Get<TerrainComponent>()
            };
            
            var layerUIEntity = new Entity("LayerManagerUI")
            {
                new UIComponent
                {
                    Page = null, // Will be set by LayerManagerPanel
                    Resolution = new Vector3(1920, 1080, 500),
                    IsFullScreen = true,
                    RenderGroup = RenderGroup.Group31
                },
                layerManagerPanel
            };
            layerUIEntity.Scene = rootScene;
            
            // Connect the panels
            terrainControlPanel.LayerManagerPanel = layerManagerPanel;
        });
    }
}

