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
    public readonly struct Maybe<TPayload>
    {
        private static readonly bool IsValueType = typeof(TPayload).IsValueType;

        private readonly Optional<TPayload> _value;

        public bool HasValue => _value.HasValue;

        public Maybe(TPayload data)
        {
            if (IsNull(data))
            {
                _value = Optional<TPayload>.None();
                return;
            }

            _value = data;
        }

        private static bool IsNull(TPayload data)
        {
            if (IsValueType) return false;

            return data == null;
        }

        public static Maybe<TPayload> From(TPayload data) => new(data);

        public static implicit operator Maybe<TPayload>(TPayload data) => new(data);
        public static implicit operator TPayload(Maybe<TPayload> optional) => optional._value;
        public static implicit operator bool(Maybe<TPayload> optional) => optional.HasValue;

        public static Maybe<TPayload> Some(TPayload payload) => new(payload);
        public static Maybe<TPayload> None() => new();

        public override string ToString()
        {
            return _value.ToString();
        }

        public TPayload Unwrap()
        {
            return _value.Unwrap();
        }

        public TPayload UnwrapOrDefault(TPayload defaultValue)
        {
            return _value.UnwrapOrDefault(defaultValue);
        }

        public bool TryUnwrap(out TPayload value)
        {
            return _value.TryUnwrap(out value);
        }

        internal string ToDebuggerString()
        {
            return _value.ToDebuggerString();
        }
    }
    
    [DebuggerDisplay("{ToDebuggerString(),nq}")]
    public readonly struct MaybeError<TError>
    {
        private static readonly bool IsValueType = typeof(TError).IsValueType;

        private readonly Optional<TError> _value;

        public bool HasError => _value.HasValue;

        public MaybeError(TError data)
        {
            if (IsNull(data))
            {
                _value = Optional<TError>.None();
                return;
            }

            _value = data;
        }

        public MaybeError()
        {
            _value = Optional<TError>.None();
        }

        private static bool IsNull(TError data)
        {
            if (IsValueType) return false;

            return data == null;
        }
    
        public static MaybeError<TError> From(TError data) => new(data);

        public static implicit operator MaybeError<TError>(TError data) => new(data);
        public static implicit operator TError(MaybeError<TError> optional) => optional._value;
        public static implicit operator bool(MaybeError<TError> optional) => optional.HasError;

        public static MaybeError<TError> Some(TError payload) => new(payload);
        public static MaybeError<TError> None() => new();

        public override string ToString()
        {
            return _value.ToString();
        }
    
        public TError Unwrap()
        {
            return _value.Unwrap();
        }

        public TError UnwrapOrDefault(TError defaultValue)
        {
            return _value.UnwrapOrDefault(defaultValue);
        }

        public bool TryUnwrap(out TError value)
        {
            return _value.TryUnwrap(out value);
        }

        internal string ToDebuggerString()
        {
            return _value.ToDebuggerString();
        }
    }
}