//-----------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// Source: https://github.com/microsoft/MinIoC
//-----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SharpVectors.IoC
{
    /// <summary>
    /// Inversion of control container handles dependency injection for registered types
    /// </summary>
    public sealed class SvgContainer : ISvgScope
    {
        private bool _isDisposed;
        // Map of registered types
        private readonly Dictionary<Type, Func<ILifetime, object>> _registeredTypes;

        // Lifetime management
        private readonly ContainerLifetime _lifetime;

        /// <summary>
        /// Creates a new instance of IoC Container
        /// </summary>
        public SvgContainer()
        {
            _registeredTypes = new Dictionary<Type, Func<ILifetime, object>>();
            _lifetime        = new ContainerLifetime(t => _registeredTypes[t]);
        }

        ~SvgContainer()
        {
            this.Dispose(false);
        }

        public bool IsDisposed
        {
            get {
                return _isDisposed;
            }
        }

        /// <summary>
        /// Registers a factory function which will be called to resolve the specified interface
        /// </summary>
        /// <param name="anInterface">Interface to register</param>
        /// <param name="factory">Factory function</param>
        /// <returns></returns>
        public ISvgRegisteredType Register(Type anInterface, Func<object> factory)
        {
            return RegisterType(anInterface, _ => factory());
        }

        /// <summary>
        /// Registers an implementation type for the specified interface
        /// </summary>
        /// <param name="anInterface">Interface to register</param>
        /// <param name="implementation">Implementing type</param>
        /// <returns></returns>
        public ISvgRegisteredType Register(Type anInterface, Type implementation)
        {
            return RegisterType(anInterface, FactoryFromType(implementation));
        }

        private ISvgRegisteredType RegisterType(Type itemType, Func<ILifetime, object> factory)
        {
            return new RegisteredType(itemType, f => _registeredTypes[itemType] = f, factory);
        }

        /// <summary>
        /// Returns the object registered for the given type
        /// </summary>
        /// <param name="type">Type as registered with the container</param>
        /// <returns>Instance of the registered type</returns>
        public object GetService(Type type)
        {
            if (_isDisposed || _lifetime.IsDisposed)
            {
                return null;
            }
            return _registeredTypes[type](_lifetime);
        }

        /// <summary>
        /// Creates a new scope
        /// </summary>
        /// <returns>Scope object</returns>
        public ISvgScope CreateScope()
        {
            if (_isDisposed || _lifetime.IsDisposed)
            {
                return null;
            }
            return new ScopeLifetime(_lifetime);
        }

        /// <summary>
        /// Disposes any <see cref="IDisposable"/> objects owned by this container.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lifetime != null)
                {
                    _lifetime.Dispose();
                }
            }

            _isDisposed = true;
        }
        
        #region Lifetime management

        // ILifetime management adds resolution strategies to an IScope
        private interface ILifetime : ISvgScope
        {
            object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory);

            object GetServicePerScope(Type type, Func<ILifetime, object> factory);
        }

        // ObjectCache provides common caching logic for lifetimes
        private abstract class ObjectCache : IDisposable
        {
            private bool _isDisposed;
            // Instance cache
            private readonly ConcurrentDictionary<Type, object> _instanceCache;

            protected ObjectCache()
            {
                _instanceCache = new ConcurrentDictionary<Type, object>();
            }

            ~ObjectCache()
            {
                this.Dispose(false);
            }

            public bool IsDisposed
            {
                get {
                    return _isDisposed;
                }
            }

            // Get from cache or create and cache object
            protected object GetCached(Type type, Func<ILifetime, object> factory, ILifetime lifetime)
            {
                return _instanceCache.GetOrAdd(type, _ => factory(lifetime));
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    foreach (var obj in _instanceCache.Values)
                    {
                        (obj as IDisposable)?.Dispose();
                    }
                }

                _isDisposed = true;
            }
        }

        // Container lifetime management
        private sealed class ContainerLifetime : ObjectCache, ILifetime
        {
            // Retrieves the factory functino from the given type, provided by owning container
            public Func<Type, Func<ILifetime, object>> GetFactory { get; private set; }

            public ContainerLifetime(Func<Type, Func<ILifetime, object>> getFactory)
            {
                GetFactory = getFactory;
            }

            public object GetService(Type type) => GetFactory(type)(this);

            // Singletons get cached per container
            public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
            {
                return GetCached(type, factory, this);
            }

            // At container level, per-scope items are equivalent to singletons
            public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
            {
                return GetServiceAsSingleton(type, factory);
            }
        }

        // Per-scope lifetime management
        private sealed class ScopeLifetime : ObjectCache, ILifetime
        {
            // Singletons come from parent container's lifetime
            private readonly ContainerLifetime _parentLifetime;

            public ScopeLifetime(ContainerLifetime parentContainer)
            {
                _parentLifetime = parentContainer;
            }

            public object GetService(Type type) => _parentLifetime.GetFactory(type)(this);

            // Singleton resolution is delegated to parent lifetime
            public object GetServiceAsSingleton(Type type, Func<ILifetime, object> factory)
            {
                return _parentLifetime.GetServiceAsSingleton(type, factory);
            }

            // Per-scope objects get cached
            public object GetServicePerScope(Type type, Func<ILifetime, object> factory)
            {
                return GetCached(type, factory, this);
            }
        }

        #endregion

        #region Container items

        // Compiles a lambda that calls the given type's first constructor resolving arguments
        private static Func<ILifetime, object> FactoryFromType(Type itemType)
        {
            // Get first constructor for the type
            var constructors = itemType.GetConstructors();
            if (constructors.Length == 0)
            {
                // If no public constructor found, search for an internal constructor
                constructors = itemType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            }
            var constructor = constructors.First();

            // Compile constructor call as a lambda expression
            var arg = Expression.Parameter(typeof(ILifetime));
            return (Func<ILifetime, object>)Expression.Lambda(
                Expression.New(constructor, constructor.GetParameters().Select(
                    param =>
                    {
                        var resolve = new Func<ILifetime, object>(
                            lifetime => lifetime.GetService(param.ParameterType));
                        return Expression.Convert(
                            Expression.Call(Expression.Constant(resolve.Target), resolve.Method, arg),
                            param.ParameterType);
                    })),
                arg).Compile();
        }

        // RegisteredType is supposed to be a short lived object tying an item to its container
        // and allowing users to mark it as a singleton or per-scope item
        private sealed class RegisteredType : ISvgRegisteredType
        {
            private readonly Type _itemType;
            private readonly Action<Func<ILifetime, object>> _registerFactory;
            private readonly Func<ILifetime, object> _factory;

            public RegisteredType(Type itemType, Action<Func<ILifetime, object>> registerFactory, 
                Func<ILifetime, object> factory)
            {
                _itemType        = itemType;
                _registerFactory = registerFactory;
                _factory         = factory;

                registerFactory(_factory);
            }

            public void AsSingleton()
            {
                _registerFactory(lifetime => lifetime.GetServiceAsSingleton(_itemType, _factory));
            }

            public void PerScope()
            {
                _registerFactory(lifetime => lifetime.GetServicePerScope(_itemType, _factory));
            }
        }

        #endregion
    }
}