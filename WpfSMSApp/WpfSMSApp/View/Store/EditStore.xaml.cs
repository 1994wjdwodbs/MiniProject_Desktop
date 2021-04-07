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
    public partial class EditStore : Page
    {
        bool IsValid = true;
        private int StoreID { get; set; }
        // 수정할 창고 객체
        private Model.Store CurrentStore { get; set; }

        public EditStore()
        {
            InitializeComponent();
        }

        // 추가 생성자 (StoreList에서 storeId를 받아옴)
        public EditStore(int storeId) : this()
        {
            StoreID = storeId;
        }

        public void ChangeLabelsVisible(Visibility visible)
        {

        }
        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;
            TxtStoreID.Text = TxtStoreName.Text = TxtStoreLocation.Text = "";

            // Store 테이블에서 내용을 읽음
            try
            {
                // FirstOrDefault : 시퀀스의 첫 번째 요소를 반환하거나, 시퀀스에 요소가 없으면 기본값을 반환합니다.
                CurrentStore = Logic.DataAccess.GetStores().Where(s => s.StoreID.Equals(StoreID)).FirstOrDefault();
                TxtStoreID.Text = CurrentStore.StoreID.ToString();
                TxtStoreName.Text = CurrentStore.StoreName;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"EditStore.xaml.cs Page Loaded 예외발생 : {ex}");
                Commons.ShowMessageAsync("예외", $"예외발생 : {ex}");
            }
        }

        public bool IsValidInput()
        {
            // 모든 입력된 값이 만족하는지 유효성 검사 플래그
            IsValid = true;

            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;

            
            if (string.IsNullOrEmpty(TxtStoreName.Text))
            {
                LblStoreName.Visibility = Visibility.Visible;
                LblStoreName.Text = "창고명을 입력하세요.";
                IsValid = false;
            }
            else
            {
                if (Logic.DataAccess.GetStores().Where(u => u.StoreName.Equals(TxtStoreName.Text)).Count() > 0)
                {
                    LblStoreName.Visibility = Visibility.Visible;
                    LblStoreName.Text = "중복된 창고명이 존재합니다.";
                    IsValid = false;
                }
            }

            if (string.IsNullOrEmpty(TxtStoreLocation.Text))
            {
                LblStoreLocation.Visibility = Visibility.Visible;
                LblStoreLocation.Text = "창고위치를 입력하세요.";
                IsValid = false;
            }

            return IsValid;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;

            // 새로운 회원 정보 
            Model.Store store = new Model.Store();

            // 전부 올바르면(유효성 체크)
            if (IsValidInput())
            {
                // DB 입력처리
                CurrentStore.StoreName = TxtStoreName.Text;
                CurrentStore.StoreLocation = TxtStoreLocation.Text;

                try
                {
                    // SetUser -> System.Data.Entity.Migrations.AddOrUpdate는 바뀐 내용이 없으면 Update하지 않는다!!
                    int result = Logic.DataAccess.SetStore(CurrentStore);

                    if (result == 0)
                    {
                        // Application.Current : System.Windows.Application 현재에 대한 개체(Application 객체값)
                        // Application.Current.MainWindow : System.Windows.Application.MainWindow 와 같은 브라우저에서 호스팅되는 응용 프로그램에서 설정 된 XBAP(XAML
                        // 브라우저 응용 프로그램)합니다.
                        Commons.LOGGER.Error("AddStore.xaml.cs 창고정보 저장오류 발생");
                        ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync("창고 수정 실패", "창고 생성에 문제가 발생했습니다. 관리자에게 문의 바랍니다.",
               MessageDialogStyle.Affirmative, null);
                    }
                    // 수정 됨
                    else
                    {
                       ((MetroWindow)(Application.Current.MainWindow)).ShowMessageAsync("창고 수정 성공", "창고 수정 완료.",
              MessageDialogStyle.Affirmative, null);
                        NavigationService.Navigate(new StoreList());
                    }
                }
                catch (Exception ex)
                {
                    // Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }

        private void TxtStoreName_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }

        private void TxtStoreLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }
    }
}
