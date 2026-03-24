

// BAD: Mixing minimal APIs with Controllers folder just to confuse structure
namespace WebApi.Controllers
{
    public static class OrdersController /* No ControllerBase, no attributes: unused on purpose */ 
    {
        public const string Message = "This controller does nothing. Endpoints are in Program.cs";
    }
}
