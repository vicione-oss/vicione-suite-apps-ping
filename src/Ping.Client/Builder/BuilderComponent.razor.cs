using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using ViciOne.Ui.Blazor.Components.ComboBox;

namespace Ping.Client.Builder;

/// <summary>
/// this is just an experimental page for testing purposes. 
/// therefore it's excluded from code coverage
/// </summary>
[ExcludeFromCodeCoverage]
public sealed partial class BuilderComponent
{
    private ComponentBase? _componentReference;

    private RenderFragment? Child { get; set; }
    private BuilderPageModel Model { get; } = new();

    private void ClearComponent()
    {
        _componentReference = null;
        Child = null;
    }

    private static Type GetClientComponentType(string typeName)
    {
        var assemblies = GetDomainAssembliesWithFullName();

        foreach (var assembly in assemblies)
        {
            var t = assembly.GetType(typeName, false);
            if (t is not null)
                return t;
        }

        throw new ArgumentException(
            "Type " + typeName + " doesn't exist in the current app domain");
    }

    private static IEnumerable<Assembly> GetDomainAssembliesWithFullName()
        => AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !string.IsNullOrEmpty(a.FullName));

    private void InitComponentsForRendering()
    {
        Model.ComponentList.Clear();
        var components = GetDomainAssembliesWithFullName()
            .Where(k => !string.IsNullOrEmpty(k.FullName) &&
                        k.FullName.StartsWith("ViciOne.", StringComparison.Ordinal))
            .SelectMany(
                a => a.GetExportedTypes().Where(t => t.IsClass && t.IsPublic && t.IsSubclassOf(typeof(ComponentBase))))
            .Select(t => t.FullName).ToList();

        var ignore = new string[]
        {
            "Blazor.Shared.Components.App",
            "Blazor.Shared.Components.Routes",
            "Blazor.Shared.Components.Pages.Index.IndexPage",
            "Sdk.Client.Modules.ModuleComponentBase",
            "Sdk.Client.Modules.ModulePageBase`1",
            "Sdk.Client.Modules.NotificationBarElementComponent",
        };

        foreach (var name in components)
        {
            if (string.IsNullOrEmpty(name) || name.Contains("_Imports", StringComparison.Ordinal) || ignore.Contains(name))
                continue;

            Model.ComponentList.Add(new ComboBoxItem<string, string>() { Text = name, Value = name });
        }

        Model.SelectFirstComponent();
    }

    protected override void OnAfterInitialized()
        => InitComponentsForRendering();

    private void RenderComponent(string? componentName)
    {
        try
        {
            if (string.IsNullOrEmpty(componentName))
                throw new ArgumentException("Component is not specified");

            Model.IsLoading = true;

            var componentType = GetClientComponentType(componentName);
            if (!componentType.IsSubclassOf(typeof(ComponentBase)))
            {
                Model.BuildResult = $"{componentName} is no component!";
                return;
            }

            Child = builder =>
            {
                builder.OpenComponent(0, componentType);
                builder.AddComponentReferenceCapture(1, inst => { _componentReference = (ComponentBase)inst; });
                builder.CloseComponent();
            };

            Model.BuildResult = componentType.AssemblyQualifiedName;
        }
        catch (Exception e)
        {
            Model.BuildResult = e.Message;
        }
        finally
        {
            Model.IsLoading = false;
        }
    }
}
