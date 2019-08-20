using System.Windows.Input;

namespace TranslationCenter.UI.Desktop.Views.TranslateWindow
{
    internal static class TranslateWindowCommands
    {
        public static readonly RoutedUICommand SelectEngineCommand =
            new RoutedUICommand("Select Engine",
                nameof(SelectEngineCommand),
                typeof(TranslateWindowCommands),
                new InputGestureCollection() { new KeyGesture(Key.F3) });

        public static readonly RoutedUICommand SelectLanguageCommand =
            new RoutedUICommand("Select Language",
            nameof(SelectLanguageCommand),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.F4) });

        public static readonly RoutedUICommand TranslateCommand =
            new RoutedUICommand("Translate",
            nameof(TranslateCommand),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.F5) });
    }
}