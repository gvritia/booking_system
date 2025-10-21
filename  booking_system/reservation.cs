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

    public void Update(string newName, string newPhone, DateTime newStart, DateTime newEnd, string newComment)
    {
        // Для простоты: изменение брони не включает изменение стола.
        // Если меняется время, это должно обрабатываться через отмену и создание новой.
        ClientName = newName;
        ClientPhone = newPhone;
        StartTime = newStart;
        EndTime = newEnd;
        Comment = newComment;
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