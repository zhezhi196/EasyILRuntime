using SqlCipher4Unity3D;
using UnityEngine;
//using UnityEngine.iOS;

namespace Module
{
    public static class PlayerInfo
    {
        private static string deviceUniqueIdentifier
        {
            get { return LocalFileMgr.GetString("deviceUniqueIdentifier"); }
            set { LocalFileMgr.SetString("deviceUniqueIdentifier", value); }
        }

        private static string _tempid;
        public static string pid
        {
            get
            {
#if LOG_ENABLE
                if (_tempid == null)
                {
                    _tempid = EncryptionHelper.MD5Encrypt("我是一只可爱的小青蛙");
                }
                return _tempid;
#else
                if (_tempid.IsNullOrEmpty())
                {
                    if (deviceUniqueIdentifier.IsNullOrEmpty())
                    {
                        deviceUniqueIdentifier = EncryptionHelper.MD5Encrypt(SystemInfo.deviceUniqueIdentifier + Application.identifier+"112233");
                    }
                    
                    _tempid = EncryptionHelper.MD5Encrypt(deviceUniqueIdentifier + ((SqlService<PlayerInfoSaveData>)SqlData.tableDic[typeof(PlayerInfoSaveData).FullName]).tableList[0].creatDataTime);
                }

                return _tempid;
#endif

            }
        }

        public static AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            process.IsDone = false;
            SqlService<PlayerInfoSaveData> service = SqlData.tableDic[typeof(PlayerInfoSaveData).FullName] as SqlService<PlayerInfoSaveData>;
            if (service != null)
            {
                var tableList = service.tableList;
                if (tableList.Count == 0)
                {
                    service.Insert(new PlayerInfoSaveData() {creatDataTime = TimeHelper.now.ToString()});
                } 
            }
            process.SetComplete();
            return process;
        }
    }
}