using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using WpfSMSApp.Helper;
using WpfSMSApp.Model;

namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddStore : Page
    {
        public AddStore()
        {
            InitializeComponent();
        }

        public void ChangeLabelsVisible(Visibility visible)
        {

        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LblUserIdentityNumber.Visibility = LblUserSurName.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 MyAccount Loaded : {ex.Message}");
                throw ex;
            }
        }

        public bool IsValidInput()
        {
            // 모든 입력된 값이 만족하는지 유효성 검사 플래그
            bool isValid = true;

            LblUserIdentityNumber.Visibility = LblUserSurName.Visibility = Visibility.Hidden;

            
            if (string.IsNullOrEmpty(TxtUserIdentityNumber.Text))
            {
                LblUserIdentityNumber.Visibility = Visibility.Visible;
                LblUserIdentityNumber.Text = "사번을 입력하세요.";
                isValid = false;
            }
            else
            {
                if (Logic.DataAccess.GetUsers().Where(u => u.UserIdentityNumber.Equals(TxtUserIdentityNumber.Text)).Count() > 0)
                {
                    LblUserIdentityNumber.Visibility = Visibility.Visible;
                    LblUserIdentityNumber.Text = "중복된 사번이 존재합니다.";
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(TxtUserSurName.Text))
            {
                LblUserSurName.Visibility = Visibility.Visible;
                LblUserSurName.Text = "이름(성)을 입력하세요.";
                isValid = false;
            }

            return isValid;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // 새로운 회원 정보 
            User user = new Model.User();

            // 전부 올바르면
            if (IsValidInput())
            {
                // DB 입력처리
                user.UserIdentityNumber = TxtUserIdentityNumber.Text;
                user.UserSurname = TxtUserSurName.Text;

                try
                {
                    // SetUser -> System.Data.Entity.Migrations.AddOrUpdate는 바뀐 내용이 없으면 Update하지 않는다!!
                    int result = Logic.DataAccess.SetUser(user);

                    if (result == 0)
                    {
                        // Application.Current : System.Windows.Application 현재에 대한 개체(Application 객체값)
                        // Application.Current.MainWindow : System.Windows.Application.MainWindow 와 같은 브라우저에서 호스팅되는 응용 프로그램에서 설정 된 XBAP(XAML
                        // 브라우저 응용 프로그램)합니다.
                        ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync("계정 생성 실패", "계정 생성에 문제가 발생했습니다. 관리자에게 문의 바랍니다.",
               MessageDialogStyle.Affirmative, null);
                    }
                    // 수정 됨
                    else
                    {
                       ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync("계정 생성 성공", "계정 생성 완료.",
              MessageDialogStyle.Affirmative, null);
                        // NavigationService.Navigate(new UserList());
                    }
                }
                catch (Exception ex)
                {
                    // Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }
    }
}
