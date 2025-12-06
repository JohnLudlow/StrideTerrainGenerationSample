using Stride.CommunityToolkit.Engine;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideTerrainGeneration.Sample;

internal class Program
{
    private static void Main()
    {
        var game = new Game();
        
        game.Run(start: (Scene rootScene) =>
        {
            game.SetupBase3D();
            
            // TODO: Initialize terrain generation system
            // TODO: Setup UI for terrain generation options
            // TODO: Configure camera controls (orbital, WASD, mouse wheel)
        });
    }
}
