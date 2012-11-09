
namespace BaconFTP.Data.Logger
{
    public interface ILogger
    {
        void Write(string message, params object[] args);
    }
}
