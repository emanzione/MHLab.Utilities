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
    public readonly struct Either<TLeftType, TRightType>
    {
        private const string InvalidString = "<Invalid>";
        public static readonly Either<TLeftType, TRightType> Invalid = new();

        private readonly Optional<TLeftType> _left;
        private readonly Optional<TRightType> _right;

        public Either(TLeftType data)
        {
            _left = data;
            _right = Optional<TRightType>.None();
        }

        public Either(TRightType data)
        {
            _left = Optional<TLeftType>.None();
            _right = data;
        }

        public bool IsLeft => _left.HasValue;
        public bool IsRight => _right.HasValue;
        public bool IsValid => IsLeft || IsRight;

        public static Either<TLeftType, TRightType> From(TLeftType data) => new(data);
        public static Either<TLeftType, TRightType> From(TRightType data) => new(data);

        public static implicit operator Either<TLeftType, TRightType>(TLeftType data) => From(data);
        public static implicit operator Either<TLeftType, TRightType>(TRightType data) => From(data);

        public static implicit operator TLeftType(Either<TLeftType, TRightType> data) => data._left;
        public static implicit operator TRightType(Either<TLeftType, TRightType> data) => data._right;

        public static implicit operator bool(Either<TLeftType, TRightType> data) => data.IsValid;

        public TLeftType UnwrapLeft()
        {
            if (_left.HasValue)
                return _left.Unwrap();

            const string error = "Cannot Unwrap: Left has no value";
            throw new InvalidOperationException(error);
        }

        public bool TryUnwrapLeft(out TLeftType value)
        {
            if (IsLeft)
            {
                value = _left.Unwrap();
            }
            else
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                value = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            }

            return IsLeft;
        }

        public TLeftType UnwrapLeftOrDefault(TLeftType defaultValue)
        {
            return _left.HasValue ? _left.Unwrap() : defaultValue;
        }

        public TRightType UnwrapRight()
        {
            if (_right.HasValue)
                return _right.Unwrap();

            const string error = "Cannot Unwrap: Right has no value";
            throw new InvalidOperationException(error);
        }

        public bool TryUnwrapRight(out TRightType value)
        {
            if (IsRight)
            {
                value = _right.Unwrap();
            }
            else
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                value = default;
#pragma warning restore CS8601 // Possible null reference assignment.
            }

            return IsRight;
        }

        public TRightType UnwrapRightOrDefault(TRightType defaultValue)
        {
            return _right.HasValue ? _right.Unwrap() : defaultValue;
        }

        public override string ToString()
        {
            if (!IsValid)
            {
                return InvalidString;
            }

            return (_left.HasValue) ? _left.ToString() : _right.ToString();
        }

        private string ToDebuggerString()
        {
            if (!IsValid)
            {
                return InvalidString;
            }

            return (_left.HasValue)
                ? $"Left: {_left.ToString()}"
                : $"Right: {_right.ToString()}";
        }
    }
}