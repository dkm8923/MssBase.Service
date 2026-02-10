namespace Contract.Security
{
    public interface ISecurityConnectionStrings
    {
        public string SecurityConnectionStringReadWrite { get; set; }
        public string SecurityConnectionStringReadOnly { get; set; }
    }
}
