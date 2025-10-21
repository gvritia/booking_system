using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OrderItem
{
    public Dish Dish { get; set; }
    public int Quantity { get; set; }
    public decimal ItemTotal => Dish.Price * Quantity;
}

public class Order
{
    public int ID { get; private set; }
    public int TableID { get; set; }
    public List<OrderItem> Items { get; private set; } 
    public string Comment { get; set; }
    public DateTime AcceptanceTime { get; private set; }
    public int WaiterID { get; set; }
    public DateTime? ClosureTime { get; private set; }
    public decimal TotalCost { get; private set; }

    public Order(int id, int tableId, int waiterId)
    {
        ID = id;
        TableID = tableId;
        WaiterID = waiterId;
        Items = new List<OrderItem>();
        AcceptanceTime = DateTime.Now;
        ClosureTime = null;
        TotalCost = 0;
    }
    
    public void AddItem(Dish dish, int quantity)
    {
        Items.Add(new OrderItem { Dish = dish, Quantity = quantity });
        RecalculateTotal();
    }

    public void RemoveItem(int dishId, int quantityToRemove)
    {
        var item = Items.FirstOrDefault(i => i.Dish.ID == dishId);
        if (item != null)
        {
            item.Quantity -= quantityToRemove;
            if (item.Quantity <= 0)
            {
                Items.Remove(item);
            }
        }
        RecalculateTotal();
    }
    
    private void RecalculateTotal()
    {
        TotalCost = Items.Sum(item => item.ItemTotal);
    }
    
    public void CloseOrder()
    {
        ClosureTime = DateTime.Now;
        RecalculateTotal();
    }
    
    public string GenerateReceipt(int tableId, int waiterId)
    {
        if (ClosureTime == null) return "Заказ не закрыт. Чек недоступен.";

        var sb = new StringBuilder();
        sb.AppendLine("*************************************************");
        sb.AppendLine($"Столик: {tableId}");
        sb.AppendLine($"Официант: ID {waiterId}");
        sb.AppendLine($"Период обслуживания: с {AcceptanceTime:HH:mm} по {ClosureTime:HH:mm}");
        sb.AppendLine();
        
        var groupedItems = Items.GroupBy(item => item.Dish.Category).OrderBy(g => g.Key);

        foreach (var group in groupedItems)
        {
            decimal categorySubtotal = 0;
            sb.AppendLine($"Категория блюда: {group.Key.ToString().Replace("_", " ")}:");
            foreach (var item in group)
            {
                categorySubtotal += item.ItemTotal;
                sb.AppendLine($"{item.Dish.Name} {item.Quantity}*{item.Dish.Price:C} = {item.ItemTotal:C}");
            }
            sb.AppendLine($"Под_итог категории: {categorySubtotal:C}");
            sb.AppendLine();
        }

        sb.AppendLine($"Итог счета: {TotalCost:C}");
        sb.AppendLine("*************************************************");
        
        return sb.ToString();
    }
}