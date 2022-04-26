using web_hotel_1;
using web_hotel_1.EF;
using static web_hotel_1.EF.HotelContext;
using System.Data.SqlClient;

static void AdoNetDemo()
{
    var connection = new SqlConnection(@"Server = DESKTOP-MP8QK16; Database = Hotel; Trusted_Connection = True;");
    connection.Open();

    //                                                      1. Вывести список свободных комнат (номер, класс), отсортированных по стоимости аренды

    Console.WriteLine("1. Вывести список свободных комнат (номер, класс), отсортированных по стоимости аренды: \n");


    var command = connection.CreateCommand();
    command.CommandText = "SELECT room.id, class.name, class.cost FROM class, room WHERE room.room_empty = 'свободна' AND room.classID = class.id ORDER BY class.cost;";
    var reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"Комната: {reader.GetInt32(reader.GetOrdinal("id"))}, Класс: {reader.GetString(reader.GetOrdinal("name"))}, Цена: {reader.GetDecimal(reader.GetOrdinal("cost"))}");
    }
    reader.Close();

    //                                                                          2. Рассчитать ежедневную прибыль отеля

    Console.WriteLine("\n\n2. Рассчитать ежедневную прибыль отеля: \n");


    command = connection.CreateCommand();
    command.CommandText = "SELECT SUM(class.cost) FROM class, room WHERE room.room_empty = 'занята' AND room.classID = class.id;";
    reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"Прибыль за сегодня: {reader.GetDecimal(0)}");
    }
    reader.Close();

    //                                                      3. Рассчитать загруженность отеля (отношение числа сданных к общему числу комнат)

    Console.WriteLine("\n\n3. Рассчитать загруженность отеля (отношение числа сданных к общему числу комнат): \n");


    command = connection.CreateCommand();
    command.CommandText = "SELECT CAST(SUM(case room.room_empty when 'занята' then 1 else 0 end) AS float)/COUNT(room.id) FROM room;";
    reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"Загруженность отеля: {reader.GetDouble(0)}");
    }
    reader.Close();

    //                                                                      4. Вывести пары: (класс,общее число комнат)

    Console.WriteLine("\n\n4. Вывести пары: (класс,общее число комнат): \n");


    command = connection.CreateCommand();
    command.CommandText = "SELECT class.name, COUNT(room.id) AS room_count FROM class, room WHERE room.classID = class.id GROUP BY class.name ORDER BY COUNT(room.id)";
    reader = command.ExecuteReader();
    while (reader.Read())
    {
        Console.WriteLine($"Класс: {reader.GetString(0)}, Общее количество комнат: {reader.GetInt32(reader.GetOrdinal("room_count"))}");
    }
    reader.Close();

    //                                                                      5. Извлечь самую дорогую свободную комнату, пометить как занятую

    Console.WriteLine("\n\n5. Извлечь самую дорогую свободную комнату, пометить как занятую: \n");


    command = connection.CreateCommand();
    command.CommandText = "SELECT TOP (1) room.id FROM room, class WHERE room.classID = class.id AND room.room_empty = 'свободна' AND class.cost = (SELECT MAX(class.cost) FROM class)";
    reader = command.ExecuteReader();
    while (reader.Read())
    {
        var connection_update = new SqlConnection(@"Server = DESKTOP-MP8QK16; Database = Hotel; Trusted_Connection = True;");
        connection_update.Open();
        var command_update = connection_update.CreateCommand();
        command_update.CommandText = $"UPDATE Room SET room_empty = 'занята' WHERE id = {reader.GetInt32(reader.GetOrdinal("id"))};";
        command_update.ExecuteReader();
        connection_update.Close();
    }
    reader.Close();

    //                                                                   6. Найти самую дешёвую в расчёте на человека комнату и превратить её в Люксовую

    Console.WriteLine("\n\n6. Найти самую дешёвую в расчёте на человека комнату и превратить её в Люксовую: \n");


    command = connection.CreateCommand();
    command.CommandText = "SELECT TOP (1) room.id, CAST(class.cost AS float)/room.number_of_places AS s FROM class, room WHERE room.classID = class.id ORDER BY s";
    reader = command.ExecuteReader();
    while (reader.Read())
    {
        var connection_update = new SqlConnection(@"Server = DESKTOP-MP8QK16; Database = Hotel; Trusted_Connection = True;");
        connection_update.Open();
        var command_update = connection_update.CreateCommand();
        command_update.CommandText = $"UPDATE Room SET ClassID = 3 WHERE id = {reader.GetInt32(reader.GetOrdinal("id"))};";
        command_update.ExecuteReader();
        connection_update.Close();
    }
    reader.Close();

    //                                    7. Найти самый занятый постояльцами класс (отношение числа занятых комнат к общему числу комнат в классе) и добавить в отель новую комнату этого класса

    Console.WriteLine("\n\n7. Найти самый занятый постояльцами класс (отношение числа занятых комнат к общему числу комнат в классе) и добавить в отель новую комнату этого класса: \n");


    command = connection.CreateCommand();
    command.CommandText = "SELECT TOP (1) class.id, CAST(SUM(case room.room_empty when 'занята' then 1 else 0 end) AS float)/COUNT(room.id) AS w FROM room, class WHERE room.classID = class.id GROUP BY class.name, class.id ORDER BY w DESC;";
    reader = command.ExecuteReader();
    while (reader.Read())
    {
        var connection_insert = new SqlConnection(@"Server = DESKTOP-MP8QK16; Database = Hotel; Trusted_Connection = True;");
        connection_insert.Open();
        var command_insert = connection_insert.CreateCommand();
        command_insert.CommandText = $"INSERT INTO Room VALUES('свободна', 1, {reader.GetInt32(reader.GetOrdinal("id"))});";
        command_insert.ExecuteReader();
        connection_insert.Close();
    }
    reader.Close();

    connection.Close();

}

