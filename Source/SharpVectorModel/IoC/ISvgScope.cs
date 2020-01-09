//-----------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Source: https://github.com/microsoft/MinIoC
//-----------------------------------------------------------------------------------

using System;

namespace SharpVectors.IoC
{
    /// <summary>
    /// Represents a scope in which per-scope objects are instantiated a single time
    /// </summary>
    public interface ISvgScope : IDisposable, IServiceProvider
    {
        bool IsDisposed { get; }
    }
}
