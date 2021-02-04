using System;
using System.Windows;

namespace Launcher.Localization.Loc
{
    public static class LocManager
    {
        private static ResourceDictionary _entryDictionary;
        private static LocalizationResourceDictionary _currentDictionary;

        #region Public interface

        public static void Initialize(Application application)
        {
            _entryDictionary = application.Resources;
        }

        public static LauncherLocalization CurrentLoc { get; private set; } = LauncherLocalization.None;

        public static void SetLocale(LauncherLocalization loc)
        {
            if (CurrentLoc == loc) return;

            var newLocaleDictionary = new LocalizationResourceDictionary
                { Source = new Uri($"/Launcher.Localization;component/{loc}.xaml", UriKind.Relative) };
            var countOfDictionaries = _entryDictionary.MergedDictionaries.Count;

            _entryDictionary.MergedDictionaries.Insert(countOfDictionaries, newLocaleDictionary);
            _entryDictionary.MergedDictionaries.Remove(_currentDictionary);

            _currentDictionary = newLocaleDictionary;
            CurrentLoc = loc;
        }

        public static string Get(string key) => (string) _currentDictionary[key];

        #endregion
    }
}