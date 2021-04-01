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

namespace WpfSMSApp.View.Account
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditAccount : Page
    {
        public EditAccount()
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

                User user = Commons.LOGINED_USER;
                TxtUserID.Text = user.UserID.ToString();
                TxtUserIdentityNumber.Text = user.UserIdentityNumber.ToString();
                TxtUserSurName.Text = user.UserSurname.ToString();
                TxtUserName.Text = user.UserName.ToString();
                TxtUserEmail.Text = user.UserEmail.ToString();
                TxtUserPassword.Password = string.Empty; //user.UserPassword;

                CboUserAdmin.SelectedIndex = (user.UserAdmin == false ? 0 : 1);
                CboUserActivated.SelectedIndex = (user.UserActivated == false ? 0 : 1);
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 MyAccount Loaded : {ex.Message}");
                throw ex;
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // 모든 입력된 값이 만족하는지 유효성 검사 플래그
            bool isValid = true;

            LblUserIdentityNumber.Visibility = LblUserSurName.Visibility =
                   LblUserName.Visibility = LblUserEmail.Visibility =
                   LblUserPassword.Visibility = LblUserAdmin.Visibility =
                   LblUserActivated.Visibility = Visibility.Hidden;

            User user = Commons.LOGINED_USER;

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

            if (string.IsNullOrEmpty(TxtUserPassword.Password))
            {
                LblUserPassword.Visibility = Visibility.Visible;
                LblUserPassword.Text = "패스워드를 입력하세요.";
                isValid = false;
            }

            // 전부 올바르면
            if (isValid)
            {
                // MessageBox.Show("확인되었습니다.");
                // DB 회원 정보 수정
                user.UserSurname = TxtUserSurName.Text;
                user.UserName = TxtUserName.Text;
                user.UserEmail = TxtUserEmail.Text;

                user.UserPassword = TxtUserPassword.Password;
                var mdHash = MD5.Create();
                user.UserPassword = Commons.GetMD5Hash(mdHash, user.UserPassword);

                user.UserAdmin = (bool) bool.Parse(CboUserAdmin.SelectedValue.ToString());
                user.UserActivated = (bool) bool.Parse(CboUserActivated.SelectedValue.ToString());

                try
                {
                    // SetUser -> System.Data.Entity.Migrations.AddOrUpdate는 바뀐 내용이 없으면 Update하지 않는다!!
                    int result = Logic.DataAccess.SetUser(user);
                    // 수정 안됨
                    if (result == 0)
                    {

                        // Application.Current : System.Windows.Application 현재에 대한 개체(Application 객체값)
                        // Application.Current.MainWindow : System.Windows.Application.MainWindow 와 같은 브라우저에서 호스팅되는 응용 프로그램에서 설정 된 XBAP(XAML
                        // 브라우저 응용 프로그램)합니다.
                        ((MetroWindow) (Application.Current.MainWindow)).ShowMessageAsync("계정 수정 실패", "계정 수정에 문제가 발생했습니다. 관리자에게 문의 바랍니다.",
               MessageDialogStyle.Affirmative, null);
                    }
                    // 수정 됨
                    else
                    {
                        ((MetroWindow) (Application.Current.MainWindow)).ShowMessageAsync("계정 수정 완료", "계정 수정 완료.",
              MessageDialogStyle.Affirmative, null);
                    }
                }
                catch(Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 BtnUpdate_Click : {ex}");
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
