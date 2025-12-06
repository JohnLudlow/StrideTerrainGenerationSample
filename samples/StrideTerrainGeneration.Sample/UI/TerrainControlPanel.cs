using System.Globalization;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using StrideTerrainGeneration.Core.Configuration;
using StrideTerrainGeneration.Core.Noise;
using StrideTerrainGeneration.Sample.Components;

namespace StrideTerrainGeneration.Sample.UI;

/// <summary>
/// UI component for terrain generation controls.
/// </summary>
public class TerrainControlPanel : StartupScript
{
    public SpriteFont Font { get; set; } = null!;
    public LayerManagerPanel? LayerManagerPanel { get; set; }

    private TerrainComponent? _terrainComponent;
    private TextBlock? _seedText;
    private TextBlock? _widthText;
    private TextBlock? _heightText;
    private TextBlock? _noiseScaleText;
    private TextBlock? _heightScaleText;

    private int _seed = 12345;
    private int _terrainWidth = 64;
    private int _terrainHeight = 64;
    private float _noiseScale = 0.05f;
    private float _heightScale = 5.0f;
    private TerrainGeneratorType _generatorType = TerrainGeneratorType.Perlin;

    public override void Start()
    {
        var uiComponent = Entity.Get<UIComponent>();
        if (uiComponent == null) return;

        uiComponent.Page = new UIPage { RootElement = CreateUI() };
    }

