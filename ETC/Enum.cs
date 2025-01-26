
public enum ItemType
{
    Coin1, Coin5, Coin10, Coin15, Coin100, Key, CardPack, ForcedDeletion, ProgramRemove, ProgramRecycle
        , Heal, TemHp, Shiled, Spark, HPFull
        , Card_Clover, Card_Spade, Card_Hearth, Card_Dia
        , Ticket_Random, Ticket_Down, Ticket_Shop, Ticket_Special, Ticket_BlackShop, Ticket_Boss
        , ExpansionKit_1, ExpansionKit_2, ExpansionKit_3
}

public enum FolderType
{
    Start,           // 시작 지점
    Boss,            // 보스 폴더
    Shop,            // 상점 폴더
    Download,        // 다운로드 폴더
    RandomSpecial,   // 랜덤 특수 폴더
    Hidden,          // 숨겨진 폴더 (특정 조건에서만 진입 가능)
    Middle,          // 일반 폴더 (노멀 폴더)
    End,             // 엔드포인트 폴더 (오른쪽 포탈 없음)
    MiddleBoss       // 중간 보스 폴더
}

public enum MonsterType
{
    M_V1,
    M_V2,
    M_V3,
    M_CardPack,
    M_VE_1,
    M_VE_2,
    M_SpiderCardPack,
    Red_Spider,
    White_Spider,
    Boss_Mouse
}

public enum UI
{
    UI_MyPC, UI_DownLoad, UI_MyDocument, UI_LocalDisk, UI_NetWork, UI_Control, UI_Help
}