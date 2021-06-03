using Module;

namespace Module
{
    public interface IapDataBase : ISqlData
    {
        int ID { get; set; }
        int switchStation { get; set; }
        //0 货币 1 广告 2 订阅 3 免费
        int consume { get; set; }
        float rmb { get; set; }
        float usd { get; set; }
        int adsType { get; set; }
        string appStore { get; set; }
        string appStoreCN { get; set; }
        string googlePlay { get; set; }
        int getDes { get; set; }
    }
}
