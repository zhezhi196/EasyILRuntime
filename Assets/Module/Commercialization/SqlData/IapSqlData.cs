using Module;

namespace Module
{
    public interface IapSqlData : ISqlData
    {
        int switchStation { get; set; }
    }

    public interface ICurrencyData : IapSqlData
    {
        string sku { get; set; }
        string showPrice { get; set; }
    }
    
    public interface IAdsData : IapSqlData
    {
        int adsType { get; set; }
    }

    public interface ISubscribeData : IapSqlData
    {
    }
}
