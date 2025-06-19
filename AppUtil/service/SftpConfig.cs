

namespace AppUtil.Service
{
    public class SftpConfig
    {
        public string Host {  get; set; }
        public int Port {  get; set; }
        public string User {  get; set; }
        public string Password {  get; set; }
        public SftpConfig() {
            Host = "200.166.2.203";
            Port = 4422;
            User = "oper";
            Password = "foxconn168!!";
        }
    }
}
