namespace Contract.Common
{
    public interface ICommonConnectionStrings
    {
        public string CommonConnectionStringReadWrite { get; set; }
        public string CommonConnectionStringReadOnly { get; set; }
    }
}
