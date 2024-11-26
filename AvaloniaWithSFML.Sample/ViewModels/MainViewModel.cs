﻿using Avalonia.VisualTree;
using SFML.Window;

namespace AvaloniaWithSFML.Sample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public SFMLGame? CurrentGame { get; set; }
    public SFMLGame? TestGame { get; set; }


    public MainViewModel(Avalonia.Controls.Window window)
    {
        var root = window.GetVisualRoot();
        if(root is Avalonia.Controls.Window)
        {
            CurrentGame = new SFMLGame((Avalonia.Controls.Window)root, new ContextSettings { AntialiasingLevel = 8 });
        }
    }
}
