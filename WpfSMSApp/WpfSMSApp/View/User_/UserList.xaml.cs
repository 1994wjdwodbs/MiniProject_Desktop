using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public partial class UserList : Page
    {
        public UserList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                RdoAll.IsChecked = true;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 UserList Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnAddUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUser());
            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnAddUser_Click : {ex}");
                throw ex;
            }
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EditUser());
            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditUser_Click : {ex}");
                throw ex;
            }
        }

        private void BtnDeactiveUser_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DeactiveUser());
            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnDeactiveUser_Click : {ex}");
                throw ex;
            }
        }

        private void BtnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "PDF File(*.pdf)|*.pdf";
            saveDialog.FileName = "";
            if(saveDialog.ShowDialog() == true)
            {
                // pdf 변환
                try
                {
                    iTextSharp.text.Font font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12);
                    string pdfFilePath = saveDialog.FileName;

                    Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    
                    // #1 pdf 객체 생성
                    PdfPTable pdfTable = new PdfPTable(GrdData.Columns.Count);

                    // #2 pdf 폰트 설정(한글) & 내용 만들기
                    string nanumttf = System.IO.Path.Combine(Environment.CurrentDirectory, @"NanumGothic.ttf");
                    BaseFont nanumBase = BaseFont.CreateFont(nanumttf, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font nanumFont = new iTextSharp.text.Font(nanumBase, 16f);

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph($"부경대 Stock Management System : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", nanumFont);

                    // #3 pdf 내용 생성
                    using(FileStream stream = new FileStream(pdfFilePath, FileMode.OpenOrCreate))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        // #2 에서 만든 내용 추가
                        pdfDoc.Add(title);
                        pdfDoc.Close();
                        stream.Close();
                    }
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 BtnExportPdf_Click : {ex}");
                }
            }
        }

        private void RdoAll_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<WpfSMSApp.Model.User> users = new List<WpfSMSApp.Model.User>();

                if (RdoAll.IsChecked != null ? (bool) RdoAll.IsChecked : false)
                {
                    users = Logic.DataAccess.GetUsers();
                }

                // 데이터 바인딩에 참여할 때 요소에 대한 데이터 컨텍스트를 가져오거나 설정합니다.
                this.DataContext = users;
                // GrdData.DataContext = users;
                // GrdData.ItemsSource = users;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void RdoActive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<WpfSMSApp.Model.User> users = new List<WpfSMSApp.Model.User>();

                if (RdoActive.IsChecked != null ? (bool)RdoActive.IsChecked : false)
                {
                    users = Logic.DataAccess.GetUsers().Where(u => u.UserActivated == true).ToList();
                }

                // 데이터 바인딩에 참여할 때 요소에 대한 데이터 컨텍스트를 가져오거나 설정합니다.
                this.DataContext = users;
                // GrdData.DataContext = users;
                // GrdData.ItemsSource = users;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }

        private void RdoDeactive_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                List<WpfSMSApp.Model.User> users = new List<WpfSMSApp.Model.User>();

                if (RdoDeactive.IsChecked != null ? (bool)RdoDeactive.IsChecked : false)
                {
                    users = Logic.DataAccess.GetUsers().Where(u => u.UserActivated == false).ToList();
                }

                // 데이터 바인딩에 참여할 때 요소에 대한 데이터 컨텍스트를 가져오거나 설정합니다.
                this.DataContext = users;
                // GrdData.DataContext = users;
                // GrdData.ItemsSource = users;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 : {ex}");
            }
        }
    }
}
