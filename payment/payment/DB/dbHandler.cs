
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace payment.DB
{
    public class dbHandler
    {
        DbContextOptions<ApplicationContext> options;
        public dbHandler(DbContextOptions<ApplicationContext> _option) 
        {
            options = _option;
        }
        public dbHandler()
        {
            options = null;
        }


        public ApplicationContext getDb()
        {
             return new ApplicationContext();
        }
        public payment getLoyalty(string username)
        {
            using (ApplicationContext db = getDb())
            {
                // получаем объекты из бд и выводим на консоль
                var Payments = db.payment.ToList();
                //Console.WriteLine("Persons list:");

                foreach (payment u in Payments)
                {
                   /* if (u.username == username)
                        return u;*/

                }
                return null;
            }
        }





    }
}