static void LinqDemo()
{
    Database db = new Database();

    //                                                      1. Вывести список свободных комнат (номер, класс), отсортированных по стоимости аренды

    Console.WriteLine("1. Вывести список свободных комнат (номер, класс), отсортированных по стоимости аренды: \n");

    foreach (Room e in db.Rooms.Where(e => e.Room_empty == "свободна").OrderBy(e => e.ClassID.Cost))
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.ClassID.Cost}");
    }

    Console.WriteLine("\n\n");

    //                                                                      2. Рассчитать ежедневную прибыль отеля

    Console.WriteLine("2. Рассчитать ежедневную прибыль отеля: \n");

    Console.WriteLine("Прибыль за сегодня: " + db.Rooms.Where(e => e.Room_empty == "занята").Sum(e => e.ClassID.Cost) + "\n\n");

    //                                                      3. Рассчитать загруженность отеля (отношение числа сданных к общему числу комнат)

    Console.WriteLine("3. Рассчитать загруженность отеля (отношение числа сданных к общему числу комнат): \n");

    /*double size = db.Rooms.Count();
    Console.WriteLine(size);
    double size_not_empty = db.Rooms.Where(e => e.Room_empty == "занята").Count();
    Console.WriteLine(size_not_empty);
    Console.WriteLine("Загруженность отеля: " + size_not_empty/size); */
    Console.WriteLine("Загруженность отеля: " + Convert.ToDouble(db.Rooms.Where(e => e.Room_empty == "занята").Count()) / Convert.ToDouble(db.Rooms.Count()) + "\n\n");

    //                                                                      4. Вывести пары: (класс,общее число комнат)

    Console.WriteLine("4. Вывести пары: (класс,общее число комнат): \n");

    foreach (Class e in db.ClassList)
    {
        Console.WriteLine($"{e.Name}: " + db.Rooms.Where(g => g.ClassID.Id == e.Id).Count() + "\n\n");
    }

    //                                                                      5. Извлечь самую дорогую свободную комнату, пометить как занятую

    Console.WriteLine("5. Извлечь самую дорогую свободную комнату, пометить как занятую: \n");

    int most_expensive_room = db.Rooms.Max(e => Convert.ToInt32(e.ClassID.Cost));

    Console.WriteLine("Было: \n");
    foreach (Room e in db.Rooms)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.Room_empty}");
    }

    foreach (Room e in db.Rooms.Where(e => e.ClassID.Cost == most_expensive_room).Take(1))
    {
        e.Room_empty = "занята";
        //Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.ClassID.Cost}");
    }
    Console.WriteLine("Стало: \n");
    foreach (Room e in db.Rooms)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.Room_empty}");
    }

    //                                                                   6. Найти самую дешёвую в расчёте на человека комнату и превратить её в Люксовую

    Console.WriteLine("\n\n6. Найти самую дешёвую в расчёте на человека комнату и превратить её в Люксовую: \n");

    double the_cheapest_cost = db.Rooms.Min(e => Convert.ToDouble(e.ClassID.Cost) / Convert.ToDouble(e.Number_of_places));
    Console.WriteLine(the_cheapest_cost);

    Console.WriteLine("Было: \n");
    foreach (Room e in db.Rooms)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Number_of_places} {e.ClassID.Cost}");
        //double maxIndex = e.ClassID.Cost.IndexOf(the_cheapest_cost);
    }
    foreach (Room e in db.Rooms)
    {
        if (Convert.ToDouble(e.ClassID.Cost) / Convert.ToDouble(e.Number_of_places) == the_cheapest_cost)
        {
            /*int x = e.Id;
            if (e.Id == x)
            {
                e.ClassID.Name.Where(e => e.Id == x) = "Люкс";
            }*/
            e.ClassID = db.ClassList[2];
            break;
        }
    }

    Console.WriteLine("Стало: \n");
    foreach (Room e in db.Rooms)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Number_of_places} {e.ClassID.Cost}");
    }

    //                                    7. Найти самый занятый постояльцами класс (отношение числа занятых комнат к общему числу комнат в классе) и добавить в отель новую комнату этого класса

    Console.WriteLine("\n\n7. Найти самый занятый постояльцами класс (отношение числа занятых комнат к общему числу комнат в классе) и добавить в отель новую комнату этого класса: \n");

    Console.WriteLine("Было: \n");
    foreach (Room e in db.Rooms)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Room_empty} {e.Number_of_places} {e.ClassID.Cost}");

    }

    foreach (var g in db.ClassList.Select(x => new { res_id = x.Id, count = Convert.ToDouble(db.Rooms.Where(a => a.ClassID.Id == x.Id && a.Room_empty == "занята").Count()) / Convert.ToDouble(db.Rooms.Where(a => a.ClassID.Id == x.Id).Count()) }).OrderByDescending(x => x.count).Take(1))
    {
        int max_id = db.Rooms.Count() + 1;
        Console.WriteLine($"{g.res_id}, {g.count}");
        db.Rooms.Add(new Room { Id = max_id, Room_empty = "свободна", Number_of_places = 1, ClassID = db.ClassList[g.res_id - 1] });
    }

    Console.WriteLine("Стало: \n");
    foreach (Room e in db.Rooms)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Room_empty} {e.Number_of_places} {e.ClassID.Cost}");
    }
}


