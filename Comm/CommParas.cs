namespace YEasyInstaller.Comm
{
    public class CommParas
    {
        /// <summary>
        /// 数据库连接字符串Key
        /// </summary>
        public const string ConnectionStringKey = "ConnectionString";
        /// <summary>
        /// 数据库信息Key
        /// </summary>
        public const string DBKey = "Database";
        /// <summary>
        /// web管理端Key
        /// </summary>
        public const string WebKey = "Web";
        /// <summary>
        /// Api服务 Key
        /// </summary>
        public const string ApiKey = "Api";
        /// <summary>
        /// Socket服务 Key
        /// </summary>
        public const string SocketKey = "Socket";

        /// <summary>
        /// 门禁Web 端口号
        /// </summary>
        public const int Web_MJ_Port = 8021;
        /// <summary>
        /// 门禁Api 端口号
        /// </summary>
        public const int Api_MJ_Port = 8022;
        /// <summary>
        /// 消费Web 端口号
        /// </summary>
        public const int Web_XF_Port = 5501;
        /// <summary>
        /// 消费Api 端口号
        /// </summary>
        public const int Api_XF_Port = 8011;
        /// <summary>
        /// 消费Socket 端口号
        /// </summary>
        public const int Socket_XF_Port = 8882;
        /// <summary>
        /// 门禁Socket 端口号
        /// </summary>
        public const int Socket_MJ_Port = 8883;
    }
}
