namespace SharpBasic.Evaluation;

public class SymbolTable(SymbolTable? parent = null)
{
    private readonly Dictionary<string, Value> _store = new();
    private readonly HashSet<string> _consts = new();

    public Value? Get(string name) =>
        _store.TryGetValue(name, out var val) ? val : parent?.Get(name);

    public Value? GetLocal(string name) =>
        _store.TryGetValue(name, out var val) ? val : null;

    public bool IsConst(string name) =>
        _consts.Contains(name) || (parent?.IsConst(name) ?? false);

    public void SetConst(string name, Value value)
    {
        _consts.Add(name);
        _store[name] = value;
    }

    public bool Set(string name, Value value)
    {
        if (IsConst(name)) return false;
        _store[name] = value;
        return true;
    }

    public bool IsGlobal => parent is null;
    public SymbolTable Root => parent?.Root ?? this;
}