using Avalonia.VisualTree;
using ReactiveUI;
using SFML.Window;

namespace AvaloniaWithSFML.Sample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public SFMLGame? CurrentGame { get; set; }
    public SFMLGame? TestGame { get; set; }

    public float MinZoom { get; set; } = 0.0f;
    public float MaxZoom { get; set; } = 1.0f;

    private float zoom = 1.0f;

    public float Zoom
    {
        get => zoom;
        set
        { 
            this.RaiseAndSetIfChanged(ref zoom, value);
            ChangeZoom();
        }
    }

    public float Rotation
    {
        get => CurrentGame != null ? CurrentGame.Rotation : 0.0f;
        set
        {
            if(CurrentGame is not null)
                CurrentGame.Rotation = value;
        }
    }

    public void ChangeZoom()
    {
        if (CurrentGame is not null)
        {
            CurrentGame.ChangeZoom(zoom);
        }
    }

    public MainViewModel(Avalonia.Controls.Window window)
    {
        var root = window.GetVisualRoot();
        if(root is Avalonia.Controls.Window)
        {
            CurrentGame = new SFMLGame((Avalonia.Controls.Window)root, new ContextSettings { AntialiasingLevel = 8 }, 1024, 720);
        }
    }
}
