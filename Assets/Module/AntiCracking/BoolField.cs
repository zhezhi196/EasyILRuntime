using System;
using UnityEngine;

namespace Module
{
    [Serializable]
    public struct BoolField : IAntiCrackingField<bool>
    {
        private FieldEncryption field1;
        [Sirenix.OdinInspector.ShowInInspector,Sirenix.OdinInspector.HideLabel]
        public bool value
        {
            get
            {
                return field1.encryptionValue.ToBool();
            }
            set
            {
                string key = PlayerInfo.pid;
                field1.SetKey(key, value.ToString());
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

        public BoolField(bool value, FieldAesType aesType = FieldAesType.Xor)
        {
            field1 = new FieldEncryption();
            field1.aesType = aesType;
            this.value = value;
        }
        
        public static bool operator ==(BoolField left, BoolField right)
        {
            return left.value == right.value;
        }

        public static bool operator !=(BoolField left, BoolField right)
        {
            return left.value != right.value;
        }
        
        public static BoolField operator &(BoolField left, BoolField right)
        {
            return new BoolField(left.value & right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static BoolField operator |(BoolField left, BoolField right)
        {
            return new BoolField(left.value | right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static BoolField operator ^(BoolField left, BoolField right)
        {
            return new BoolField(left.value ^ right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static bool operator ==(bool left, BoolField right)
        {
            return left == right.value;
        }
        
        public static bool operator !=(bool left, BoolField right)
        {
            return left != right.value;
        }
        
        public static BoolField operator &(bool left, BoolField right)
        {
            return new BoolField(left & right.value, right.aesType);
        }
        
        public static BoolField operator |(bool left, BoolField right)
        {
            return new BoolField(left | right.value,right.aesType);
        }
        
        public static BoolField operator ^(bool left, BoolField right)
        {
            return new BoolField(left ^ right.value,right.aesType);
        }
        
        public static bool operator ==(BoolField left, bool right)
        {
            return left.value == right;
        }
        
        public static bool operator !=(BoolField left, bool right)
        {
            return left.value != right;
        }
        
        public static BoolField operator &(BoolField left, bool right)
        {
            return new BoolField(left.value & right,left.aesType);
        }
        
        public static BoolField operator |(BoolField left, bool right)
        {
            return new BoolField(left.value | right,left.aesType);
        }
        
        public static BoolField operator ^(BoolField left, bool right)
        {
            return new BoolField(left.value ^ right,left.aesType);
        }
        
        public static BoolField operator !(BoolField left)
        {
            return new BoolField(!left.value,left.aesType);
        }
        //
        public static bool operator true(BoolField left)
        {
            return left.value;
        }

        public static bool operator false(BoolField left)
        {
            return left.value;
        }
        
        public static implicit operator bool(BoolField v)
        {
            return v.value;
        }

        // public static implicit operator BoolField(int v)
        // {
        //     if (v == 0) return new BoolField(false);
        //     return new BoolField(true);
        // }
    }
}