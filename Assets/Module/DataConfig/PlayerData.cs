namespace Module
{
    public class PlayerData : SqlData
    {
        public string name { get; set; }
        public int gold { get; set; }
        public int gen { get; set; }

        public static PlayerData[] DefineDefault()
        {
            PlayerData[] playerData = new PlayerData[1];
            playerData[0] = new PlayerData();
            playerData[0].name = "nickName";
            playerData[0].gold = 10;
            playerData[0].gen = 10;
            return playerData;
        }
    }
}