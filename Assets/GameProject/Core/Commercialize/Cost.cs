using Module;

public struct Cost
{
    public static Cost[] CreatCost(string type, string cost)
    {
        string[] typestr = type.Split(ConstKey.Spite0);
        string[] coststr = cost.Split(ConstKey.Spite0);
        Cost[] result = new Cost[typestr.Length];
        for (int i = 0; i < typestr.Length; i++)
        {
            result[i] = new Cost((MoneyType) (typestr[i].ToInt()), coststr[i].ToInt());
        }

        return result;
    }
    
    public IntField cost;
    public MoneyType type;

    public Cost(MoneyType type, int value)
    {
        this.cost = new IntField(value);
        this.type = type;
    }
    
    public static Cost operator -(Cost left, Cost right)
    {
        if (left.type != right.type)
        {
            GameDebug.LogError("Left type!= right type");
            return default;
        }
        return new Cost(left.type, left.cost - right.cost);
    }
    public static Cost operator +(Cost left, Cost right)
    {
        if (left.type != right.type)
        {
            GameDebug.LogError("Left type!= right type");
            return default;
        }
        return new Cost(left.type, left.cost + right.cost);
    }

    public static Cost operator +(float left, Cost right)
    {
        Cost result = new Cost(right.type, (int)(left + right.cost));
        return result;
    }

    public static Cost operator -(float left, Cost right)
    {
        Cost result = new Cost(right.type, (int) (left - right.cost));
        return result;
    }

    public static Cost operator +(Cost left, float right)
    {
        Cost result = new Cost(left.type, (int) (left.cost + right));
        return result;
    }

    public static Cost operator -(Cost left, float right)
    {
        Cost result = new Cost(left.type, (int) (left.cost - right));
        return result;
    }

    public static Cost operator *(float left, Cost right)
    {
        Cost result = new Cost(right.type, (int) (right.cost * left));
        return result;
    }

    public static Cost operator *(Cost left, float right)
    {
        return right * left;
    }
}