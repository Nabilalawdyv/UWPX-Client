﻿using System.Runtime.CompilerServices;
using Shared.Classes;
using Windows.UI.Xaml;

namespace UWPX_UI_Context.Classes.DataTemplates.Pages
{
    public sealed class PersonalizeSettingsPageDataTemplate: AbstractDataTemplate
    {
        //--------------------------------------------------------Attributes:-----------------------------------------------------------------\\
        #region --Attributes--
        private bool _LightThemeChecked;
        public bool LightThemeChecked
        {
            get => _LightThemeChecked;
            set => SetThemeProperty(ref _LightThemeChecked, value, ElementTheme.Light);
        }
        private bool _DarkThemeChecked;
        public bool DarkThemeChecked
        {
            get => _DarkThemeChecked;
            set => SetThemeProperty(ref _DarkThemeChecked, value, ElementTheme.Dark);
        }
        private bool _SystemThemeChecked;
        public bool SystemThemeChecked
        {
            get => _SystemThemeChecked;
            set => SetThemeProperty(ref _SystemThemeChecked, value, ElementTheme.Default);
        }

        #endregion
        //--------------------------------------------------------Constructor:----------------------------------------------------------------\\
        #region --Constructors--
        public PersonalizeSettingsPageDataTemplate()
        {
            LoadSettings();
        }

        #endregion
        //--------------------------------------------------------Set-, Get- Methods:---------------------------------------------------------\\
        #region --Set-, Get- Methods--
        public void SetThemeProperty(ref bool storage, bool value, ElementTheme theme, [CallerMemberName] string propertyName = null)
        {
            if (SetProperty(ref storage, value, propertyName) && value)
            {
                ThemeUtils.RootTheme = theme;
            }
        }

        #endregion
        //--------------------------------------------------------Misc Methods:---------------------------------------------------------------\\
        #region --Misc Methods (Public)--


        #endregion

        #region --Misc Methods (Private)--
        private void LoadSettings()
        {
            switch (ThemeUtils.RootTheme)
            {
                case ElementTheme.Light:
                    LightThemeChecked = true;
                    break;

                case ElementTheme.Dark:
                    DarkThemeChecked = true;
                    break;

                default:
                    SystemThemeChecked = true;
                    break;
            }
        }

        #endregion

        #region --Misc Methods (Protected)--


        #endregion
        //--------------------------------------------------------Events:---------------------------------------------------------------------\\
        #region --Events--


        #endregion
    }
}
