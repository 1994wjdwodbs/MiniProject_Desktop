﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSMSApp.Model;

namespace WpfSMSApp.Logic
{
    public class DataAccess
    {

        public static List<User> GetUsers()
        {
            List<User> users;

            using (var ctx = new SMSEntities())
            {
                users = ctx.User.ToList(); // SELECT * FROM user
            }

            return users;
        }
    }
}