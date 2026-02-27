namespace SharpBasic.Evaluation;

public class SymbolTable
{
    private Dictionary<string, Value> _symbolTable;

    public SymbolTable()
    {
        _symbolTable = new();
    }
    public Value? Get(string name) => _symbolTable.TryGetValue(name, out var val) ? val : null;

    public void Set(string name, Value value) => _symbolTable[name] = value;
}