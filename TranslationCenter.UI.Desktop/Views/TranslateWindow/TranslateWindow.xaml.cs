﻿using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace TranslationCenter.UI.Desktop.Views.TranslateWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TranslateWindow : Window
    {
        private TranslateWindowModel model = new TranslateWindowModel();

        public TranslateWindow()
        {
            InitializeComponent();
            this.DataContext = model;
            model.PropertyChanged += Model_PropertyChanged;
            NavigateToCurrentResult();
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(model.CurrentResult))
            {
                NavigateToCurrentResult();
            }
        }

        private void NavigateToCurrentResult()
        {
            var content = model.CurrentResult;
            if (string.IsNullOrWhiteSpace(content))
                content = "<HTML />";
            this.Dispatcher.Invoke(() => webBrowserResult.NavigateToString(content), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (e.Command == TranslateWindowCommands.SelectEngineCommand)
                model.SelectEngines(this);
            else if (e.Command == TranslateWindowCommands.SelectLanguageCommand)
                model.SelectLanguages(this);
            else if (e.Command == TranslateWindowCommands.SelectLanguageFrom)
                OpenLanguageFrom();
            else if (e.Command == TranslateWindowCommands.TranslateCommand)
                model.Translate();
            else if (e.Command == TranslateWindowCommands.NextLanguage)
                model.GoToNextLanguage();
            else if (e.Command == TranslateWindowCommands.PreviousLanguage)
                model.GoToPreviousLanguage();
            else if (e.Command == TranslateWindowCommands.NextEngine)
                model.GoToNextEngine();
            else if (e.Command == TranslateWindowCommands.PreviousEngine)
                model.GoToPreviousEngine();
            else if (e.Command == TranslateWindowCommands.SwitchLanguages)
                model.SwitchLanguages();
        }

        private void OpenLanguageFrom()
        {
            cboLanguageFrom.IsDropDownOpen = true;
            cboLanguageFrom.Focus();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void WebBrowserResult_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            HideJsScriptErrors((WebBrowser)sender);
        }

        public void HideJsScriptErrors(WebBrowser wb)
        {
            // IWebBrowser2 interface
            // Exposes methods that are implemented by the WebBrowser control
            // Searches for the specified field, using the specified binding constraints.
            FieldInfo fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fld == null)
                return;
            object obj = fld.GetValue(wb);
            if (obj == null)
                return;
            // Silent: Sets or gets a value that indicates whether the object can display dialog boxes.
            // HRESULT IWebBrowser2::get_Silent(VARIANT_BOOL *pbSilent);HRESULT IWebBrowser2::put_Silent(VARIANT_BOOL bSilent);
            obj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] { true });
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            txtTextSearch.Focus();
        }

        private void CboLanguageFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtTextSearch.Focus();
        }

        private void TabItem_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.Source is TabItem tabItem && e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                tabItem.IsSelected = true;
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }

        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Source is TabItem tabItemTarget && e.Data.GetData(typeof(TabItem)) is TabItem tabItemSource
                    && tabItemTarget != tabItemSource)
            {
                model.ChangePositionItem(tabItemTarget.DataContext, tabItemSource.DataContext);
            }
        }

        private void TabItem_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Source is TabItem tabItemTarget && e.Data.GetData(typeof(TabItem)) is TabItem tabItemSource)
            {
                if (tabItemTarget == tabItemSource || tabItemTarget.DataContext.GetType() != tabItemSource.DataContext.GetType())
                {
                    e.Handled = true;
                    e.Effects = DragDropEffects.None;
                }
            }
        }

        private void MnuSortLanguagesByName_Click(object sender, RoutedEventArgs e)
        {
            model.SortLanguagesByName();
        }

        private void MnuSortLanguagesByIso_Click(object sender, RoutedEventArgs e)
        {
            model.SortLanguagesByIso();
        }

        private void MnuSortLanguagesByCustomOrder_Click(object sender, RoutedEventArgs e)
        {
            model.SortLanguagesByCustomOrder();
        }

        private void MnuSortEnginesByName_Click(object sender, RoutedEventArgs e)
        {
            model.SortEnginesByName();
        }

        private void MnuSortEnginesByCustomOrder_Click(object sender, RoutedEventArgs e)
        {
            model.SortEnginesByCustomOrder();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            model.SaveModelState();
        }
    }
}