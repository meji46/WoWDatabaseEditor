using System.Buffers;

namespace WDE.MpqReader
{
    public class PooledArray<T> : System.IDisposable
    {
        private readonly T[] array;
        private readonly int length;

        public PooledArray(int length)
        {
            this.length = length;
            array = ArrayPool<T>.Shared.Rent(length);
        }

        public int Length => length;
    
        public T this[int index]
        {
            get => array[index];
            set => array[index] = value;
        }

        public ReadOnlySpan<T> AsSpan() => array.AsSpan(0, length);
    
        public T[] AsArray() => array;

        public void Dispose()
        {
            ArrayPool<T>.Shared.Return(array);
        }
    }

    public struct GroupedArrayPooler<T> : System.IDisposable
    {
        private readonly T[][] arrays;
        private readonly int length;
        private int i = 0;

        public GroupedArrayPooler(int capacity)
        {
            length = capacity;
            i = 0;
            arrays = ArrayPool<T[]>.Shared.Rent(capacity);
        }

        public T[] Get(int size)
        {
            var array= ArrayPool<T>.Shared.Rent(size);
            if (i >= length)
                throw new Exception("This array pooler can create no more than " + length + " arrays");
            arrays[i++] = array;
            return array;
        }

        public void Dispose()
        {
            for (int j = 0; j < i; ++j)
            {
                ArrayPool<T>.Shared.Return(arrays[j]);
            }
            ArrayPool<T[]>.Shared.Return(arrays);
        }
    }
}