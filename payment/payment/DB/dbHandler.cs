
using Microsoft.AspNetCore.Mvc.TagHelpers;
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
        public void addPayment(Guid payment_uid, int price)
        {
            using (ApplicationContext db = getDb())
            {
                int maxId = 0;
                var Payments = db.payment.ToList();
                //Console.WriteLine("Persons list:");

                foreach (payment u in Payments)
                {
                    if (u.id > maxId)
                         maxId = u.id;
                }
                payment _ = new payment();
                _.id = maxId;
                _.payment_uid = payment_uid;
                _.price = price;
                _.status = "PAID";
                db.payment.Add(_);
                
            }
        }





    }
}

