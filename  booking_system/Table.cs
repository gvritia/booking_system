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
        // Убедимся, что проверяем только полные часы
        int startHour = startTime.Hour;
        int endHour = endTime.Hour;

        // Если бронь на 19:00 - 20:00, проверяем только час 19
        // Если бронь на 19:30 - 20:30, проверяем час 19 и час 20
        // Для простоты, оставим проверку до < endHour
        
        for (int hour = startHour; hour < endHour; hour++)
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
        int startHour = startTime.Hour;
        int endHour = endTime.Hour;

        for (int hour = startHour; hour < endHour; hour++)
        {
            if (Schedule.ContainsKey(hour))
            {
                // ИСПРАВЛЕНИЕ/УЛУЧШЕНИЕ: Убедиться, что мы не перезаписываем чужую бронь
                if (isBooking)
                {
                    // Записываем, только если свободно или если это наша бронь (для Update)
                    if (Schedule[hour] == 0 || Schedule[hour] == reservationId)
                    {
                         Schedule[hour] = reservationId;
                    }
                    else
                    {
                        // В реальной системе здесь должна быть ошибка/исключение 
                        // о попытке занять уже забронированный час.
                    }
                }
                else
                {
                    // Отменяем, только если это наша бронь
                    if (Schedule[hour] == reservationId)
                    {
                        Schedule[hour] = 0;
                    }
                }
            }
        }
    }
    
    // Форматированный вывод информации о столе
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
                // ПРИМЕЧАНИЕ: Этот код предполагает, что BookingSystem.Reservations доступен 
                // и содержит актуальные данные.
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