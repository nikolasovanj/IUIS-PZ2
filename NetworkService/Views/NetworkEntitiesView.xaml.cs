using NetworkService.ViewModel;
using System.Windows.Controls;

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
        }
    }
}
