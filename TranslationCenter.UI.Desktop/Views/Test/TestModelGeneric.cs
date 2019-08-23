namespace TranslationCenter.UI.Desktop.Views.Test
{
    internal class TestModel<T> : ViewModelBase
    {
        private string _name = "Fulvio Lage";

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
    }
}