using System.Collections.Generic;
using System.Security.Cryptography;
using Domain.Entities;

namespace Domain.Services;

public static class OrderService
{
    public static List<Order> LastOrders { get; }= new List<Order>();

    public static Order CreateTerribleOrder(string customer, string product, int qty, decimal price)
    {
        var o = new Order
        {
            Id = RandomNumberGenerator.GetInt32(1, 9999999), 
            CustomerName = customer, 
            ProductName = product, 
            Quantity = qty, 
            UnitPrice = price
        };
        LastOrders.Add(o);
        return o;
    }
}
