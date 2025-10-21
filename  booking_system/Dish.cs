using System.Collections.Generic;

public enum DishCategory
{
    Напитки, Салаты, Холодные_Закуски, Горячие_Закуски, Супы, Горячие_Блюда, Десерт, Другое
}

public class Dish
{
    public int ID { get; private set; }
    public string Name { get; set; }
    public string Ingredients { get; set; }
    public string Weight { get; set; } 
    public decimal Price { get; set; }
    public DishCategory Category { get; set; }
    public int CookingTimeMinutes { get; set; }
    public List<string> Types { get; set; } 

    public Dish(int id, string name, string ingredients, string weight, decimal price, DishCategory category, int cookingTime, List<string> types)
    {
        ID = id;
        Name = name;
        Ingredients = ingredients;
        Weight = weight;
        Price = price;
        Category = category;
        CookingTimeMinutes = cookingTime;
        Types = types ?? new List<string>();
    }

    public void UpdateInfo(string name, string ingredients, string weight, decimal price, DishCategory category, int cookingTime, List<string> types)
    {
        Name = name;
        Ingredients = ingredients;
        Weight = weight;
        Price = price;
        Category = category;
        CookingTimeMinutes = cookingTime;
        Types = types ?? new List<string>();
    }
}