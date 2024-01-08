/*
The MIT License (MIT)

Copyright (c) 2022 Emanuele Manzione

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

namespace MHLab.Utilities
{
    public static class Result
    {
        public static Result<TSuccess, TError> Ok<TSuccess, TError>(TSuccess data) =>
            new (data);

        public static Result<TSuccess, TError> Err<TSuccess, TError>(TError error) =>
            new (error);
    }

    public readonly struct Result<TSuccess, TError>
    {
        public readonly TSuccess Ok;
        public readonly TError   Error;

        private readonly bool _success;

        public bool IsOk    => _success;
        public bool IsError => !IsOk;

        public Result(TSuccess data)
        {
            Ok    = data;
            Error = default;

            _success = true;
        }

        public Result(TError data)
        {
            Ok    = default;
            Error = data;

            _success = false;
        }
        
        private static void ThrowInvalidOperation()
        {
            throw new InvalidOperationException();
        }

        public static implicit operator Result<TSuccess, TError>(TSuccess data) => new (data);
        public static implicit operator Result<TSuccess, TError>(TError data)   => new (data);

        public static implicit operator TSuccess(Result<TSuccess, TError> data)
        {
#if DEBUG
            if (data._success == false)
            {
                ThrowInvalidOperation();
                return default;
            }
#endif
            
            return data.Ok;
        }

        public static implicit operator TError(Result<TSuccess, TError> data)
        {
#if DEBUG
            if (data._success)
            {
                ThrowInvalidOperation();
                return default;
            }
#endif
            return data.Error;
        }

        public static implicit operator bool(Result<TSuccess, TError> data)   => data._success;
    }
}