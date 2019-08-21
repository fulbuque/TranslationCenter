using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TranslationCenter.UI.Desktop.Views.SelectWindow
{
    /// <summary>
    /// Interaction logic for SelectWindow.xaml
    /// </summary>
    public partial class SelectWindow : Window
    {
        public SelectWindow()
        {
            InitializeComponent();
        }

        private ISelectWindowModel model
        {
            get
            {
                if (this.DataContext is ISelectWindowModel selectWindowModel)
                    return selectWindowModel;
                return default;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {

            if (!model.ValidateSelectedItems(lstItems.SelectedItems))
                return;

            model.SetSelectedItems(lstItems.SelectedItems);
            this.DialogResult = true;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.LeftCtrl)
                lstItems.SelectionMode = SelectionMode.Extended;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.LeftShift || e.Key == Key.LeftCtrl) && lstItems.SelectionMode == SelectionMode.Extended)
                lstItems.SelectionMode = SelectionMode.Multiple;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lstItems.Focus();
        }
    }
}