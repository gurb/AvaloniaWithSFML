using Avalonia.Controls;
using AvaloniaWithSFML.Sample.ViewModels;

namespace AvaloniaWithSFML.Sample.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        DataContext = new MainViewModel(this);
        InitializeComponent();
    }
}
