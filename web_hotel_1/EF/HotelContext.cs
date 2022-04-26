using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace web_hotel_1.EF
{
    internal class HotelContext : DbContext
    {
        public DbSet<Class_EF> ClassList_EF { get; set; }
        public DbSet<Room_EF> Rooms_EF { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server = DESKTOP-MP8QK16; Database = HotelEF; Trusted_Connection = True;");
            base.OnConfiguring(optionsBuilder);
        }

        public void CreateDbIfNotExist()
        {
            this.Database.EnsureCreated();
        }

        public void DropDB()
        {
            this.Database.EnsureDeleted();
        }

        public class Class_EF
        {
            public int Id { get; set; }

            [MaxLength(50)]
            public string? Name { get; set; }
            public Decimal Cost { get; set; }
        }

        public class Room_EF
        {
            public int Id { get; set; }

            [MaxLength(50)]
            public string? Room_empty { get; set; }
            public int Number_of_places { get; set; }
            public Class_EF ClassID { get; set; }
        }

        public void Add_ClassList_EF()
        {
            ClassList_EF.AddRange(entities: new Class_EF[]
            {
                new Class_EF { Name = "Эконом", Cost = 2000},
                new Class_EF { Name = "Бизнес", Cost = 5000},
                new Class_EF { Name = "Люкс", Cost = 8000}
            });
        }


        public void Add_Rooms_EF()
        {
            Rooms_EF.AddRange(entities: new Room_EF[]
            {
            new Room_EF { Room_empty = "свободна", Number_of_places = 1, ClassID = ClassList_EF.First(t => t.Id== 1)},
            new Room_EF { Room_empty = "занята", Number_of_places = 2, ClassID = ClassList_EF.First(t => t.Id== 1)},
            new Room_EF { Room_empty = "занята", Number_of_places = 2, ClassID = ClassList_EF.First(t => t.Id== 1)},
            new Room_EF { Room_empty = "занята", Number_of_places = 1, ClassID = ClassList_EF.First(t => t.Id== 2)},
            new Room_EF { Room_empty = "свободна", Number_of_places = 2, ClassID = ClassList_EF.First(t => t.Id== 2)},
            new Room_EF { Room_empty = "свободна", Number_of_places = 2, ClassID = ClassList_EF.First(t => t.Id== 3)}
            });
        }
    }
}
