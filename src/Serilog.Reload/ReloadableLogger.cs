using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Serilog.Core;
using Serilog.Events;

// ReSharper disable MemberCanBePrivate.Global

namespace Serilog.Reload
{
    public sealed class ReloadableLogger : ILogger, IReloadableLogger, IDisposable
    {
        readonly object _sync = new object();
        Func<LoggerConfiguration, LoggerConfiguration> _configure;
        Logger _logger;
        
        // One-way; if the value is `true` it can never again be made `false`, allowing "double-checked" reads. If
        // `true`, `_logger` is final and a memory barrier ensures the final value is seen by all threads.
        bool _frozen;

        public ReloadableLogger(Func<LoggerConfiguration, LoggerConfiguration> configure)
        {
            _configure = configure ?? throw new ArgumentNullException(nameof(configure));
            _logger = _configure(new LoggerConfiguration()).CreateLogger();
        }

        ILogger IReloadableLogger.ReloadLogger()
        {
            return _logger;
        }

        public void Reload()
        {
            lock (_sync)
            {
                _logger.Dispose();
                _logger = _configure(new LoggerConfiguration()).CreateLogger();
            }
        }

        public void Reload(Func<LoggerConfiguration, LoggerConfiguration> configure)
        {
            if (configure == null) throw new ArgumentNullException(nameof(configure));
            
            lock (_sync)
            {
                _configure = configure;
                Reload();
            }
        }

        public Logger Freeze()
        {
            lock (_sync)
            {
                if (_frozen)
                    throw new InvalidOperationException("The logger is already frozen.");

                _frozen = true;
                
                // https://github.com/dotnet/runtime/issues/20500#issuecomment-284774431
                // Publish `_logger` and `_frozen`. This is useful here because it means that once the logger is frozen - which
                // we always expect - reads don't require any synchronization/interlocked instructions.
                Interlocked.MemoryBarrierProcessWide();
                
                return _logger;
            }
        }

        public void Dispose()
        {
            lock (_sync)
                _logger.Dispose();
        }

        public ILogger ForContext(ILogEventEnricher enricher)
        {
            if (enricher == null) return this;
            
            if (_frozen)
                return _logger.ForContext(enricher);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(enricher));
        }

