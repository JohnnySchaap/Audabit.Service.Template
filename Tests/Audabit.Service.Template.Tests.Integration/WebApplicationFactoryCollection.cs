using Microsoft.AspNetCore.Mvc.Testing;

namespace Audabit.Service.Template.Tests.Integration;

/// <summary>
/// Collection fixture definition for sharing WebApplicationFactory across integration test classes.
/// This prevents the static ServiceName from being initialized multiple times.
/// </summary>
[CollectionDefinition(nameof(WebApplicationFactoryCollection))]
public class WebApplicationFactoryCollection : ICollectionFixture<WebApplicationFactory<Program>>;