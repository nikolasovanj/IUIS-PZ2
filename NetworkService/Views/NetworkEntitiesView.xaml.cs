using NetworkService.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for NetworkEntitiesView.xaml
    /// </summary>
    public partial class NetworkEntitiesView : UserControl
    {
        public NetworkEntitiesView()
        {
            InitializeComponent();
            this.DataContext = new NetworkEntitiesViewModel(MainWindowViewModel.Entities);

            Loaded += (_, __) => Keyboard.Focus(this);
            Focus();
        }
    }
}
