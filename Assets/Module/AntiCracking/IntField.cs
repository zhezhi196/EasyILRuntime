using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module
{
    [Serializable]
    public struct IntField : IAntiCrackingField<int>
    {
        private FieldEncryption field1;
        private int field2;
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
        public int value
        {
            get
            {
                if (aesType != FieldAesType.offset)
                {
                    return field1.encryptionValue.ToInt();
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

        public IntField(int value, FieldAesType type = FieldAesType.Xor)
        {
            field1 = new FieldEncryption();
            field1.aesType = type;
            this.field2 = -offset;
            this.value = value;
        }

        public static IntField operator +(IntField left, IntField right)
        {
            return new IntField(left.value + right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static IntField operator -(IntField left, IntField right)
        {
            return new IntField(left.value - right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static IntField operator *(IntField left, IntField right)
        {
            return new IntField(left.value * right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static IntField operator /(IntField left, IntField right)
        {
            return new IntField(left.value / right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static bool operator ==(IntField left, IntField right)
        {
            return left.value == right.value;
        }
        
        public static bool operator !=(IntField left, IntField right)
        {
            return left.value != right.value;
        }
        
        public static bool operator <(IntField left, IntField right)
        {
            return left.value < right.value;
        }

        public static bool operator >(IntField left, IntField right)
        {
            return left.value > right.value;
        }
        
        public static bool operator <=(IntField left, IntField right)
        {
            return left.value <= right.value;
        }

        public static bool operator >=(IntField left, IntField right)
        {
            return left.value >= right.value;
        }
        
        public static IntField operator %(IntField left, IntField right)
        {
            return new IntField(left.value % right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static IntField operator &(IntField left, IntField right)
        {
            return new IntField(left.value & right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static IntField operator |(IntField left, IntField right)
        {
            return new IntField(left.value | right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static IntField operator ^(IntField left, IntField right)
        {
            return new IntField(left.value ^ right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }

        public static IntField operator <<(IntField left, int right)
        {
            return new IntField(left.value << right, left.aesType);
        }
        
        public static IntField operator >>(IntField left, int right)
        {
            return new IntField(left.value >> right, left.aesType);
        }


        public static IntField operator +(int left, IntField right)
        {
            return new IntField(left + right.value, right.aesType);
        }
        
        public static IntField operator -(int left, IntField right)
        {
            return new IntField(left - right.value, right.aesType);
        }
        
        public static IntField operator *(int left, IntField right)
        {
            return new IntField(left * right.value, right.aesType);
        }
        
        public static IntField operator /(int left, IntField right)
        {
            return new IntField(left / right.value, right.aesType);
        }
        
        public static bool operator ==(int left, IntField right)
        {
            return left == right.value;
        }
        
        public static bool operator !=(int left, IntField right)
        {
            return left != right.value;
        }
        
        public static bool operator <(int left, IntField right)
        {
            return left < right.value;
        }

        public static bool operator >(int left, IntField right)
        {
            return left > right.value;
        }
        
        public static bool operator <=(int left, IntField right)
        {
            return left <= right.value;
        }

        public static bool operator >=(int left, IntField right)
        {
            return left >= right.value;
        }
        
        public static IntField operator %(int left, IntField right)
        {
            return new IntField(left % right.value, right.aesType);
        }
        
        public static IntField operator &(int left, IntField right)
        {
            return new IntField(left & right.value, right.aesType);
        }
        
        public static IntField operator |(int left, IntField right)
        {
            return new IntField(left | right.value, right.aesType);
        }
        
        public static IntField operator ^(int left, IntField right)
        {
            return new IntField(left ^ right.value, right.aesType);
        }

        public static IntField operator +(IntField left, int right)
        {
            return new IntField(left.value + right, left.aesType);
        }

        public static IntField operator -(IntField left, int right)
        {
            return new IntField(left.value - right, left.aesType);
        }

        public static IntField operator *(IntField left, int right)
        {
            return new IntField(left.value * right, left.aesType);
        }

        public static IntField operator /(IntField left, int right)
        {
            return new IntField(left.value / right, left.aesType);
        }

        public static bool operator ==(IntField left, int right)
        {
            return left.value == right;
        }

        public static bool operator !=(IntField left, int right)
        {
            return left.value != right;
        }

        public static bool operator <(IntField left, int right)
        {
            return left.value < right;
        }

        public static bool operator >(IntField left, int right)
        {
            return left.value > right;
        }

        public static bool operator <=(IntField left, int right)
        {
            return left.value <= right;
        }

        public static bool operator >=(IntField left, int right)
        {
            return left.value >= right;
        }

        public static IntField operator %(IntField left, int right)
        {
            return new IntField(left.value % right, left.aesType);
        }

        public static IntField operator &(IntField left, int right)
        {
            return new IntField(left.value & right, left.aesType);
        }

        public static IntField operator |(IntField left, int right)
        {
            return new IntField(left.value | right, left.aesType);
        }

        public static IntField operator ^(IntField left, int right)
        {
            return new IntField(left.value ^ right, left.aesType);
        }

        public static IntField operator ++(IntField left)
        {
            return new IntField(left.value++, left.aesType);
        }
        
        public static IntField operator --(IntField left)
        {
            return new IntField(left.value--, left.aesType);
        }
        
        public static IntField operator ~(IntField left)
        {
            return new IntField(~left.value, left.aesType);
        }

        public static implicit operator int(IntField v)
        {
            return v.value;
        }
    }
}