using System.Globalization;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Graphics;
using Stride.UI;
using Stride.UI.Controls;
using Stride.UI.Panels;
using StrideTerrainGeneration.Core.Noise;
using StrideTerrainGeneration.Sample.Components;

namespace StrideTerrainGeneration.Sample.UI;

/// <summary>
/// UI component for managing noise layers in layered terrain generation.
/// </summary>
public class LayerManagerPanel : StartupScript
{
    public SpriteFont Font { get; set; } = null!;
    public TerrainComponent? TerrainComponent { get; set; }

    private ScrollViewer? _layersScrollViewer;
    private StackPanel? _layersPanel;
    private int _selectedLayerIndex = -1; // Reserved for future use

    public override void Start()
    {
        var uiComponent = Entity.Get<UIComponent>();
        if (uiComponent == null) return;

        uiComponent.Page = new UIPage { RootElement = CreateUI() };
    }

    private UIElement CreateUI()
    {
        var mainPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(10, 10, 10, 10),
            BackgroundColor = new Color(0, 0, 0, 180),
            Width = 300
        };

        // Title
        mainPanel.Children.Add(new TextBlock
        {
            Text = "Noise Layers",
            Font = Font,
            TextSize = 18,
            TextColor = Color.White,
            Margin = new Thickness(5, 5, 5, 10)
        });

        // Layers list
        _layersPanel = new StackPanel
        {
            Orientation = Orientation.Vertical
        };

        _layersScrollViewer = new ScrollViewer
        {
            Content = _layersPanel,
            ScrollMode = ScrollingMode.Vertical,
            Height = 300,
            Margin = new Thickness(5, 5, 5, 5)
        };
        mainPanel.Children.Add(_layersScrollViewer);

        // Add Layer Button
        var addLayerButton = CreateButton("Add Layer", AddNewLayer);
        addLayerButton.Margin = new Thickness(5, 5, 5, 5);
        mainPanel.Children.Add(addLayerButton);

        RefreshLayersList();

        return mainPanel;
    }

    /// <summary>
    /// Public method to refresh the UI from external callers.
    /// </summary>
    public void RefreshUI()
    {
        RefreshLayersList();
    }

    private void RefreshLayersList()
    {
        if (_layersPanel == null || TerrainComponent?.LayeredNoiseGenerator == null)
            return;

        _layersPanel.Children.Clear();

        var layers = TerrainComponent.LayeredNoiseGenerator.Layers;
        for (int i = 0; i < layers.Count; i++)
        {
            _layersPanel.Children.Add(CreateLayerPanel(layers[i], i));
        }
    }

    private UIElement CreateLayerPanel(NoiseLayer layer, int index)
    {
        var layerPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(5, 2, 5, 2),
            BackgroundColor = new Color(40, 40, 40, 255)
        };

        // Header with name and controls
        var headerGrid = new UniformGrid
        {
            Rows = 1,
            Columns = 3,
            Margin = new Thickness(0, 0, 0, 5)
        };

        headerGrid.Children.Add(new TextBlock
        {
            Text = layer.Name,
            Font = Font,
            TextSize = 14,
            TextColor = layer.Enabled ? Color.Yellow : Color.Gray
        });

        var enableButton = CreateSmallButton(layer.Enabled ? "✓" : "✗",
            () => ToggleLayer(index));
        headerGrid.Children.Add(enableButton);

        var removeButton = CreateSmallButton("✕", () => RemoveLayer(index));
        headerGrid.Children.Add(removeButton);

        layerPanel.Children.Add(headerGrid);

        // Layer details (collapsed by default, could expand on selection)
        var detailsText = $"{layer.NoiseType} | {layer.Operation}\n" +
                         $"Scale: {layer.Scale:F3} | Amp: {layer.Amplitude:F2}";
        
        layerPanel.Children.Add(new TextBlock
        {
            Text = detailsText,
            Font = Font,
            TextSize = 10,
            TextColor = Color.LightGray,
            WrapText = true
        });

        return layerPanel;
    }

    private Button CreateButton(string text, Action onClick)
    {
        var button = new Button
        {
            Content = new TextBlock
            {
                Text = text,
                Font = Font,
                TextSize = 12,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            Padding = new Thickness(8, 4, 8, 4),
            BackgroundColor = new Color(60, 60, 60, 255)
        };

        button.Click += (sender, args) => onClick();
        return button;
    }

    private Button CreateSmallButton(string text, Action onClick)
    {
        var button = new Button
        {
            Content = new TextBlock
            {
                Text = text,
                Font = Font,
                TextSize = 12,
                TextColor = Color.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            },
            Padding = new Thickness(4, 2, 4, 2),
            BackgroundColor = new Color(60, 60, 60, 255)
        };

        button.Click += (sender, args) => onClick();
        return button;
    }

    private void AddNewLayer()
    {
        if (TerrainComponent?.LayeredNoiseGenerator == null)
            return;

        var newLayer = new NoiseLayer
        {
            Name = $"Layer {TerrainComponent.LayeredNoiseGenerator.Layers.Count + 1}",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.1f,
            Amplitude = 0.5f,
            Octaves = 2,
            Persistence = 0.5f,
            Lacunarity = 2.0f,
            Enabled = true
        };

        TerrainComponent.LayeredNoiseGenerator.AddLayer(newLayer);
        RefreshLayersList();
        TerrainComponent.RegenerateTerrain();
    }

    private void RemoveLayer(int index)
    {
        if (TerrainComponent?.LayeredNoiseGenerator == null)
            return;

        TerrainComponent.LayeredNoiseGenerator.RemoveLayerAt(index);
        RefreshLayersList();
        TerrainComponent.RegenerateTerrain();
    }

    private void ToggleLayer(int index)
    {
        if (TerrainComponent?.LayeredNoiseGenerator == null)
            return;

        var layers = TerrainComponent.LayeredNoiseGenerator.Layers;
        if (index >= 0 && index < layers.Count)
        {
            var layer = layers[index];
            layer.Enabled = !layer.Enabled;
            RefreshLayersList();
            TerrainComponent.RegenerateTerrain();
        }
    }
}
