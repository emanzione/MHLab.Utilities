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

using System.Diagnostics;

namespace MHLab.Utilities
{
    [DebuggerDisplay("{ToDebuggerString(),nq}")]
    public readonly struct Result<TSuccess, TError>
    {
        private readonly Either<TSuccess, TError> _payload;

        public bool IsOk => _payload.IsLeft;
        public bool IsError => !IsOk;

        public Result(TSuccess data)
        {
            _payload = data;
        }

        public Result(TError data)
        {
            _payload = data;
        }
        
        public static Result<TSuccess, TError> From(TSuccess data) => new(data);
        public static Result<TSuccess, TError> From(TError error) => new(error);

        public static implicit operator Result<TSuccess, TError>(TSuccess data) => new (data);
        public static implicit operator Result<TSuccess, TError>(TError data) => new (data);

        public static implicit operator TSuccess(Result<TSuccess, TError> data) => data.Unwrap();
        public static implicit operator TError(Result<TSuccess, TError> data) => data.UnwrapError();

        public static implicit operator bool(Result<TSuccess, TError> data) => data.IsOk;

        public override string ToString()
        {
    #pragma warning disable CS8603 // Possible null reference return.
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
            return (IsOk) 
                ? Unwrap().ToString()
                : UnwrapError().ToString();
    #pragma warning restore CS8602 // Dereference of a possibly null reference.
    #pragma warning restore CS8603 // Possible null reference return.
        }

        public TSuccess Unwrap()
        {
            return _payload.UnwrapLeft();
        }

        public bool TryUnwrap(out TSuccess value)
        {
            if (IsOk)
            {
                value = Unwrap();
            }
            else
            {
    #pragma warning disable CS8601 // Possible null reference assignment.
                value = default;
    #pragma warning restore CS8601 // Possible null reference assignment.
            }
            
            return IsOk;
        }

        public TSuccess UnwrapOrDefault(TSuccess defaultValue)
        {
            return _payload.UnwrapLeftOrDefault(defaultValue);
        }

        public TError UnwrapError()
        {
            return _payload.UnwrapRight();
        }

        public bool TryUnwrapError(out TError value)
        {
            if (IsError)
            {
                value = UnwrapError();
            }
            else
            {
    #pragma warning disable CS8601 // Possible null reference assignment.
                value = default;
    #pragma warning restore CS8601 // Possible null reference assignment.
            }
            
            return IsError;
        }

        public TError UnwrapErrorOrDefault(TError defaultValue)
        {
            return _payload.UnwrapRightOrDefault(defaultValue);
        }

        private string ToDebuggerString()
        {
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
            return (IsOk)
                ? $"Ok: {Unwrap().ToString()}"
                : $"Error: {UnwrapError().ToString()}";
    #pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}