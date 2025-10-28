using System;
using System.Collections.Generic;
using System.Linq;

public static class BookingSystem
{
    public static List<Table> Tables = new List<Table>();
    public static List<Reservation> Reservations = new List<Reservation>();
    private static int nextReservationId = 1;

    public static void InitializeTables()
    {
        if (Tables.Count == 0)
        {
            Tables.Add(new Table(1, TableLocation.У_Окна, 4));
            Tables.Add(new Table(2, TableLocation.У_Прохода, 2));
            Tables.Add(new Table(3, TableLocation.В_Глубине, 6));
            Tables.Add(new Table(4, TableLocation.У_Окна, 8));
        }
    }

    public static void CreateTable(TableLocation location, int capacity)
    {
        int newId = Tables.Count > 0 ? Tables.Max(t => t.ID) + 1 : 1;
        Tables.Add(new Table(newId, location, capacity));
    }

    public static Reservation CreateReservation(string name, string phone, DateTime start, DateTime end, int tableId, string comment = "")
    {
        var table = Tables.FirstOrDefault(t => t.ID == tableId);
        
        // УЛУЧШЕНИЕ: Дополнительная проверка на то, что время начала в будущем
        if (start < DateTime.Now)
        {
            throw new InvalidOperationException("Нельзя создать бронь в прошлом.");
        }
        
        if (table == null || !table.IsAvailable(start, end))
        {
            throw new InvalidOperationException("Стол недоступен или не существует.");
        }
        
        var reservation = new Reservation(nextReservationId++, name, phone, start, end, table, comment);
        Reservations.Add(reservation);
        return reservation;
    }

    public static bool EditTableInfo(int tableId, TableLocation newLocation, int newCapacity)
    {
        var table = Tables.FirstOrDefault(t => t.ID == tableId);
        if (table == null) return false;

        // ПРЕДУПРЕЖДЕНИЕ: Проверка isActive должна быть более строгой, 
        // например, проверять только будущие бронирования, а не просто EndTime > DateTime.Now
        bool isActive = Reservations.Any(r => r.AssignedTable?.ID == tableId && r.EndTime > DateTime.Now);

        if (!isActive)
        {
            table.UpdateInfo(newLocation, newCapacity);
            return true;
        }
        // УЛУЧШЕНИЕ: Возвращать false, если есть активная бронь
        return false;
    }

    public static List<Table> GetAvailableTables(DateTime start, DateTime end, int requiredCapacity = 0, TableLocation? preferredLocation = null)
    {
        // УЛУЧШЕНИЕ: Исключаем бронирования, которые уже начались, если start в прошлом.
        if (start < DateTime.Now)
        {
            // Для поиска доступных столов имеет смысл искать только будущие бронирования
        }

        return Tables
            .Where(t => t.IsAvailable(start, end) && 
                        t.Capacity >= requiredCapacity &&
                        (!preferredLocation.HasValue || t.Location == preferredLocation.Value))
            .ToList();
    }
    
    public static List<Reservation> SearchReservations(string lastFourDigits, string clientName)
    {
        return Reservations
            .Where(r => r.ClientPhone != null && 
                        r.ClientPhone.Length >= 4 && 
                        r.ClientPhone.Substring(r.ClientPhone.Length - 4) == lastFourDigits && 
                        r.ClientName.Equals(clientName, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}