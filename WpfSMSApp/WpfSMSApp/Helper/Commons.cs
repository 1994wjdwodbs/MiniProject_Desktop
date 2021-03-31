using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WpfSMSApp.Model;

namespace WpfSMSApp.Helper
{
    public class Commons
    {
        // NLog 정적 인스턴스 생성
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        // Model의 User 테이블(클래스)
        // 로그인 유저 정보
        public static User LOGINED_USER;

        // 패스워드 해시(MD5)
        public static string GetMD5Hash(MD5 md5Hash, string plainStr)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(plainStr));
            StringBuilder builder = new StringBuilder();
            for(int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));
            }

            return builder.ToString();
        }

        // 이메일 정규식 확인
        public static bool IsValidEmail(string email)
        {
            bool valid = Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");

            return valid;
        }

        public static async Task<MessageDialogResult> ShowMessageAsync(
            string title, string message,
            MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            return await (Application.Current.MainWindow as MetroWindow).ShowMessageAsync(title, message, style);
        }
    }
}