        public ILogger ForContext(IEnumerable<ILogEventEnricher> enrichers)
        {
            if (enrichers == null) return this;
            
            if (_frozen)
                return _logger.ForContext(enrichers);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(enrichers));
        }

        public ILogger ForContext(string propertyName, object value, bool destructureObjects = false)
        {
            if (propertyName == null) return this;
            
            if (_frozen)
                return _logger.ForContext(propertyName, value, destructureObjects);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(propertyName, value, destructureObjects));
        }

        public ILogger ForContext<TSource>()
        {
            if (_frozen)
                return _logger.ForContext<TSource>();

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext<TSource>());
        }

        public ILogger ForContext(Type source)
        {
            if (source == null) return this;
            
            if (_frozen)
                return _logger.ForContext(source);

            lock (_sync)
                return new CachingReloadableLogger(this, _logger, this, p => p.ForContext(source));
        }

        public void Write(LogEvent logEvent)
        {
            if (_frozen)
            {
                _logger.Write(logEvent);
                return;
            }

            lock (_sync)
            {
                _logger.Write(logEvent);
            }
        }

        public void Write(LogEventLevel level, string messageTemplate)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate);
            }
        }

        public void Write<T>(LogEventLevel level, string messageTemplate, T propertyValue)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValue);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValue);
            }
        }

        public void Write<T0, T1>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
            }
        }

        public void Write<T0, T1, T2>(LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
            }
        }

        public void Write(LogEventLevel level, string messageTemplate, params object[] propertyValues)
        {
            if (_frozen)
            {
                _logger.Write(level, messageTemplate, propertyValues);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, messageTemplate, propertyValues);
            }
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate);
            }
        }

        public void Write<T>(LogEventLevel level, Exception exception, string messageTemplate, T propertyValue)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue);
            }
        }

        public void Write<T0, T1>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
            }
        }

        public void Write<T0, T1, T2>(LogEventLevel level, Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
            }
        }

        public void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (_frozen)
            {
                _logger.Write(level, exception, messageTemplate, propertyValues);
                return;
            }

            lock (_sync)
            {
                _logger.Write(level, exception, messageTemplate, propertyValues);
            }
        }

        public bool IsEnabled(LogEventLevel level)
        {
            if (_frozen)
            {
                return _logger.IsEnabled(level);
            }

            lock (_sync)
            {
                return _logger.IsEnabled(level);
            }
        }

        public void Verbose(string messageTemplate)
        {
            Write(LogEventLevel.Verbose, messageTemplate);
        }

        public void Verbose<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Verbose, messageTemplate, propertyValues);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate);
        }

        public void Verbose<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue);
        }

        public void Verbose<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Verbose<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Verbose(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Verbose, exception, messageTemplate, propertyValues);
        }

        public void Debug(string messageTemplate)
        {
            Write(LogEventLevel.Debug, messageTemplate);
        }

        public void Debug<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Debug, messageTemplate, propertyValues);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate);
        }

        public void Debug<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue);
        }

        public void Debug<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Debug<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Debug(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Debug, exception, messageTemplate, propertyValues);
        }
        
        public void Information(string messageTemplate)
        {
            Write(LogEventLevel.Information, messageTemplate);
        }

        public void Information<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValue);
        }

        public void Information<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Information, messageTemplate, propertyValues);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Information, exception, messageTemplate);
        }

        public void Information<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue);
        }

        public void Information<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Information<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Information(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Information, exception, messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate)
        {
            Write(LogEventLevel.Warning, messageTemplate);
        }

        public void Warning<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Warning, messageTemplate, propertyValues);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate);
        }

        public void Warning<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue);
        }

        public void Warning<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Warning<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }
        
        public void Warning(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Warning, exception, messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate)
        {
            Write(LogEventLevel.Error, messageTemplate);
        }

        public void Error<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValue);
        }

        public void Error<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Error, exception, messageTemplate);
        }

        public void Error<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValue);
        }

        public void Error<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Error<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);
        }
        
        public void Fatal(string messageTemplate)
        {
            Write(LogEventLevel.Fatal, messageTemplate);
        }

        public void Fatal<T>(string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValue);
        }

        public void Fatal<T0, T1>(string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Fatal, messageTemplate, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate);
        }

        public void Fatal<T>(Exception exception, string messageTemplate, T propertyValue)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue);
        }

        public void Fatal<T0, T1>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Fatal<T0, T1, T2>(Exception exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1,
            T2 propertyValue2)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValues);
        }

        public bool BindMessageTemplate(string messageTemplate, object[] propertyValues, out MessageTemplate parsedTemplate,
            out IEnumerable<LogEventProperty> boundProperties)
        {
            if (_frozen)
            {
                return _logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
            }

            lock (_sync)
            {
                return _logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
            }
        }

        public bool BindProperty(string propertyName, object value, bool destructureObjects, out LogEventProperty property)
        {
            if (_frozen)
            {
                return _logger.BindProperty(propertyName, value, destructureObjects, out property);
            }

            lock (_sync)
            {
                return _logger.BindProperty(propertyName, value, destructureObjects, out property);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        (ILogger, bool) UpdateForCaller(ILogger root, ILogger cached, IReloadableLogger caller, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (cached != null && root == _logger)
            {
                newRoot = default;
                newCached = default;
                frozen = _frozen;
                return (cached, frozen); // If we're frozen, then the caller hasn't observed this yet and should update.
            }
            
            newRoot = _logger;
            newCached = caller.ReloadLogger();
            frozen = false;
            return (newCached, true);
        }
        
        internal bool InvokeIsEnabled(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, out bool isEnabled, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                isEnabled = logger.IsEnabled(level);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                isEnabled = logger.IsEnabled(level);
                return update;
            }
        }
        
        internal bool InvokeBindMessageTemplate(ILogger root, ILogger cached, IReloadableLogger caller, string messageTemplate, 
            object[] propertyValues, out MessageTemplate parsedTemplate, out IEnumerable<LogEventProperty> boundProperties,
            out bool canBind, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindMessageTemplate(messageTemplate, propertyValues, out parsedTemplate, out boundProperties);
                return update;
            }
        }
        
        internal bool InvokeBindProperty(ILogger root, ILogger cached, IReloadableLogger caller, string propertyName, 
            object propertyValue, bool destructureObjects, out LogEventProperty property,
            out bool canBind, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindProperty(propertyName, propertyValue, destructureObjects, out property);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                canBind = logger.BindProperty(propertyName, propertyValue, destructureObjects, out property);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEvent logEvent, out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(logEvent);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(logEvent);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate);
                return update;
            }
        }

        internal bool InvokeWrite<T>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            T propertyValue,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1, T2>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, string messageTemplate,
            object[] propertyValues,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValues);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, messageTemplate, propertyValues);
                return update;
            }
        }
        
        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate);
                return update;
            }
        }

        internal bool InvokeWrite<T>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            T propertyValue,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1);
                return update;
            }
        }
        
        internal bool InvokeWrite<T0, T1, T2>(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            T0 propertyValue0, T1 propertyValue1, T2 propertyValue2,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
                return update;
            }
        }

        internal bool InvokeWrite(ILogger root, ILogger cached, IReloadableLogger caller, LogEventLevel level, Exception exception, string messageTemplate,
            object[] propertyValues,
            out ILogger newRoot, out ILogger newCached, out bool frozen)
        {
            if (_frozen)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValues);
                return update;
            }

            lock (_sync)
            {
                var (logger, update) = UpdateForCaller(root, cached, caller, out newRoot, out newCached, out frozen);
                logger.Write(level, exception, messageTemplate, propertyValues);
                return update;
            }
        }

        internal bool CreateChild(
            ILogger root, 
            IReloadableLogger parent, 
            ILogger cachedParent,
            Func<ILogger, ILogger> configureChild,
            out ILogger child,
            out ILogger newRoot,
            out ILogger newCached,
            out bool frozen)
        {
            if (_frozen)
            {
                var (logger, _) = UpdateForCaller(root, cachedParent, parent, out newRoot, out newCached, out frozen);
                child = configureChild(logger);
                return true; // Always an update, since the caller has not observed that the reloadable logger is frozen.
            }

            // No synchronization, here - a lot of loggers are created and thrown away again without ever being used,
            // so we just return a lazy wrapper.
            child = new CachingReloadableLogger(this, root, parent, configureChild);
            newRoot = default;
            newCached = default;
            frozen = default;
            return false;
        }
    }
}
