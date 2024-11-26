using Avalonia.Controls;
using Avalonia.VisualTree;
using AvaloniaWithSFML.Sample.ViewModels;

namespace AvaloniaWithSFML.Sample.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        var root = this.GetVisualRoot();
        if (root is Avalonia.Controls.Window)
        {
            DataContext = new MainViewModel((Avalonia.Controls.Window)root);
        }
        InitializeComponent();
    }

    private void Slider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {

    }
}
