namespace WpfTranslator
{
    public partial class TranslateWindowModel
    {
        public class Language
        {
            // languages":[{"iso639_1":"ps","iso639_2":"pus","name":"Pashto","nativeName":"پښتو"},
            public string Iso => Iso639_1;
            public string Iso639_1 { get; set; }
            public string Iso639_2 { get; set; }
            public string Name { get; set; }
            public string NativeName { get; set; }


            public override bool Equals(object obj)
            {
                if (obj is Language language)
                    return Iso == language.Iso;
                return false;
            }

            public override int GetHashCode()
            {
                return (Iso ?? string.Empty).GetHashCode();
            }
        }


    }
}
