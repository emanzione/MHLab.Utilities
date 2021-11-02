namespace MHLab.Utilities.Types
{
    public static class TypeMapper<TType>
    {
        public static readonly TypeId Id = TypeIdGenerator.Get();
    }

    internal static class TypeIdGenerator
    {
        private static int _counter;

        public static TypeId Get()
        {
            var counter = _counter;
            _counter++;
            return new TypeId(counter);
        }
    }
}