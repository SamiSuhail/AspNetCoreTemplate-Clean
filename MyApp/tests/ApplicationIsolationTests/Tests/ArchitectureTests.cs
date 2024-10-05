using System.Reflection;
using MyApp.Application;
using MyApp.Application.Interfaces;
using MyApp.Domain;
using MyApp.Infrastructure;
using MyApp.Presentation;
using MyApp.Server;

namespace MyApp.ApplicationIsolationTests.Tests;

public class ArchitectureTests
{
    private readonly Assembly _domainAssembly = typeof(IDomainAssemblyMarker).Assembly;
    private readonly Assembly _applicationAssembly = typeof(IApplicationAssemblyMarker).Assembly;
    private readonly Assembly _applicationInterfacesAssembly = typeof(IApplicationInterfacesAssemblyMarker).Assembly;
    private readonly Assembly _infrastructureAssembly = typeof(IInfrastructureAssemblyMarker).Assembly;
    private readonly Assembly _presentationAssembly = typeof(IPresentationAssemblyMarker).Assembly;
    private readonly Assembly _serverAssembly = typeof(IServerAssemblyMarker).Assembly;

    private readonly Assembly[] _mainProjectAssemblies = [
        typeof(IDomainAssemblyMarker).Assembly,
        typeof(IApplicationAssemblyMarker).Assembly,
        typeof(IInfrastructureAssemblyMarker).Assembly,
        typeof(IPresentationAssemblyMarker).Assembly,
        typeof(IServerAssemblyMarker).Assembly
        ];

    [Fact]
    public void Domain_Should_Have_No_Project_References()
    {
        var references = GetReferencedProjects(_domainAssembly);

        references.Should().BeEmpty();
    }

    [Fact]
    public void Application_Should_Only_Reference_Domain()
    {
        var references = GetReferencedProjects(_applicationAssembly);

        references.Should().HaveCount(1);
        references.Should().Contain(r => r.Name == GetAssemblyName(_domainAssembly));
    }

    [Fact]
    public void Infrastructure_Should_Only_Reference_Application_Domain()
    {
        var references = GetReferencedProjects(_infrastructureAssembly);

        references.Should().HaveCount(2);
        references.Should().Contain(r => r.Name == GetAssemblyName(_applicationAssembly));
        references.Should().Contain(r => r.Name == GetAssemblyName(_domainAssembly));
    }

    [Fact]
    public void Presentation_Should_Only_Reference_Application_Domain()
    {
        var references = _presentationAssembly.GetReferencedAssemblies()
            .In([.. _mainProjectAssemblies, _applicationInterfacesAssembly]);

        references.Should().HaveCount(2);
        references.Should().Contain(r => r.Name == GetAssemblyName(_applicationInterfacesAssembly));
        references.Should().Contain(r => r.Name == GetAssemblyName(_domainAssembly));
    }

    [Fact]
    public void Server_Should_Only_Reference_Presentation_Infrastructure_Application()
    {
        var references = GetReferencedProjects(_serverAssembly);

        references.Should().HaveCount(3);
        references.Should().Contain(r => r.Name == GetAssemblyName(_infrastructureAssembly));
        references.Should().Contain(r => r.Name == GetAssemblyName(_presentationAssembly));
        references.Should().Contain(r => r.Name == GetAssemblyName(_applicationAssembly));
    }
    
    private string GetAssemblyName(Assembly assembly) => assembly.GetName().Name ?? throw new Exception("Assembly has null name.");

    private AssemblyName[] GetReferencedProjects(Assembly assembly)
        => assembly.GetReferencedAssemblies()
            .In(_mainProjectAssemblies)
            .ToArray();
}

file static class AssemblyExtensions
{
    public static IEnumerable<AssemblyName> In(this IEnumerable<AssemblyName> assemblies, params Assembly[] filters)
        => assemblies.Where(a => filters.Any(projectAssembly => projectAssembly.FullName == a.FullName));
}