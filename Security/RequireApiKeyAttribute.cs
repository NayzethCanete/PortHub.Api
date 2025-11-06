namespace PortHub.Api.Security
{
    /// <summary>
    /// Atributo "marcador" para indicar que un endpoint
    /// requiere validación de API Key.
    /// El middleware ApiKeyAuthMiddleware buscará este atributo.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] // Solo se puede aplicar a métodos (endpoints)
    public class RequireApiKeyAttribute : Attribute
    {
    }
}