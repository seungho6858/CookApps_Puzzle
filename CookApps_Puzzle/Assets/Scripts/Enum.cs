public enum Block_Type
{
    Green = 0,
    Yellow,
    Purple,
    Blue,
    Red,
    Max
}

public enum Dir
{
    LU = 0, // Left Up
    U,
    RU, 
    RD, // Right Down
    D,
    LD,
    Max
}

public enum Near_State
{
    Empty,
    Block_Exist,
    Block_NonExist,
    Block_Moving,
    Max
}

public enum Game_State
{
    Ready, // 맵 생성중
    Playing, // 플레이 중
    Pause,
    Max
}