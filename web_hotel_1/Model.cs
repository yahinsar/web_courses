using System.Collections.Generic;

namespace web_hotel_1;

public class Class
{
    public int Id;
    public string Name;
    public Decimal Cost;
}

public class Room
{
    public int Id;
    public string Room_empty;
    public int Number_of_places;
    public Class ClassID;
}

public class Database
{
    public List<Room> Rooms;
    public List<Class> ClassList;

    public Database()
    {
        ClassList = new List<Class> {
       new Class { Id = 1, Name = "Эконом", Cost = 2000},
       new Class { Id = 2, Name = "Бизнес", Cost = 5000},
       new Class { Id = 3, Name = "Люкс", Cost = 8000},
    };

        Rooms = new List<Room> {
       new Room { Id = 1, Room_empty = "свободна", Number_of_places = 1, ClassID = ClassList[0]},
       new Room { Id = 2, Room_empty = "занята", Number_of_places = 2, ClassID = ClassList[0]},
       new Room { Id = 3, Room_empty = "занята", Number_of_places = 2, ClassID = ClassList[0]},
       new Room { Id = 4, Room_empty = "занята", Number_of_places = 1, ClassID = ClassList[1]},
       new Room { Id = 5, Room_empty = "свободна", Number_of_places = 2, ClassID = ClassList[1]},
       new Room { Id = 6, Room_empty = "свободна", Number_of_places = 2, ClassID = ClassList[2]},
    };
    }
}
