using System;

namespace Module
{
    public struct AnalyticsInfo
    {
        public string eventID;
        public int type;
        public int MissionID;
        public string targetID;
        public int maxSendCount;

        public bool MatchSuccess(int type, int missionId, string targetId,bool log)
        {
            if (maxSendCount != -1 && maxSendCount <= 0) return false;
            if (targetId == null) targetId = string.Empty;
            bool typeBool = this.type == type;
            bool missionBool = this.MissionID == missionId;
            bool targetIDBool = this.targetID == targetId;
            if (log)
            {
                if (!typeBool)
                {
                    GameDebug.Log($"{eventID}_typeBool: this.type={this.type} type={type}");
                }

                if (!missionBool)
                {
                    GameDebug.Log($"{eventID}_missionBool: this.MissionID={this.MissionID} type={missionId}");
                }

                if (!targetIDBool)
                {
                    GameDebug.Log($"{eventID}_targetIDBool: this.type={this.targetID} type={targetId}");
                }
            }
            return typeBool && missionBool && targetIDBool;
        }
    }
}