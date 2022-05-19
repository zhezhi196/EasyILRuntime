using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module
{
    [Serializable]

    public struct FloatField: IAntiCrackingField<float>
    {
        private FieldEncryption field1;
        private float field2;
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
        public float value
        {
            get
            {
                if (aesType != FieldAesType.offset)
                {
                    return field1.encryptionValue.ToFloat();
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

        public FloatField(float value, FieldAesType aesType = FieldAesType.Xor)
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

        public static FloatField operator +(FloatField left, FloatField right)
        {
            return new FloatField(left.value + right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static FloatField operator -(FloatField left, FloatField right)
        {
            return new FloatField(left.value - right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static FloatField operator *(FloatField left, FloatField right)
        {
            return new FloatField(left.value * right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static FloatField operator /(FloatField left, FloatField right)
        {
            return new FloatField(left.value / right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        public static bool operator ==(FloatField left, FloatField right)
        {
            return Mathf.Abs(left.value - right.value) < 0.0001f;
        }
        
        public static bool operator !=(FloatField left, FloatField right)
        {
            return Mathf.Abs(left.value - right.value) > 0.0001f;
        }
        
        public static bool operator <(FloatField left, FloatField right)
        {
            return left.value < right.value;
        }

        public static bool operator >(FloatField left, FloatField right)
        {
            return left.value > right.value;
        }
        
        public static bool operator <=(FloatField left, FloatField right)
        {
            return left.value <= right.value;
        }

        public static bool operator >=(FloatField left, FloatField right)
        {
            return left.value >= right.value;
        }
        
        public static FloatField operator %(FloatField left, FloatField right)
        {
            return new FloatField(left.value % right.value, FieldEncryption.Merge(left.aesType, right.aesType));
        }
        
        
        public static FloatField operator +(float left, FloatField right)
        {
            return new FloatField(left + right.value, right.aesType);
        }
        
        public static FloatField operator -(float left, FloatField right)
        {
            return new FloatField(left - right.value, right.aesType);
        }
        
        public static FloatField operator *(float left, FloatField right)
        {
            return new FloatField(left * right.value, right.aesType);
        }
        
        public static FloatField operator /(float left, FloatField right)
        {
            return new FloatField(left / right.value, right.aesType);
        }
        
        public static bool operator ==(float left, FloatField right)
        {
            return Mathf.Abs(left - right.value) < 0.0001f;
        }
        
        public static bool operator !=(float left, FloatField right)
        {
            return Mathf.Abs(left - right.value) > 0.0001f;
        }
        
        public static bool operator <(float left, FloatField right)
        {
            return left < right.value;
        }

        public static bool operator >(float left, FloatField right)
        {
            return left > right.value;
        }
        
        public static bool operator <=(float left, FloatField right)
        {
            return left <= right.value;
        }

        public static bool operator >=(float left, FloatField right)
        {
            return left >= right.value;
        }
        
        public static FloatField operator %(float left, FloatField right)
        {
            return new FloatField(left % right.value, right.aesType);
        }
        
        
        public static FloatField operator +(FloatField left, float right)
        {
            return new FloatField(left.value + right, left.aesType);
        }
        
        public static FloatField operator -(FloatField left, float right)
        {
            return new FloatField(left.value - right, left.aesType);
        }
        
        public static FloatField operator *(FloatField left, float right)
        {
            return new FloatField(left.value * right, left.aesType);
        }
        
        public static FloatField operator /(FloatField left, float right)
        {
            return new FloatField(left.value / right, left.aesType);
        }
        
        public static bool operator ==(FloatField left, float right)
        {
            return Mathf.Abs(left.value - right) < 0.0001f;
        }
        
        public static bool operator !=(FloatField left, float right)
        {
            return Mathf.Abs(left.value - right) > 0.0001f;
        }
        
        public static bool operator <(FloatField left, float right)
        {
            return left.value < right;
        }

        public static bool operator >(FloatField left, float right)
        {
            return left.value > right;
        }
        
        public static bool operator <=(FloatField left, float right)
        {
            return left.value <= right;
        }

        public static bool operator >=(FloatField left, float right)
        {
            return left.value >= right;
        }
        
        public static FloatField operator %(FloatField left, float right)
        {
            return new FloatField(left.value % right, left.aesType);
        }
        
        
        public static FloatField operator ++(FloatField left)
        {
            return new FloatField(left.value++, left.aesType);
        }
        
        public static FloatField operator --(FloatField left)
        {
            return new FloatField(left.value--, left.aesType);
        }

        public static implicit operator FloatField(IntField v)
        {
            return new FloatField(v.value, v.aesType);
        }
        
        public static implicit operator FloatField(LongField v)
        {
            return new FloatField(v.value, v.aesType);
        }
        
        public static implicit operator float(FloatField v)
        {
            return v.value;
        }
    }
}