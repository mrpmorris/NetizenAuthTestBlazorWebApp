using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;

namespace NetizenAuthTestBlazorWebApp.Extensions;

public static class IComponentExtensions
{
    private static readonly ConcurrentDictionary<Type, string> ComponentTypeToJSFilePathLookup = new();

    public static string GetJSFilePath(this IComponent component) =>
        ComponentTypeToJSFilePathLookup.GetOrAdd(
            key: component.GetType(),
            valueFactory: componentType =>
            {
                string componentAssemblyDefaultNamespace = componentType.Assembly.GetName().Name!;
                string componentNamespace = componentType.FullName!;
                string componentRelativeNamespace = componentNamespace.Substring(componentAssemblyDefaultNamespace.Length);
                string result = componentRelativeNamespace.Replace('.', '/') + ".razor.js";
                return result;
            });
}
