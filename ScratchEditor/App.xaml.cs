using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ScratchEditor.Annotations;
using ScratchEditor.ThemeHandling;

namespace ScratchEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : INotifyPropertyChanged
    {
        public App()
        {
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public Color BackgroundColor => PropertyManager.getColor(ColorIdentifier.Background);
        public Color TextColor => PropertyManager.getColor(ColorIdentifier.TilteText);
    }
}