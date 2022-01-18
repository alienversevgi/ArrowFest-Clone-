public enum OperationType
{
    Addition,
    Subtraction,
    Multiplication,
    Division
}

public struct DoorValue 
{
    public int Value;
    public OperationType OperationType;

    public DoorValue(int value, OperationType operationType)
    {
        Value = value;
        OperationType = operationType;
    }

    public override string ToString()
    {
        return $"{GetOperationSymbol(OperationType)}{Value}";
    }

    public char GetOperationSymbol(OperationType type)
    {
        char symbol = '$';
        switch (type)
        {
            case OperationType.Addition:
                symbol = '+';
                break;
            case OperationType.Subtraction:
                symbol = '-';
                break;
            case OperationType.Multiplication:
                symbol = 'X';
                break;
            case OperationType.Division:
                symbol = '%';
                break;
            default:
                break;
        }

        return symbol;
    }
}
