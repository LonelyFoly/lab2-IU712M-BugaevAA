
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace loyalty.DB
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
        public loyalty getLoyalty(string username)
        {
            using (ApplicationContext db = getDb())
            {
                // получаем объекты из бд и выводим на консоль
                var Loyalties = db.loyalty.ToList();
                //Console.WriteLine("Persons list:");

                foreach (loyalty u in Loyalties)
                {
                    if (u.username == username)
                    {
                        Console.WriteLine($"=====: {u.username}");
                        return u;
                    }
                }
                return null;
            }
        }





    }
}

