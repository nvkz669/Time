using System;
using System.Collections.Generic;
using System.Windows;
using disUtils;


namespace TiMonSys
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    
    
    public partial class App
    {
        public App()
        {

            CryptoInit();
        }

        

        //public static ResponsibleFace CurrentUser { get; set; }

        public static ResponsibleFace CurrentUser { get; set; }

        private static void CryptoInit()
        {
            try
            {
                Crypt.ProgramKey = new Guid("FD0767FA-2004-2008-81C3-37AB83E0EDC8");
                Crypt.Init();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка запуска приложения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            // Устанавливаем правильные региональные параметры для WPF
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );

            System.Windows.Documents.TextElement.LanguageProperty.OverrideMetadata(
                typeof(System.Windows.Documents.TextElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)
                )
            );


            base.OnStartup(e);
        }
    }
}
