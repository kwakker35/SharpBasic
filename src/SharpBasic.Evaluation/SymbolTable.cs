namespace SharpBasic.Evaluation;

public class SymbolTable(SymbolTable? parent = null)
{
    private readonly Dictionary<string, Value> _store = new();

    public Value? Get(string name) =>
        _store.TryGetValue(name, out var val) ? val : parent?.Get(name);

    public void Set(string name, Value value) => _store[name] = value;
}