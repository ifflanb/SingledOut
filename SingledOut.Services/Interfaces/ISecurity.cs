namespace SingledOut.Services.Interfaces
{
    public interface ISecurity
    {
        string CreateHash(string unHashed);

        bool MatchHash(string hashData, string hashUser);
    }
}
