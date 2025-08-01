﻿namespace Minfys.Services;

/// <summary>
/// Provides a contract for saving and loading settings.
/// </summary>
public interface IOptionsService
{
    /// <summary>
    /// Loads Options from userPreferences.json. 
    /// </summary>
    /// <param name="sectionKey">The name of the options section.</param>
    /// <returns>Returns null if there is an error during loading. Otherwise, returns specific options type object.</returns>
    TOptions? Load<TOptions>(string? sectionKey = null) where TOptions : class, new();

    /// <summary>
    /// Saves options in userPreferences.json.
    /// </summary>
    /// <param name="options">Option type to save.</param>
    /// <param name="sectionKey">The name of the options section.</param>
    void Save<TOptions>(TOptions options, string? sectionKey = null) where TOptions : class;
}