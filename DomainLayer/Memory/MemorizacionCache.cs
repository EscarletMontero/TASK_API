namespace DomainLayer.Memory
{
    public static class MemorizacionCache<TInput, TResult>
    {
        private static readonly Dictionary<string, TResult> _cache = new();

        public static TResult? Obtener(string clave)
        {
            return _cache.TryGetValue(clave, out var valor) ? valor : default;
        }

        public static void Guardar(string clave, TResult valor)
        {
            if (!_cache.ContainsKey(clave))
                _cache[clave] = valor;
        }

        public static void Limpiar()
        {
            _cache.Clear();
        }

        public static string GenerarClave(params object[] entradas)
        {
            return string.Join("_", entradas.Select(e => e.ToString()));
        }
    }
}
