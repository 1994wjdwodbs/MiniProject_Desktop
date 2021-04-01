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
namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccount.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StoreList : Page
    {
        public StoreList()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // TODO : Store 데이터를 읽어온다.
                List<Model.Store> stores = new List<Model.Store>();
                List<Model.StockStore> stockStores = new List<Model.StockStore>();
                List<Model.Stock> stocks = new List<Model.Stock>();
                stores = Logic.DataAccess.GetStores();
                stocks = Logic.DataAccess.GetStocks();

                // stores 데이터를 stockStores로 옮김
                foreach (Model.Store item in stores)
                {
                    stockStores.Add(new Model.StockStore()
                    {
                        StoreID = item.StoreID,
                        StoreName = item.StoreName,
                        StoreLocation = item.StoreLocation,
                        ItemStatus = item.ItemStatus,
                        TagID = item.TagID,
                        BarcodeID = item.BarcodeID,
                        StockQuantity = 0
                    });
                }


                this.DataContext = stockStores;

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 StoreList Loaded : {ex}");
                throw ex;
            }
        }

        private void BtnAddStore_Click(object sender, RoutedEventArgs e)
        {
           NavigationService.Navigate(new AddStore());
            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnAddStore_Click : {ex}");
                throw ex;
            }
        }

        private void BtnEditStore_Click(object sender, RoutedEventArgs e)
        {
            // NavigationService.Navigate(new EditStore());
            try
            {

            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 BtnEditStore_Click : {ex}");
                throw ex;
            }
        }

        

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
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

                    // #1 : pdf 객체 생성
                    Document pdfDoc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
                    
                    // #2 : pdf 폰트 설정(한글)
                    // 폰트 경로
                    string nanumPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"NanumGothic.ttf");
                    BaseFont nanumBase = BaseFont.CreateFont(nanumPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    // 20pt 나눔폰트 (타이틀)
                    Font nanumFont_Title = new iTextSharp.text.Font(nanumBase, 20f);
                    // 12pt 나눔폰트 (본문)
                    Font nanumFont_Content = new iTextSharp.text.Font(nanumBase, 12f);

                    iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph($"부경대 재고관리시스템(Stock Management System, SMS)", nanumFont_Title);
                    iTextSharp.text.Paragraph subtitle = new iTextSharp.text.Paragraph($"exported : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n\n", nanumFont_Content);

                    // #3 : pdf 내용 생성
                    using (FileStream stream = new FileStream(pdfFilePath, FileMode.OpenOrCreate))
                    {
                        PdfWriter.GetInstance(pdfDoc, stream);
                        pdfDoc.Open();
                        // #4 : #2 에서 만든 폰트 이용 -> 타이틀 생성
                        pdfDoc.Add(title);
                        pdfDoc.Add(subtitle);

                        // #5 : pdf 테이블 생성
                        PdfPTable pdfTable = new PdfPTable(GrdData.Columns.Count);
                        // pdf 테이블 전체 사이즈 설정
                        pdfTable.WidthPercentage = 100;
                        // pdf 테이블 셀 사이즈 설정
                        pdfTable.SetWidths(new float[] {7f,15f,10f,15f,28f,12f,10f});

                        // #6 : pdf 테이블 Attribute=Col=Field 생성
                        foreach (DataGridColumn col in GrdData.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(col.Header.ToString(), nanumFont_Content));
                            cell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        // #7 : pdf 테이블 Tuple=Row=Record 생성
                        foreach (var item in GrdData.Items)
                        {
                            if (item is Model.User)
                            {
                                var temp = item as Model.User;
                                PdfPCell cell = new PdfPCell(new Phrase(temp.UserID.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);

                                cell = new PdfPCell(new Phrase(temp.UserIdentityNumber.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);

                                cell = new PdfPCell(new Phrase(temp.UserSurname.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);

                                cell = new PdfPCell(new Phrase(temp.UserName.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);

                                cell = new PdfPCell(new Phrase(temp.UserEmail.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);

                                cell = new PdfPCell(new Phrase(temp.UserAdmin.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);

                                cell = new PdfPCell(new Phrase(temp.UserActivated.ToString(), nanumFont_Content));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                pdfTable.AddCell(cell);
                            }
                        }
                        // pdf 테이블 추가
                        pdfDoc.Add(pdfTable);

                        pdfDoc.Close();
                        stream.Close();
                    }

                    Commons.ShowMessageAsync("PDF변환", "PDF 익스포트 성공!");
                }
                catch (Exception ex)
                {
                    Commons.LOGGER.Error($"예외발생 BtnExportPdf_Click : {ex}");
                }
            }
        }

       
    }
}