static void EFDemo()
{
    HotelContext db = new HotelContext();
    db.DropDB();
    db.CreateDbIfNotExist();
    db.Add_ClassList_EF();
    db.SaveChanges();
    db.Add_Rooms_EF();
    db.SaveChanges();

    //                                                      1. Вывести список свободных комнат (номер, класс), отсортированных по стоимости аренды

    Console.WriteLine("1. Вывести список свободных комнат (номер, класс), отсортированных по стоимости аренды: \n");

    foreach (Room_EF e in db.Rooms_EF.Where(e => e.Room_empty == "свободна").OrderBy(e => e.ClassID.Cost))
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.ClassID.Cost}");
    }

    Console.WriteLine("\n\n");

    //                                                                      2. Рассчитать ежедневную прибыль отеля

    Console.WriteLine("2. Рассчитать ежедневную прибыль отеля: \n");

    Console.WriteLine("Прибыль за сегодня: " + db.Rooms_EF.Where(e => e.Room_empty == "занята").Sum(e => e.ClassID.Cost) + "\n\n");

    //                                                      3. Рассчитать загруженность отеля (отношение числа сданных к общему числу комнат)

    Console.WriteLine("3. Рассчитать загруженность отеля (отношение числа сданных к общему числу комнат): \n");

    Console.WriteLine("Загруженность отеля: " + Convert.ToDouble(db.Rooms_EF.Where(e => e.Room_empty == "занята").Count()) / Convert.ToDouble(db.Rooms_EF.Count()) + "\n\n");

    //                                                                      4. Вывести пары: (класс,общее число комнат)

    Console.WriteLine("4. Вывести пары: (класс,общее число комнат): \n");

    foreach (Class_EF e in db.ClassList_EF)
    {
        HotelContext db2 = new HotelContext();
        Console.WriteLine($"{e.Name}: " + db2.Rooms_EF.Where(g => g.ClassID.Id == e.Id).Count() + "\n\n");
        db2.Dispose();
    }

    //                                                                      5. Извлечь самую дорогую свободную комнату, пометить как занятую

    Console.WriteLine("5. Извлечь самую дорогую свободную комнату, пометить как занятую: \n");

    int most_expensive_room = db.Rooms_EF.Max(e => Convert.ToInt32(e.ClassID.Cost));

    Console.WriteLine("Было: \n");
    foreach (Room_EF e in db.Rooms_EF)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.Room_empty}");
    }

    foreach (Room_EF e in db.Rooms_EF.Where(e => e.ClassID.Cost == most_expensive_room).Take(1))
    {
        e.Room_empty = "занята";
        //Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.ClassID.Cost}");
    }
    Console.WriteLine("Стало: \n");
    foreach (Room_EF e in db.Rooms_EF)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Name} {e.Room_empty}");
    }

    //                                                       6.Найти самую дешёвую в расчёте на человека комнату и превратить её в Люксовую

    Console.WriteLine("\n\n6. Найти самую дешёвую в расчёте на человека комнату и превратить её в Люксовую: \n");



    Console.WriteLine("Было: \n");
    foreach (Room_EF e in db.Rooms_EF)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Number_of_places} {e.ClassID.Cost}");
        //double maxIndex = e.ClassID.Cost.IndexOf(the_cheapest_cost);
    }
    db.SaveChanges();
    double the_cheapest_cost = db.Rooms_EF.Min(e => Convert.ToDouble(e.ClassID.Cost) / Convert.ToDouble(e.Number_of_places));
    foreach (Room_EF e in db.Rooms_EF)
    {
        HotelContext db2 = new HotelContext();
        if (Convert.ToDouble(e.ClassID.Cost) / Convert.ToDouble(e.Number_of_places) == the_cheapest_cost)
        {
            e.ClassID = db2.ClassList_EF.First(t => t.Id == 3);
            db2.SaveChanges();
        }

        db2.Dispose();
    }
    Console.WriteLine("Стало: \n");
    foreach (Room_EF e in db.Rooms_EF)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Number_of_places} {e.ClassID.Cost}");
    }

    // 7.Найти самый занятый постояльцами класс(отношение числа занятых комнат к общему числу комнат в классе) и добавить в отель новую комнату этого класса

    Console.WriteLine("\n\n7. Найти самый занятый постояльцами класс (отношение числа занятых комнат к общему числу комнат в классе) и добавить в отель новую комнату этого класса: \n");

    Console.WriteLine("Было: \n");
    foreach (Room_EF e in db.Rooms_EF)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Room_empty} {e.Number_of_places} {e.ClassID.Cost}");
    }
    HotelContext db3 = new HotelContext();
    Class_EF EF_add = db3.ClassList_EF.First(t => t.Id == 1);
    foreach (var g in db3.ClassList_EF.Select(x => new { res_id = x, count = Convert.ToDouble(db3.Rooms_EF.Where(a => a.ClassID.Id == x.Id && a.Room_empty == "занята").Count()) / Convert.ToDouble(db3.Rooms_EF.Where(a => a.ClassID.Id == x.Id).Count()) }).OrderByDescending(n => n.count).Take(1))
    {
        EF_add = g.res_id;
        break;
    }
    db3.Rooms_EF.Add(new Room_EF { Room_empty = "свободна", Number_of_places = 1, ClassID = EF_add });
    db3.SaveChanges();
    Console.WriteLine("Стало: \n");
    foreach (Room_EF e in db.Rooms_EF)
    {
        Console.WriteLine($"{e.Id} {e.ClassID.Id} {e.ClassID.Name} {e.Room_empty} {e.Number_of_places} {e.ClassID.Cost}");
    }
}

//Console.WriteLine("AdoNet:\n");
AdoNetDemo();

//Console.WriteLine("\nLinq:\n");
LinqDemo();

Console.WriteLine("\nEF:\n");
EFDemo();
