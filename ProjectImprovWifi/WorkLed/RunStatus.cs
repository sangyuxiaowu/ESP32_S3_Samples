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
        /// 请求识别，蓝色快闪
        /// </summary>
        OnIdentify,

        /// <summary>
        /// 验证成功，灯绿色常亮
        /// </summary>
        AuthSuccess,

        /// <summary>  
        /// wifi连接中,橙色快烁
        /// </summary>  
        Connecting,

        /// <summary>  
        /// wifi 配置问题，红色闪烁  
        /// </summary>  
        ConfigFailed,

        /// <summary>  
        /// wifi 连接失败，红色常亮  
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
