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
    public partial class DeactiveUser : Page
    {
        public DeactiveUser()
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
                
                // 콤보박스 초기화
                List<string> comboValues = new List<String>
                {
                    "False", // Index : 0
                    "True"   // Index : 1
                };
                // ItemsSouorce : System.Windows.Controls.ItemsControl의 콘텐츠를 생성하는 데 사용되는 컬렉션을 가져오거나 설정합니다.

                // 그리드 바인딩
                List<Model.User> users = Logic.DataAccess.GetUsers();
                this.DataContext = users;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 MyAccount Loaded : {ex.Message}");
                throw ex;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private async void BtnDeactive_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if(GrdData.SelectedItem == null)
            {
                await Commons.ShowMessageAsync("비활성화 메뉴", "비활성화할 사용자를 선택하세요.");
                isValid = false;
            }

            // 전부 올바르면
            if (isValid)
            {
                // DB 입력처리

                var user = (Model.User)GrdData.SelectedItem;
                user.UserActivated = false;

                try
                {
                    // SetUser -> System.Data.Entity.Migrations.AddOrUpdate는 바뀐 내용이 없으면 Update하지 않는다!!
                    int result = Logic.DataAccess.SetUser(user);

                    if (result == 0)
                    {
                        // Application.Current : System.Windows.Application 현재에 대한 개체(Application 객체값)
                        // Application.Current.MainWindow : System.Windows.Application.MainWindow 와 같은 브라우저에서 호스팅되는 응용 프로그램에서 설정 된 XBAP(XAML
                        // 브라우저 응용 프로그램)합니다.
                        await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync("계정 비활성화 실패", "계정 비활성화에 문제가 발생했습니다. 관리자에게 문의 바랍니다.",
               MessageDialogStyle.Affirmative, null);

                    }
                    // 수정 됨
                    else
                    {
                       await ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync("계정 비활성화 성공", "계정 비활성화 완료.",
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

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var user = GrdData.SelectedItem as Model.User;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 GrdData_SelectedCellsChanged : {ex}");
            }
        }
    }
}
