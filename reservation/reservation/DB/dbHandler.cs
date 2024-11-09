
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
        public reservation cancelReservation(Guid reservationUid, string username)
        {
            using (ApplicationContext db = getDb())
            {
                var Reservations = db.reservation.ToList();

                foreach (reservation res in Reservations)
                {
                    //Console.WriteLine("Res.Username: " + res.username);
                    if (res.reservation_uid == reservationUid 
                        && res.username == username)

                    {
                        res.status = "CANCELED";
                        db.Update(res);
                        db.SaveChanges();
                        return res;
                    }
                }
                return null;
            }
        }
        public void PostReservation(reservationToDo res_)
        {
            using (ApplicationContext db = getDb())
            {
                var Reservations = db.reservation.ToList();
                int maxId = 0;
                foreach (reservation res in Reservations)
                {
                    //Console.WriteLine("Res.Username: " + res.username);
                    if (maxId< res.id)

                    {
                        maxId = res.id;
                    }
                }
                reservation new_res = new reservation();
                new_res.start_date = res_.start_date;
                new_res.end_data = res_.end_data;
                new_res.status = res_.status;
                new_res.id = maxId + 1;
                new_res.hotel_id = res_.hotel_id;
                new_res.payment_uid = res_.payment_uid;
                new_res.reservation_uid = res_.reservation_uid;


            }
        }
        //метод для обращеиня к loyalty для получения инфы о пользователе




    }
}

