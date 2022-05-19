using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module
{
    [Serializable]
    public struct LongField : IAntiCrackingField<long>
    {
        private FieldEncryption field1;
        private long field2;

        private static int _offset = 0;

        private static int offset
        {
            get
            {
                if (_offset == 0) _offset = Random.Range(1, 100);
                return _offset;
            }
        }

        [Sirenix.OdinInspector.ShowInInspector, Sirenix.OdinInspector.HideLabel]
        public long value
        {
            get
            {
                if (aesType != FieldAesType.offset)
                {
                    return field1.encryptionValue.ToLong();
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

        public override string ToString()
        {
            return value.ToString();
        }

        public LongField(long value, FieldAesType type = FieldAesType.Xor)
        {
            field1 = new FieldEncryption();
            field1.aesType = type;
            this.field2 = -offset;
            this.value = value;
        }
        
        public static LongField operator +(LongField left, LongField right)
        {
            return new LongField(left.value + right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static LongField operator -(LongField left, LongField right)
        {
            return new LongField(left.value - right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static LongField operator *(LongField left, LongField right)
        {
            return new LongField(left.value * right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static LongField operator /(LongField left, LongField right)
        {
            return new LongField(left.value / right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static bool operator ==(LongField left, LongField right)
        {
            return left.value == right.value;
        }
        
        public static bool operator !=(LongField left, LongField right)
        {
            return left.value != right.value;
        }
        
        public static bool operator <(LongField left, LongField right)
        {
            return left.value < right.value;
        }

        public static bool operator >(LongField left, LongField right)
        {
            return left.value > right.value;
        }
        
        public static bool operator <=(LongField left, LongField right)
        {
            return left.value <= right.value;
        }

        public static bool operator >=(LongField left, LongField right)
        {
            return left.value >= right.value;
        }
        
        public static LongField operator %(LongField left, LongField right)
        {
            return new LongField(left.value % right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static LongField operator &(LongField left, LongField right)
        {
            return new LongField(left.value & right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static LongField operator |(LongField left, LongField right)
        {
            return new LongField(left.value | right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static LongField operator ^(LongField left, LongField right)
        {
            return new LongField(left.value ^ right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }

        public static LongField operator <<(LongField left, int right)
        {
            return new LongField(left.value << right, left.aesType);
        }
        
        public static LongField operator >>(LongField left, int right)
        {
            return new LongField(left.value >> right, left.aesType);
        }


        public static LongField operator +(long left, LongField right)
        {
            return new LongField(left + right.value, right.aesType);
        }
        
        public static LongField operator -(long left, LongField right)
        {
            return new LongField(left - right.value, right.aesType);
        }
        
        public static LongField operator *(long left, LongField right)
        {
            return new LongField(left * right.value, right.aesType);
        }
        
        public static LongField operator /(long left, LongField right)
        {
            return new LongField(left / right.value, right.aesType);
        }
        
        public static bool operator ==(long left, LongField right)
        {
            return left == right.value;
        }
        
        public static bool operator !=(long left, LongField right)
        {
            return left != right.value;
        }
        
        public static bool operator <(long left, LongField right)
        {
            return left < right.value;
        }

        public static bool operator >(long left, LongField right)
        {
            return left > right.value;
        }
        
        public static bool operator <=(long left, LongField right)
        {
            return left <= right.value;
        }

        public static bool operator >=(long left, LongField right)
        {
            return left >= right.value;
        }
        
        public static LongField operator %(long left, LongField right)
        {
            return new LongField(left % right.value, right.aesType);
        }
        
        public static LongField operator &(long left, LongField right)
        {
            return new LongField(left & right.value, right.aesType);
        }
        
        public static LongField operator |(long left, LongField right)
        {
            return new LongField(left | right.value, right.aesType);
        }
        
        public static LongField operator ^(long left, LongField right)
        {
            return new LongField(left ^ right.value, right.aesType);
        }

        public static LongField operator +(LongField left, long right)
        {
            return new LongField(left.value + right, left.aesType);
        }

        public static LongField operator -(LongField left, long right)
        {
            return new LongField(left.value - right, left.aesType);
        }

        public static LongField operator *(LongField left, long right)
        {
            return new LongField(left.value * right, left.aesType);
        }

        public static LongField operator /(LongField left, long right)
        {
            return new LongField(left.value / right, left.aesType);
        }

        public static bool operator ==(LongField left, long right)
        {
            return left.value == right;
        }

        public static bool operator !=(LongField left, long right)
        {
            return left.value != right;
        }

        public static bool operator <(LongField left, long right)
        {
            return left.value < right;
        }

        public static bool operator >(LongField left, long right)
        {
            return left.value > right;
        }

        public static bool operator <=(LongField left, long right)
        {
            return left.value <= right;
        }

        public static bool operator >=(LongField left, long right)
        {
            return left.value >= right;
        }

        public static LongField operator %(LongField left, long right)
        {
            return new LongField(left.value % right, left.aesType);
        }

        public static LongField operator &(LongField left, long right)
        {
            return new LongField(left.value & right, left.aesType);
        }

        public static LongField operator |(LongField left, long right)
        {
            return new LongField(left.value | right, left.aesType);
        }

        public static LongField operator ^(LongField left, long right)
        {
            return new LongField(left.value ^ right, left.aesType);
        }

        public static LongField operator ++(LongField left)
        {
            return new LongField(left.value++, left.aesType);
        }
        
        public static LongField operator --(LongField left)
        {
            return new LongField(left.value--, left.aesType);
        }
        
        public static LongField operator ~(LongField left)
        {
            return new LongField(~left.value, left.aesType);
        }
        
        public static implicit operator LongField(IntField v)
        {
            return new LongField(v.value, v.aesType);
        }
        
        public static implicit operator long(LongField v)
        {
            return v.value;
        }
    }
}