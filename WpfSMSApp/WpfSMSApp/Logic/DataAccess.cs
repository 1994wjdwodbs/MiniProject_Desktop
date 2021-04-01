using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSMSApp.Model;

namespace WpfSMSApp.Logic
{
    public class DataAccess
    {
        // 회원 정보 가져오기
        public static List<User> GetUsers()
        {
            List<User> users;

            using (var ctx = new SMSEntities())
            {
                users = ctx.User.ToList(); // SELECT * FROM user
            }

            return users;
        }

        // 회원 정보 수정하기
        public static int SetUser(User user)
        {
            using (var ctx = new SMSEntities())
            {
                // System.Data.Entity.Migrations.AddOrUpdate는 바뀐 내용이 없으면 Update하지 않는다!!
                ctx.User.AddOrUpdate(user);
                return ctx.SaveChanges();
            }
        }

        public static List<Store> GetStores()
        {
            List<Store> stores;

            // ctx : context
            using (var ctx = new SMSEntities())
            {
                stores = ctx.Store.ToList(); // SELECT * FROM user
            }

            return stores;
        }

        public static int SetStore(Store store)
        {
            using (var ctx = new SMSEntities())
            {
                // System.Data.Entity.Migrations.AddOrUpdate는 바뀐 내용이 없으면 Update하지 않는다!!
                ctx.Store.AddOrUpdate(store);
                return ctx.SaveChanges();
            }
        }

        public static List<Stock> GetStocks()
        {
            List<Stock> stocks;

            // ctx : context
            using (var ctx = new SMSEntities())
            {
                stocks = ctx.Stock.ToList(); // SELECT * FROM user
            }

            return stocks;
        }

    }
}
