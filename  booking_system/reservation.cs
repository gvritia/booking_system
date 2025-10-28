using System;

public class Reservation
{
    public int ID { get; private set; }
    public string ClientName { get; set; }
    public string ClientPhone { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Comment { get; set; }
    public Table AssignedTable { get; set; }

    public Reservation(int id, string name, string phone, DateTime start, DateTime end, Table table, string comment = "")
    {
        ID = id;
        ClientName = name;
        ClientPhone = phone;
        StartTime = start;
        EndTime = end;
        AssignedTable = table;
        Comment = comment;

        // Внесение изменений в объект класса "Стол"
        AssignedTable.UpdateSchedule(StartTime, EndTime, ID, true);
    }

    // ИСПРАВЛЕНИЕ: Корректная обработка изменения времени бронирования
    public void Update(string newName, string newPhone, DateTime newStart, DateTime newEnd, string newComment)
    {
        bool timeChanged = StartTime != newStart || EndTime != newEnd;

        if (timeChanged && AssignedTable != null)
        {
            // Сначала отменяем старую бронь в расписании
            AssignedTable.UpdateSchedule(StartTime, EndTime, ID, false); 
        }

        ClientName = newName;
        ClientPhone = newPhone;
        StartTime = newStart;
        EndTime = newEnd;
        Comment = newComment;

        if (timeChanged && AssignedTable != null)
        {
            // Затем записываем новую бронь в расписание
            // Требуется дополнительная проверка на доступность стола в UI/BookingSystem перед вызовом!
            AssignedTable.UpdateSchedule(StartTime, EndTime, ID, true); 
        }
    }

    public void Cancel()
    {
        if (AssignedTable != null)
        {
            AssignedTable.UpdateSchedule(StartTime, EndTime, ID, false); 
            AssignedTable = null;
        }
    }
}