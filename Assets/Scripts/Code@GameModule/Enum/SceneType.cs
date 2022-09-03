namespace GameModule
{
    public enum SceneType
    {
        TYPE_INVALID,
        TYPE_LAUNCH,
        TYPE_LOADING,
   //     TYPE_LOGIN,
        TYPE_LOBBY,
        TYPE_MACHINE
    }

    public enum SwitchMask
    {
        MASK_SERVER_READY = 1,
        MASK_RESOURCE_READY = 2,
    }
}