using System;
using Thoth.Core.Interfaces;

namespace Thoth.Core.Models;

public class ThothOptions
{
    /// <summary>
    ///     Defines the database provider that will be used to store data
    /// </summary>
    public Lazy<IDatabase> DatabaseProvider { get; set; }

    /// <summary>
    ///     Whether feature flags should be cached, enabled by default
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    ///     Defines if the default value to a non-existent should be false or throw
    /// </summary>
    public bool ShouldReturnFalseWhenNotExists { get; set; } = true;

    /// <summary>
    ///     Defines for how long the feature flags will be cached in memory.
    ///     Distributed applications should set shorten durations.
    /// </summary>
    public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromDays(7);

    /// <summary>
    ///     Defines for how long the feature flags will be cached in memory without any accesses
    ///     Distributed applications should set shorten durations.
    /// </summary>
    public TimeSpan CacheSlidingExpiration { get; set; } = TimeSpan.FromDays(1);
    
    /// <summary>
    ///     Defines if the Thoth Api should be exposed.
    ///     This is required true when using Dashboard.
    /// </summary>
    public bool EnableThothApi { get; set; } = true;
}