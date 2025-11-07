namespace PortHub.Api.Security
{
   
    [AttributeUsage(AttributeTargets.Method)] // Solo se puede aplicar a métodos (endpoints)
    public class RequireApiKeyAttribute : Attribute
    {
    }
}