using NetworkService.Helpers.Display;
using NetworkService.Model;
using NetworkService.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetworkService.Views
{
    /// <summary>
    /// Interaction logic for NetworkDisplayView.xaml
    /// </summary>
    public partial class NetworkDisplayView : UserControl
    {
        private NetworkDisplayViewModel _viewModel;
        public NetworkDisplayView()
        {
            InitializeComponent();
            _viewModel = new NetworkDisplayViewModel(MainWindowViewModel.Entities);
            this.DataContext = _viewModel;
            Loaded += (_, __) => Keyboard.Focus(this);
            Focus();
        }
        
        private void Canvas_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            if (sender is Canvas canvas && canvas.Tag is DisplayItem item)
            {
                e.Effects = item.IsTaken ? DragDropEffects.None : DragDropEffects.Move;
            }
            e.Handled = true;
        }

        private void Canvas_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Keyboard.Focus(this);
            if (sender is Canvas canvas && canvas.Tag is DisplayItem item)
            {
                int idx = _viewModel.Slots.IndexOf(item);
                if (idx >= 0)
                {
                    _viewModel.MoveEntityCommand.Execute(idx);
                }
            }
            e.Handled = true;
        }

        private void TreeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _viewModel.ResetDrag();
        }

        private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            if (TreeView.SelectedItem is Entity entity)
            {
                _viewModel.BeginDragFromTree(entity);
                DragDrop.DoDragDrop(this, entity, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            Keyboard.Focus(this);
            if (_viewModel.IsDraggingFromSlot)
            {
                _viewModel.RemoveEntityCommand.Execute();
            }
            e.Handled = true;
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = _viewModel.IsDraggingFromSlot ? DragDropEffects.Move : DragDropEffects.None;
            e.Handled = true;
        }

        private void Canvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Canvas canvas && canvas.Tag is DisplayItem item && item.IsTaken)
            {
                int idx = _viewModel.Slots.IndexOf(item);
                if (idx < 0) return;

                _viewModel.BeginDragFromSlot(idx);
                DragDrop.DoDragDrop(this, item, DragDropEffects.Move | DragDropEffects.Copy);
            }
            e.Handled = true;
        }

        private void Canvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(sender is Canvas canvas && canvas.Tag is DisplayItem item && item.IsTaken) 
            {
                int idx = _viewModel.Slots.IndexOf(item);
                if(idx < 0) return;

                _viewModel.ConnectCommand.Execute(idx);
            }
            e.Handled = true;
        }
    }
}
