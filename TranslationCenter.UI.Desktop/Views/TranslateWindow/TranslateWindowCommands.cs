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
            new InputGestureCollection() { 
                new KeyGesture(Key.F5),
                new KeyGesture(Key.Return, ModifierKeys.Control),
                new KeyGesture(Key.Enter, ModifierKeys.Control)
            });

        public static readonly RoutedUICommand NextLanguage =
            new RoutedUICommand("NextLanguage",
            nameof(NextLanguage),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.Right, ModifierKeys.Alt) });

        public static readonly RoutedUICommand PreviousLanguage =
            new RoutedUICommand("PreviousLanguage",
            nameof(PreviousLanguage),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.Left, ModifierKeys.Alt) });
                
        
        public static readonly RoutedUICommand NextEngine =
            new RoutedUICommand("NextEngine",
            nameof(NextEngine),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.Down, ModifierKeys.Alt) });

        public static readonly RoutedUICommand PreviousEngine =
            new RoutedUICommand("PreviousEngine",
            nameof(PreviousEngine),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.Up, ModifierKeys.Alt) });

        public static readonly RoutedUICommand SelectLanguageFrom =
            new RoutedUICommand("SelectLanguageFrom",
            nameof(SelectLanguageFrom),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.F2) });


        public static readonly RoutedUICommand SwitchLanguages =
            new RoutedUICommand("SwitchLanguages",
            nameof(SwitchLanguages),
            typeof(TranslateWindowCommands),
            new InputGestureCollection() { new KeyGesture(Key.F6) });
    }
}