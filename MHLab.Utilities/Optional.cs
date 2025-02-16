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
using System.Diagnostics;

namespace MHLab.Utilities
{
    [DebuggerDisplay("{ToDebuggerString(),nq}")]
    public readonly struct Optional<TPayload>
    {
        private static readonly bool IsValueType = typeof(TPayload).IsValueType;

        private readonly TPayload _value;
        private readonly bool _hasValue;

        public bool HasValue => _hasValue;

        public Optional(TPayload data)
        {
            if (IsNull(data))
            {
                _hasValue = false;
                _value = default!;
                return;
            }

            _value = data;
            _hasValue = true;
        }

        private static bool IsNull(TPayload data)
        {
            if (IsValueType) return false;

            return data == null;
        }
        
        public static Optional<TPayload> From(TPayload data) => new(data);

        public static implicit operator Optional<TPayload>(TPayload data) => new(data);
        public static implicit operator TPayload(Optional<TPayload> optional) => optional._value;
        public static implicit operator bool(Optional<TPayload> optional) => optional.HasValue;

        public static Optional<TPayload> Some(TPayload payload) => new(payload);
        public static Optional<TPayload> None() => new();

        public override string ToString()
        {
            const string noneString = "None";

    #pragma warning disable CS8603 // Possible null reference return.
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
            return (HasValue) ? _value.ToString() : noneString;
    #pragma warning restore CS8602 // Dereference of a possibly null reference.
    #pragma warning restore CS8603 // Possible null reference return.
        }

        public TPayload Unwrap()
        {
            if (HasValue)
                return _value;

            const string error = "Cannot Unwrap: there is no value";
            throw new InvalidOperationException(error);
        }
        
        public TPayload UnwrapOrDefault(TPayload defaultValue)
        {
            return HasValue ? _value : defaultValue;
        }

        public bool TryUnwrap(out TPayload value)
        {
            value = _value;
            return HasValue;
        }

        internal string ToDebuggerString()
        {
            var output = $"HasValue = {HasValue}";

            if (HasValue)
            {
                output += $", Value = {ToString()}";
            }

            return output;
        }
    }
}