    private UIElement CreateUI()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 10, 10, 10),
            BackgroundColor = new Color(0, 0, 0, 180)
        };

        // Title
        panel.Children.Add(new TextBlock
        {
            Text = "Terrain Generator",
            Font = Font,
            TextSize = 20,
            TextColor = Color.White,
            Margin = new Thickness(5, 5, 5, 5)
        });

        // Generator Type
        panel.Children.Add(CreateLabel("Generator Type:"));
        var generatorButtons = new UniformGrid
        {
            Rows = 1,
            Columns = 3,
            Margin = new Thickness(5, 0, 5, 10)
        };

        var flatButton = CreateButton("Flat", () => SetGeneratorType(TerrainGeneratorType.Flat));
        var perlinButton = CreateButton("Perlin", () => SetGeneratorType(TerrainGeneratorType.Perlin));
        var layeredButton = CreateButton("Layered", () => SetGeneratorType(TerrainGeneratorType.Layered));
        
        generatorButtons.Children.Add(flatButton);
        generatorButtons.Children.Add(perlinButton);
        generatorButtons.Children.Add(layeredButton);
        panel.Children.Add(generatorButtons);

        // Seed
        panel.Children.Add(CreateLabel("Seed:"));
        _seedText = CreateValueLabel(_seed.ToString(CultureInfo.InvariantCulture));
        panel.Children.Add(_seedText);
        panel.Children.Add(CreateSliderWithButtons(
            v => { _seed += (int)v; UpdateSeedText(); },
            -1000, 1000));

        // Width
        panel.Children.Add(CreateLabel("Width:"));
        _widthText = CreateValueLabel(_terrainWidth.ToString(CultureInfo.InvariantCulture));
        panel.Children.Add(_widthText);
        panel.Children.Add(CreateSliderWithButtons(
            v => { _terrainWidth += (int)v; UpdateWidthText(); },
            -16, 16));

        // Height
        panel.Children.Add(CreateLabel("Height:"));
        _heightText = CreateValueLabel(_terrainHeight.ToString(CultureInfo.InvariantCulture));
        panel.Children.Add(_heightText);
        panel.Children.Add(CreateSliderWithButtons(
            v => { _terrainHeight += (int)v; UpdateHeightText(); },
            -16, 16));

        // Noise Scale (for Perlin)
        panel.Children.Add(CreateLabel("Noise Scale:"));
        _noiseScaleText = CreateValueLabel(_noiseScale.ToString("F3", CultureInfo.InvariantCulture));
        panel.Children.Add(_noiseScaleText);
        panel.Children.Add(CreateSliderWithButtons(
            v => { _noiseScale += v; UpdateNoiseScaleText(); },
            -0.01f, 0.01f));

        // Height Scale
        panel.Children.Add(CreateLabel("Height Scale:"));
        _heightScaleText = CreateValueLabel(_heightScale.ToString("F1", CultureInfo.InvariantCulture));
        panel.Children.Add(_heightScaleText);
        panel.Children.Add(CreateSliderWithButtons(
            v => { _heightScale += v; UpdateHeightScaleText(); },
            -1f, 1f));

        // Generate Button
        var generateButton = CreateButton("Generate Terrain", GenerateTerrain);
        generateButton.Margin = new Thickness(5, 10, 5, 5);
        panel.Children.Add(generateButton);

        // Randomize Button
        var randomizeButton = CreateButton("Randomize Seed", RandomizeSeed);
        randomizeButton.Margin = new Thickness(5, 5, 5, 5);
        panel.Children.Add(randomizeButton);

        return panel;
    }

    private TextBlock CreateLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            Font = Font,
            TextSize = 14,
            TextColor = Color.White,
            Margin = new Thickness(5, 10, 5, 2)
        };
    }

    private TextBlock CreateValueLabel(string text)
    {
        return new TextBlock
        {
            Text = text,
            Font = Font,
            TextSize = 14,
            TextColor = Color.Yellow,
            Margin = new Thickness(5, 2, 5, 2)
        };
    }

    private Button CreateButton(string text, Action onClick)
    {
        var button = new Button
        {
            Content = new TextBlock
            {
                Text = text,
                Font = Font,
                TextSize = 14,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            Padding = new Thickness(10, 5, 10, 5),
            BackgroundColor = new Color(60, 60, 60, 255)
        };

        button.Click += (sender, args) => onClick();
        return button;
    }

    private UniformGrid CreateSliderWithButtons(Action<float> onChange, float smallStep, float largeStep)
    {
        var grid = new UniformGrid
        {
            Rows = 1,
            Columns = 3,
            Margin = new Thickness(5, 0, 5, 5)
        };

        var decreaseButton = CreateButton("−", () =>
        {
            onChange(smallStep);
        });

        decreaseButton.SetGridColumn(0);

        var spacer = new TextBlock(); // Spacer
        spacer.SetGridColumn(1);

        var increaseButton = CreateButton("+", () =>
        {
            onChange(largeStep);
        });

        increaseButton.SetGridColumn(2);

        grid.Children.Add(decreaseButton);
        grid.Children.Add(spacer);
        grid.Children.Add(increaseButton);

        return grid;
    }

    private void SetGeneratorType(TerrainGeneratorType type)
    {
        _generatorType = type;
        GenerateTerrain();
        
        // Refresh layer manager UI if switching to Layered mode
        if (type == TerrainGeneratorType.Layered)
        {
            LayerManagerPanel?.RefreshUI();
        }
    }

    private void UpdateSeedText()
    {
        _seedText?.Text = _seed.ToString(CultureInfo.InvariantCulture);
    }

    private void UpdateWidthText()
    {
        _widthText?.Text = _terrainWidth.ToString(CultureInfo.InvariantCulture);
    }

    private void UpdateHeightText()
    {
        _heightText?.Text = _terrainHeight.ToString(CultureInfo.InvariantCulture);
    }

    private void UpdateNoiseScaleText()
    {
        _noiseScaleText?.Text = _noiseScale.ToString("F3", CultureInfo.InvariantCulture);
    }

    private void UpdateHeightScaleText()
    {
        _heightScaleText?.Text = _heightScale.ToString("F1", CultureInfo.InvariantCulture);
    }

    private void RandomizeSeed()
    {
        _seed = Random.Shared.Next(1, 99999);
        UpdateSeedText();
        GenerateTerrain();
    }

    private void GenerateTerrain()
    {
        // Find terrain component in scene
        if (_terrainComponent == null)
        {
            var terrainEntity = SceneSystem.SceneInstance.RootScene.Entities
                .FirstOrDefault(e => e.Get<TerrainComponent>() != null);
            _terrainComponent = terrainEntity?.Get<TerrainComponent>();
        }

        if (_terrainComponent == null) return;

        // Update terrain component settings
        _terrainComponent.GeneratorType = _generatorType;
        _terrainComponent.Configuration = new TerrainConfig
        {
            Width = _terrainWidth,
            Height = _terrainHeight,
            Seed = _seed
        };
        _terrainComponent.HeightScale = _heightScale;

        // Update Perlin-specific settings
        if (_generatorType == TerrainGeneratorType.Perlin)
        {
            _terrainComponent.NoiseScale = _noiseScale;
        }

        // Regenerate terrain
        _terrainComponent.RegenerateTerrain();
    }
}
