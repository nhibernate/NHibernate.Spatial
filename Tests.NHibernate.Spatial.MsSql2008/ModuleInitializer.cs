// .NET
using System;
using System.Reflection;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        SetEntryAssembly();
    }

    private static void SetEntryAssembly()
    {
        // Ensures that Assembly.GetEntryAssembly() does not return null when running unit tests
        // See: http://stackoverflow.com/a/21888521/3628232
        var assembly = Assembly.GetCallingAssembly();

        var manager = new AppDomainManager();
        var entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
        entryAssemblyfield.SetValue(manager, assembly);

        var domain = AppDomain.CurrentDomain;
        var domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
        domainManagerField.SetValue(domain, manager);
    }
}
