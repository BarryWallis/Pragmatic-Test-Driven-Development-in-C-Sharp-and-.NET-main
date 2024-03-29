﻿namespace Uqs.Weather.Wrappers;

/// <summary>
/// Wrapper for Random.Share to support dependency injection.
/// </summary>
public class RandomWrapper : IRandomWrapper
{
    private readonly Random _random = Random.Shared;

    public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);
}
