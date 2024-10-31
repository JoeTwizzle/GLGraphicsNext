namespace Example;

using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Platform;

//Adaptation of the Opentk 5 Tutorial
internal class Program
{
    static void Main()
    {
        var game = new ExampleGame();
        game.Run();
    }
}
