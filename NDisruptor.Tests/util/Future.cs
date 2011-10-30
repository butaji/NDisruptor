using System;
using System.Collections.Generic;

namespace NDisruptor.Tests
{
    public class Future<T>
    {
        private readonly T _value;

        public Future(T value)
        {
            _value = value;
        }

        public T get()
        {
            throw new NotImplementedException();
        }
    }
}