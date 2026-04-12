public enum InputKey
{
    None,

    AlwaysEnabled,

    ClickEnvironment,

    #region Slots

    HoverSlotTeam,
    HoverSlotTeamRandom = 64,
    HoverSlotCharge = 4,
    HoverSlotShopBot,
    HoverSlotShopItem,
    HoverSlotBattle = 66,

    ClickSlotTeam = 7,
    ClickSlotCharge,
    ClickSlotShopBot,
    ClickSlotShopItem,

    DragSlotTeam,
    DragSlotCharge,
    DragSlotShopBot,
    DragSlotShopItem,

    DropSlotTeam,
    DropSlotTeamRandom = 65,
    DropSlotCharge = 16,

    #endregion



    #region Buttons

    HoverButtonReplay,
    
    HoverButtonRoll,
    HoverButtonRecycle,
    HoverButtonRepair,
    HoverButtonLock,
    HoverButtonUnlock,
    HoverButtonEndTurn,

    ClickButtonReplay,

    ClickButtonRoll,
    ClickButtonRecycle,
    ClickButtonRepair,
    ClickButtonLock,
    ClickButtonUnlock,
    ClickButtonEndTurn,

    ClickButtonPlayInBattle,

    DropButtonRoll,
    DropButtonRecycle,
    DropButtonRepair,
    DropButtonLock,
    DropButtonUnlock,
    DropButtonEndTurn,

    #endregion

}
