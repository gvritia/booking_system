using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class OrderingSystem
{
    public static List<Dish> Menu = new List<Dish>();
    public static List<Order> Orders = new List<Order>();
    private static int nextDishId = 1;
    private static int nextOrderId = 1;

    public static void InitializeMenu()
    {
        if (Menu.Count == 0)
        {
            Menu.Add(new Dish(nextDishId++, "Цезарь", "Салат, курица, соус", "300", 450.00m, DishCategory.Салаты, 10, new List<string>()));
            Menu.Add(new Dish(nextDishId++, "Борщ", "Говядина, свекла", "350", 350.00m, DishCategory.Супы, 30, new List<string>()));
            Menu.Add(new Dish(nextDishId++, "Кола", "Напиток", "330", 150.00m, DishCategory.Напитки, 1, new List<string>()));
            Menu.Add(new Dish(nextDishId++, "Стейк", "Мясо, специи", "400", 1200.00m, DishCategory.Горячие_Блюда, 40, new List<string> { "острое" }));
        }
    }
    
    public static void CreateDish(string name, string ingredients, string weight, decimal price, DishCategory category, int cookingTime, List<string> types)
    {
        Menu.Add(new Dish(nextDishId++, name, ingredients, weight, price, category, cookingTime, types));
    }
    
    public static bool DeleteDish(int dishId)
    {
        var count = Menu.RemoveAll(d => d.ID == dishId);
        return count > 0;
    }
    
    public static Order CreateOrder(int tableId, int waiterId)
    {
        var order = new Order(nextOrderId++, tableId, waiterId);
        Orders.Add(order);
        return order;
    }

    public static string PrintMenu()
    {
        var sb = new StringBuilder();
        var groupedMenu = Menu.OrderBy(d => d.Category).GroupBy(d => d.Category);

        foreach (var group in groupedMenu)
        {
            sb.AppendLine($"\n*** {group.Key.ToString().Replace("_", " ")} ***");
            foreach (var dish in group)
            {
                sb.AppendLine($"  {dish.Name} ({dish.Weight}) - {dish.Price:C}");
                sb.AppendLine($"    Состав: {dish.Ingredients}");
                if (dish.Types.Count > 0)
                {
                    sb.AppendLine($"    Тип: {string.Join(", ", dish.Types)}");
                }
                sb.AppendLine($"    Время готовки: {dish.CookingTimeMinutes} мин.");
            }
        }
        return sb.ToString();
    }
    
    public static decimal GetTotalRevenue()
    {
        return Orders.Where(o => o.ClosureTime.HasValue).Sum(o => o.TotalCost);
    }
    
    public static int CountClosedOrdersByWaiter(int waiterId)
    {
        return Orders.Count(o => o.ClosureTime.HasValue && o.WaiterID == waiterId);
    }
    
    public static Dictionary<string, int> GetDishStatistics()
    {
        var stats = new Dictionary<string, int>();
        var closedOrders = Orders.Where(o => o.ClosureTime.HasValue);
        
        foreach (var order in closedOrders)
        {
            foreach (var item in order.Items)
            {
                if (stats.ContainsKey(item.Dish.Name))
                {
                    stats[item.Dish.Name] += item.Quantity;
                }
                else
                {
                    stats.Add(item.Dish.Name, item.Quantity);
                }
            }
        }
        return stats.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
    }
}