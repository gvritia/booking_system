using System;
using System.Collections.Generic;
using System.Linq;

public enum TableLocation
{
    У_Окна, У_Прохода, У_Выхода, В_Глубине
}

public class Table
{
    public int ID { get; private set; }
    public TableLocation Location { get; set; }
    public int Capacity { get; set; } // Количество мест

    // Ключ: час (0-23), Значение: ID бронирования или 0 (свободно)
    public Dictionary<int, int> Schedule { get; private set; } 

    public Table(int id, TableLocation location, int capacity)
    {
        ID = id;
        Location = location;
        Capacity = capacity;
        Schedule = new Dictionary<int, int>();
        for (int hour = 9; hour < 23; hour++) 
        {
            Schedule.Add(hour, 0); 
        }
    }

    public void UpdateInfo(TableLocation newLocation, int newCapacity)
    {
        Location = newLocation;
        Capacity = newCapacity;
    }

    public bool IsAvailable(DateTime startTime, DateTime endTime)
    {
        for (int hour = startTime.Hour; hour < endTime.Hour; hour++)
        {
            if (Schedule.ContainsKey(hour) && Schedule[hour] != 0)
            {
                return false;
            }
        }
        return true;
    }

    public void UpdateSchedule(DateTime startTime, DateTime endTime, int reservationId, bool isBooking)
    {
        for (int hour = startTime.Hour; hour < endTime.Hour; hour++)
        {
            if (Schedule.ContainsKey(hour))
            {
                Schedule[hour] = isBooking ? reservationId : 0;
            }
        }
    }
    
    // Форматированный вывод информации о столе (по требованию задания)
    public string GetDetailedInfo()
    {
        var info = $"ID: {ID:D2}\n" +
                   $"Расположение: {Location.ToString().Replace("_", " ")}\n" +
                   $"Количество мест: {Capacity}\n" +
                   "Расписание:\n";
        
        foreach (var entry in Schedule.OrderBy(s => s.Key))
        {
            info += $"{entry.Key:D2}:00 - {entry.Key + 1:D2}:00 --- ";
            if (entry.Value != 0)
            {
                var res = BookingSystem.Reservations.FirstOrDefault(r => r.ID == entry.Value);
                if (res != null)
                {
                    info += $"ID {res.ID}, {res.ClientName}, {res.ClientPhone}\n";
                }
                else
                {
                    info += "Занято (Бронь удалена)\n";
                }
            }
            else
            {
                info += "Свободно\n";
            }
        }
        return info;
    }
}