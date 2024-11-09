﻿
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
        public loyalty incLoyalty(string username)
        {
            using (ApplicationContext db = getDb())
            {
                // получаем объекты из бд и выводим на консоль
                var Loyalties = db.loyalty.ToList();
                //Console.WriteLine("Persons list:");
                loyalty _;
                foreach (loyalty u in Loyalties)
                {
                    if (u.username == username)
                    {
                        _ = u;
                        _.reservation_count++;

                        if (_.reservation_count >= 20)
                            _.status = "GOLD";
                        else if (_.reservation_count >= 10)
                            _.status = "SILVER";
                        

                        db.loyalty.Update(_);
                        return _;
                    }
                }
                
                return null;
            }
        }





    }
}
