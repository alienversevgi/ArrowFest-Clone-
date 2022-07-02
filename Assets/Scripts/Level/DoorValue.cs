using UnityEngine;

namespace Game.Level
{
    public enum OperationType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    [System.Serializable]
    public class DoorValue
    {
        public bool IsEnable;
        public int Value;
        public OperationType OperationType;
        public bool HasMoveable;

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
}