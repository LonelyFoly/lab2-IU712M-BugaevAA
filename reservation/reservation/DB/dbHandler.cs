
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reservation.DB
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
        public hotel[] getHotels(int page, int size)
        {
            using (ApplicationContext db = getDb())
            {
                var Hotels = db.hotels.ToList();

                List<hotel> hotels = new List<hotel>();
                for (int i = (page-1)*size;
                    (i<Hotels.Count() && i<page*size);i++)
                {
                        hotel u = Hotels[i];
                        hotels.Add(u);
                }
                return hotels.ToArray();
            }
        }
        
        public reservation[] getReservationsByUsername(string username)
        {
            using (ApplicationContext db = getDb())
            {
                var Reservations = db.reservation.ToList();

                List<reservation> reservations = new List<reservation>();
                foreach (reservation res in Reservations)
                {
                    Console.WriteLine("Res.Username: "+res.username);
                    if (res.username == username)
                        reservations.Add(res);
                }
                return reservations.ToArray();
            }
        }
        public reservation getReservationsByUsernameAndUid(Guid reservationUid, string username)
        {
            using (ApplicationContext db = getDb())
            {
                var Reservations = db.reservation.ToList();

                reservation reservation = new reservation();
                reservation.username = "";
                foreach (reservation res in Reservations)
                {
                    //Console.WriteLine("Res.Username: " + res.username);
                    if (res.username == username && res.reservation_uid==reservationUid)
                        reservation = res;
                }
                return reservation;
            }
        }
        public hotel checkHotel(Guid hotelUid)
        {
            using (ApplicationContext db = getDb())
            {
                var Hotels = db.hotels.ToList();

                foreach (hotel h in Hotels)
                {
                    //Console.WriteLine("Res.Username: " + res.username);
                    if (h.hotel_uid == hotelUid)
                        return h;
                }
                return null;
            }
        }
        //метод для обращеиня к loyalty для получения инфы о пользователе




    }
}

