using NetworkService.ViewModel;
using System.Windows.Controls;

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for MeasurementGraphView.xaml
    /// </summary>
    public partial class MeasurementGraphView : UserControl
    {
        public MeasurementGraphView()
        {
            InitializeComponent();
            this.DataContext = new MeasurementGraphViewModel(MainWindowViewModel.Entities);
        }
    }
}
