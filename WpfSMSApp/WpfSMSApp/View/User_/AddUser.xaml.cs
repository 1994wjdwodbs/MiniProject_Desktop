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

namespace WpfSMSApp.View.User_
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddUser : Page
    {
        public AddUser()
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
                LblUserIdentityNumber.Visibility = LblUserSurName.Visibility =
                    LblUserName.Visibility = LblUserEmail.Visibility =
                    LblUserPassword.Visibility = LblUserAdmin.Visibility =
                    LblUserActivated.Visibility = Visibility.Hidden;

                // 콤보박스 초기화
                List<string> comboValues = new List<String>
                {
                    "False", // Index : 0
                    "True"   // Index : 1
                };
                // ItemsSouorce : System.Windows.Controls.ItemsControl의 콘텐츠를 생성하는 데 사용되는 컬렉션을 가져오거나 설정합니다.
                CboUserAdmin.ItemsSource = comboValues;
                CboUserActivated.ItemsSource = comboValues;

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

            LblUserIdentityNumber.Visibility = LblUserSurName.Visibility =
                  LblUserName.Visibility = LblUserEmail.Visibility =
                  LblUserPassword.Visibility = LblUserAdmin.Visibility =
                  LblUserActivated.Visibility = Visibility.Hidden;

            
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

            if (string.IsNullOrEmpty(TxtUserName.Text))
            {
                LblUserName.Visibility = Visibility.Visible;
                LblUserName.Text = "이름을 입력하세요.";
                isValid = false;
            }

            if (string.IsNullOrEmpty(TxtUserEmail.Text))
            {
                LblUserEmail.Visibility = Visibility.Visible;
                LblUserEmail.Text = "메일을 입력하세요.";
                isValid = false;
            }
            else
            {
                if (Logic.DataAccess.GetUsers().Where(u => u.UserEmail.Equals(TxtUserEmail.Text)).Count() > 0)
                {
                    LblUserEmail.Visibility = Visibility.Visible;
                    LblUserEmail.Text = "중복된 이메일이 존재합니다.";
                    isValid = false;
                }
            }

            if (string.IsNullOrEmpty(TxtUserPassword.Password))
            {
                LblUserPassword.Visibility = Visibility.Visible;
                LblUserPassword.Text = "패스워드를 입력하세요.";
                isValid = false;
            }

            if (CboUserAdmin.SelectedIndex < 0)
            {
                LblUserAdmin.Visibility = Visibility.Visible;
                LblUserAdmin.Text = "관리자 여부를 선택하세요.";
                isValid = false;
            }

            if (CboUserActivated.SelectedIndex < 0)
            {
                LblUserActivated.Visibility = Visibility.Visible;
                LblUserActivated.Text = "활성 여부를 선택하세요.";
                isValid = false;
            }


            // 이메일 유효형식 통과 못하면
            if (!Commons.IsValidEmail(TxtUserEmail.Text))
            {
                LblUserEmail.Visibility = Visibility.Visible;
                LblUserEmail.Text = "이메일 형식이 올바르지 않습니다.";
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
                user.UserName = TxtUserName.Text;
                user.UserEmail = TxtUserEmail.Text;

                user.UserPassword = TxtUserPassword.Password;
                var mdHash = MD5.Create();
                user.UserPassword = Commons.GetMD5Hash(mdHash, user.UserPassword);

                user.UserAdmin = (bool)bool.Parse(CboUserAdmin.SelectedValue.ToString());
                user.UserActivated = (bool)bool.Parse(CboUserActivated.SelectedValue.ToString());

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
                        NavigationService.Navigate(new UserList());
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
