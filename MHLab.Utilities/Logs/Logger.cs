/*
The MIT License (MIT)

Copyright (c) 2020 Emanuele Manzione

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace MHLab.Utilities.Logs
{
    public static partial class Logger
    {
        private static List<ILogger>  _loggers;
        private static LogContextPool _contextPool;
        private static object         _sync;

        public static void Initialize()
        {
            _loggers = new List<ILogger>();

            _contextPool = new LogContextPool(
                () => new LogContext() {Level = "DBG"},
                () => new LogContext() {Level = "INF"},
                () => new LogContext() {Level = "WRN"},
                () => new LogContext() {Level = "ERR"}
            );

            _sync = new object();
        }
        
        public static void InitializeForConsole()
        {
            Initialize();
            AddLogger(new ConsoleLogger());
        }

        public static void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Log(LogContext context, Action<LogContext> callback)
        {
            if (context  == null) return;
            if (callback == null) return;

            lock (_sync) callback.Invoke(context);
        }

        public static void Info(string                    message,
                                [CallerFilePath]   string callerPath   = "",
                                [CallerLineNumber] long   callerLine   = 0,
                                [CallerMemberName] string callerMember = "")
        {
            var context = _contextPool.GetInfoContext();
            context.Message      = message;
            context.CallerFile   = callerPath;
            context.CallerMethod = callerMember;
            context.CallerLine   = callerLine;

            foreach (var logger in _loggers)
            {
                logger.Info(context);
            }

            _contextPool.ReturnInfoContext(context);
        }

        public static void Warning(string                    message,
                                   [CallerFilePath]   string callerPath   = "",
                                   [CallerLineNumber] long   callerLine   = 0,
                                   [CallerMemberName] string callerMember = "")
        {
            var context = _contextPool.GetWarningContext();
            context.Message      = message;
            context.CallerFile   = callerPath;
            context.CallerMethod = callerMember;
            context.CallerLine   = callerLine;

            foreach (var logger in _loggers)
            {
                logger.Warning(context);
            }

            _contextPool.ReturnWarningContext(context);
        }

        public static void Warning(Exception                 exception,
                                   [CallerFilePath]   string callerPath   = "",
                                   [CallerLineNumber] long   callerLine   = 0,
                                   [CallerMemberName] string callerMember = "")
        {
            var context = _contextPool.GetWarningContext();
            context.Message      = $"{exception.Message}\n{exception.StackTrace}";
            context.CallerFile   = callerPath;
            context.CallerMethod = callerMember;
            context.CallerLine   = callerLine;

            foreach (var logger in _loggers)
            {
                logger.Warning(context);
            }

            _contextPool.ReturnWarningContext(context);
        }

        public static void Error(string                    message,
                                 [CallerFilePath]   string callerPath   = "",
                                 [CallerLineNumber] long   callerLine   = 0,
                                 [CallerMemberName] string callerMember = "")
        {
            var context = _contextPool.GetErrorContext();
            context.Message      = message;
            context.CallerFile   = callerPath;
            context.CallerMethod = callerMember;
            context.CallerLine   = callerLine;

            foreach (var logger in _loggers)
            {
                logger.Error(context);
            }

            _contextPool.ReturnErrorContext(context);
        }

        public static void Error(Exception                 exception,
                                 [CallerFilePath]   string callerPath   = "",
                                 [CallerLineNumber] long   callerLine   = 0,
                                 [CallerMemberName] string callerMember = "")
        {
            var context = _contextPool.GetErrorContext();
            context.Message      = $"{exception.Message}\n{exception.StackTrace}";
            context.CallerFile   = callerPath;
            context.CallerMethod = callerMember;
            context.CallerLine   = callerLine;

            foreach (var logger in _loggers)
            {
                logger.Error(context);
            }

            _contextPool.ReturnErrorContext(context);
        }
    }
}