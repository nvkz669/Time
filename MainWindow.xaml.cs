using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using disUtils;

namespace TiMonSys
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!ConnectDataContext())
            {
                MessageBox.Show("Ошибка подключения к базе данных!\nПроверьте параметры подключения к базе данных!");

                AppSetting();
            }
            else
            {
                ReloadResponsibleFaces();
            }
          
           
        }

        private void AppSetting()
        {
            var nw = new View.WinSettingConnect();
            nw.ShowDialog();
            if ((nw.DialogResult.HasValue && nw.DialogResult.Value))
            {
                ReloadResponsibleFaces();
                return;
            }
            Application.Current.Shutdown(-1);
        }
        private int GetAdmins()
        {
            
            var admins = ContextManager.Instance.Database.ResponsibleFaces.Where(_ => !_.IsLocked & _.Role == 0);
            return (admins.Any()) ? admins.Count() : 0;
        }
        private void ReloadResponsibleFaces()
        {
            if (GetAdmins() == 0)
            {
                MessageBox.Show("В системе не зарегистрировано ни одного действующего администратора.\n" +
                    "Укажите нового администратора иначе программа будет закрыта!");
                View.CreateNewAdmin nw = new View.CreateNewAdmin();
                nw.ShowDialog();
                if ((nw.DialogResult.HasValue && nw.DialogResult.Value))
                {
                    ContextManager.Instance.ResetDataContext();
                    ReloadResponsibleFaces();
                }
                else Application.Current.Shutdown(0);
            }

            

          
            var responsibleFaces = ContextManager.Instance.Database.ResponsibleFaces.Where(_ => !_.IsLocked).ToList();
           
          
          
            CbbEmployees.ItemsSource = responsibleFaces.OrderBy(_=>_.LastName).ToList();
            try
            {
                var lastUserId = RegOptions.GetKeyValue("AppSetting", "LastUser", "").ToString();
                if (string.IsNullOrEmpty(lastUserId)) return;
                var i = int.Parse(lastUserId);
                var lastUser = responsibleFaces.FirstOrDefault(_ => _.Id == i);
                CbbEmployees.SelectedItem = lastUser;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static bool ConnectDataContext()
        {

            bool isConnectionReady;
            try
            {
                var connectionString = "Data Source=;";
                var reg = (string)RegOptions.GetKeyValue("ConnectionSetting", "DataBasePath", "");

                if (!string.IsNullOrEmpty(reg)) connectionString = reg;

                isConnectionReady = ContextManager.Instance.Connect(connectionString);
            }
            catch (Exception)
            {
                return false;
            }

            return isConnectionReady;
        }


        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {


        
            TbxPassword.Focus();

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                    case Key.F9:
                {
                    AppSetting();
                    break;
                }
                case Key.Enter:
                {
                    GoHomeWindow();
                    break;
                }
            }
        }

        private void btn_Input_Click(object sender, RoutedEventArgs e)
        {
            if (CbbEmployees.SelectedItem == null) return;
            GoHomeWindow();
        }

        private bool ValidationPassword()
        {
            var responsibleFace = CbbEmployees.SelectedItem as ResponsibleFace;
            
            return TbxPassword.Password == Crypt.DecryptStr(responsibleFace.Password);
        }

        private void GoHomeWindow()
        {
            if (!ValidationPassword())
            {
                MessageBox.Show("Неправильный пароль!\nПопробуйте ещё раз.");
                return;
            }
            var user = CbbEmployees.SelectedItem as ResponsibleFace;
            RegOptions.SetKeyValue("AppSetting", "LastUser", user.Id);
            App.CurrentUser = user;
            System.Windows.Forms.InputLanguage.CurrentInputLanguage =
               System.Windows.Forms.InputLanguage.FromCulture(new System.Globalization.CultureInfo("ru-RU"));
            var nw = new View.WinHome();
            nw.Show();
            Close();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void DockPanel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AppSetting();
        }
    }
}
