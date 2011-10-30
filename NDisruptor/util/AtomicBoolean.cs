using System;
using System.Threading;

namespace NDisruptor
{
    public struct AtomicBoolean : IEquatable<AtomicBoolean>
    {
        int val;

        public AtomicBoolean(bool value)
        {
            val = 0;
            Thread.VolatileWrite(ref val, value ? 1 : 0);
        }

        public bool Value
        {
            get
            {
                return Thread.VolatileRead(ref val) != 0;
            }
            set
            {
                Thread.VolatileWrite(ref val, value ? 1 : 0);
            }
        }

        public bool Set()
        {
            return Interlocked.Exchange(ref val, 1) == 0;
        }

        public bool Reset()
        {
            return Interlocked.Exchange(ref val, 0) != 0;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return (obj is AtomicBoolean) && Equals((AtomicBoolean)obj);
        }

        public bool Equals(AtomicBoolean other)
        {
            return this.Value == other.Value;
        }

        public static bool operator ==(AtomicBoolean left, AtomicBoolean right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(AtomicBoolean left, AtomicBoolean right)
        {
            return left.Value != right.Value;
        }

        public void set(bool b)
        {
            throw new NotImplementedException();
        }

        public bool compareAndSet(bool b, bool b1)
        {
            throw new NotImplementedException();
        }

        public bool get()
        {
            throw new NotImplementedException();
        }
    }
}