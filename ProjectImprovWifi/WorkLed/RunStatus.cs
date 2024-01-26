namespace ProjectImprovWifi.WorkLed
{
    /// <summary>
    /// 定义设备状态
    /// </summary>
    public enum RunStatus
    {
        /// <summary>
        /// 启动中,灯蓝色常亮
        /// </summary>
        Start,
        /// <summary>  
        /// wifi连接中,灯橙色闪烁  
        /// </summary>  
        Connecting,
        /// <summary>  
        /// wifi 配置问题，灯红色闪烁  
        /// </summary>  
        ConfigFailed,
        /// <summary>  
        /// wifi 连接失败，灯红色常亮  
        /// </summary>  
        ConnectFailed,
        /// <summary>  
        /// 正常工作中,绿色呼吸灯  
        /// </summary>
        Working,
        /// <summary>
        /// 关闭灯光
        /// </summary>
        Close
    }
}
