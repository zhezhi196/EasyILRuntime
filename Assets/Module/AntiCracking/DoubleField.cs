using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module
{
    [Serializable]
    public struct DoubleField : IAntiCrackingField<double>
    {
        private FieldEncryption field1;
        private double field2;
        private static int _offset = 0;

        private static int offset
        {
            get
            {
                if (_offset == 0) _offset = Random.Range(1, 100);
                return _offset;
            }
        }
        [Sirenix.OdinInspector.ShowInInspector,Sirenix.OdinInspector.HideLabel]
        public double value
        {
            get
            {
                if (aesType != FieldAesType.offset)
                {
                    return field1.encryptionValue.ToDouble();
                }
                else
                {
                    return field2 + offset;
                }
            }
            set
            {
                if (aesType != FieldAesType.offset)
                {
                    string key = PlayerInfo.pid;
                    field1.SetKey(key, value.ToString());
                }
                else
                {
                    field2 = value - offset;
                }
            }
        }
        public FieldAesType aesType
        {
            get { return field1.aesType; }
        }

        public DoubleField(double value, FieldAesType aesType = FieldAesType.Xor)
        {
            field1 = new FieldEncryption();
            field1.aesType = aesType;
            this.field2 = -offset;
            this.value = value;
        }
        
        public override string ToString()
        {
            return value.ToString();
        }

        public static DoubleField operator +(DoubleField left, DoubleField right)
        {
            return new DoubleField(left.value + right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static DoubleField operator -(DoubleField left, DoubleField right)
        {
            return new DoubleField(left.value - right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static DoubleField operator *(DoubleField left, DoubleField right)
        {
            return new DoubleField(left.value * right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static DoubleField operator /(DoubleField left, DoubleField right)
        {
            return new DoubleField(left.value / right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static bool operator ==(DoubleField left, DoubleField right)
        {
            return Math.Abs(left.value - right.value) < 0.00001f;
        }
        
        public static bool operator !=(DoubleField left, DoubleField right)
        {
            return Math.Abs(left.value - right.value) > 0.00001f;
        }
        
        public static bool operator <(DoubleField left, DoubleField right)
        {
            return left.value < right.value;
        }

        public static bool operator >(DoubleField left, DoubleField right)
        {
            return left.value > right.value;
        }
        
        public static bool operator <=(DoubleField left, DoubleField right)
        {
            return left.value <= right.value;
        }

        public static bool operator >=(DoubleField left, DoubleField right)
        {
            return left.value >= right.value;
        }
        
        public static DoubleField operator %(DoubleField left, DoubleField right)
        {
            return new DoubleField(left.value % right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static DoubleField operator +(double left, DoubleField right)
        {
            return new DoubleField(left + right.value, right.aesType);
        }
        
        public static DoubleField operator -(double left, DoubleField right)
        {
            return new DoubleField(left - right.value, right.aesType);
        }
        
        public static DoubleField operator *(double left, DoubleField right)
        {
            return new DoubleField(left * right.value,right.aesType);
        }
        
        public static DoubleField operator /(double left, DoubleField right)
        {
            return new DoubleField(left / right.value,right.aesType);
        }
        
        public static bool operator ==(double left, DoubleField right)
        {
            return Math.Abs(left - right.value) < 0.0001f;
        }
        
        public static bool operator !=(double left, DoubleField right)
        {
            return Math.Abs(left - right.value) > 0.0001f;
        }
        
        public static bool operator <(double left, DoubleField right)
        {
            return left < right.value;
        }

        public static bool operator >(double left, DoubleField right)
        {
            return left > right.value;
        }
        
        public static bool operator <=(double left, DoubleField right)
        {
            return left <= right.value;
        }

        public static bool operator >=(double left, DoubleField right)
        {
            return left >= right.value;
        }
        
        public static DoubleField operator %(double left, DoubleField right)
        {
            return new DoubleField(left % right.value,right.aesType);
        }
        
        
        public static DoubleField operator +(DoubleField left, double right)
        {
            return new DoubleField(left.value + right,left.aesType);
        }
        
        public static DoubleField operator -(DoubleField left, double right)
        {
            return new DoubleField(left.value - right,left.aesType);
        }
        
        public static DoubleField operator *(DoubleField left, double right)
        {
            return new DoubleField(left.value * right,left.aesType);
        }
        
        public static DoubleField operator /(DoubleField left, double right)
        {
            return new DoubleField(left.value / right,left.aesType);
        }
        
        public static bool operator ==(DoubleField left, double right)
        {
            return Math.Abs(left.value - right) < 0.0001f;
        }
        
        public static bool operator !=(DoubleField left, double right)
        {
            return Math.Abs(left.value - right) > 0.0001f;
        }
        
        public static bool operator <(DoubleField left, double right)
        {
            return left.value < right;
        }

        public static bool operator >(DoubleField left, double right)
        {
            return left.value > right;
        }
        
        public static bool operator <=(DoubleField left, double right)
        {
            return left.value <= right;
        }

        public static bool operator >=(DoubleField left, double right)
        {
            return left.value >= right;
        }
        
        public static DoubleField operator %(DoubleField left, double right)
        {
            return new DoubleField(left.value % right,left.aesType);
        }
        
        
        public static DoubleField operator ++(DoubleField left)
        {
            return new DoubleField(left.value++,left.aesType);
        }
        
        public static DoubleField operator --(DoubleField left)
        {
            return new DoubleField(left.value--,left.aesType);
        }
                
        public static implicit operator DoubleField(FloatField v)
        {
            return new DoubleField(v.value,v.aesType);
        }
        
        public static implicit operator DoubleField(IntField v)
        {
            return new FloatField(v.value,v.aesType);
        }

        public static implicit operator DoubleField(LongField v)
        {
            return new FloatField(v.value,v.aesType);
        }
        
        public static implicit operator double(DoubleField v)
        {
            return v.value;
        }
    }
}