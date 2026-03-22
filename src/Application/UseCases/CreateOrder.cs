using System.Threading;
using System;
using Domain.Entities;
using Domain.Services;


namespace Application.UseCases;

public class CreateOrderUseCase
{
    public Order Execute(string customer, string product, int qty, decimal price)
    { 
        return OrderService.CreateTerribleOrder(customer, product, qty, price);
    }
}

