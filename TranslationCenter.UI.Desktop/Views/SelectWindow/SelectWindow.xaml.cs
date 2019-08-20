using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            model.SetSelectedItems(lstItems.SelectedItems);
            this.DialogResult = true;
            this.Close();
        }
    }
}
