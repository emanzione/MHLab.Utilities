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
    [DebuggerDisplay("HasValue = {HasValue}")]
    public readonly struct Optional<TPayload>
    {
        public readonly TPayload Value;

        private readonly bool _hasValue;

        public bool HasValue => _hasValue;

        public Optional(TPayload data)
        {
            Value     = data;
            _hasValue = true;
        }

        public static implicit operator Optional<TPayload>(TPayload data) => new Optional<TPayload>(data);

        public static implicit operator TPayload(Optional<TPayload> optional) => optional.Value;
        
        public static implicit operator bool(Optional<TPayload> optional) => optional.HasValue;

        public static Optional<TPayload> Some(TPayload payload) => new Optional<TPayload>(payload);
        public static Optional<TPayload> None()                 => new Optional<TPayload>();

        public override string ToString()
        {
            const string noneString = "None";
            
            return (HasValue) ? Value.ToString() : noneString;
        }
    }
}