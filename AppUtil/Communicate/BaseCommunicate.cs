using AppUtil.Communicate.Interface;

namespace AppUtil.Communicate
{
    public abstract class BaseCommunicate : BaseInOutStream, IConnectable
    {
        public abstract bool Connect();
        public abstract bool IsConnect();
        public abstract bool Disconnect();
    }
}
