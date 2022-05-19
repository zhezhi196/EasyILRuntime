namespace Module
{
    public enum FieldAesType
    {
        offset,
        Xor,
        Aes,
    }
    public struct FieldEncryption
    {
        public static FieldAesType Merge(FieldAesType left, FieldAesType right)
        {
            if (left == FieldAesType.Aes || right == FieldAesType.Aes)
            {
                return FieldAesType.Aes;
            }

            if (left == FieldAesType.Xor || right == FieldAesType.Xor)
            {
                return FieldAesType.Xor;
            }

            return FieldAesType.offset;
        }
        public FieldAesType aesType;

        private string _encryptionValue;

        private string _key;

        private string key
        {
            get { return _key; }
        }
        
        public string encryptionValue
        {
            get
            {
                if (aesType == FieldAesType.Xor)
                {
                    return EncryptionHelper.Xor(_encryptionValue, key);
                }
                else if (aesType == FieldAesType.Aes)
                {
                    return EncryptionHelper.AesDecrypt(_encryptionValue, _key);
                }

                return null;
            }
        }

        public void SetKey(string key, string value)
        {
            this._key = key;
            if (aesType == FieldAesType.Xor)
            {
                _encryptionValue = EncryptionHelper.Xor(value, this.key);
            }
            else if (aesType == FieldAesType.Aes)
            {
                _encryptionValue = EncryptionHelper.AesEncrypt(value, _key);
            }
        }
    }
}