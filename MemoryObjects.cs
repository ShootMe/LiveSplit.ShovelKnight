﻿using System;
using System.ComponentModel;
namespace LiveSplit.ShovelKnight {
    public enum LogObject {
        CurrentSplit,
        BossKills,
        Level,
        LevelName,
        LevelLoad,
        Character,
        HP,
        BossHP,
        Gold,
        Mana,
        ExtraItems,
        Checkpoint,
        LevelTimer,
        Pos,
        IFrames
    }
    public enum SplitName {
        [Description("Manual Split (Not Automatic)"), ToolTip("Does not split automatically. Use this for custom splits not yet defined.")]
        ManualSplit,

        [Description("Return to Overworld / Hub / Airship (Transition)"), ToolTip("Splits when going back to the overworld / hub / airship at any point")]
        BossEndOverworld,
        [Description("Enter Level from Overworld / Hub (Transition)"), ToolTip("Splits when entering a level from the overworld / hub")]
        EnterLevel,
        [Description("Memory to Hub (Specter Knight)"), ToolTip("Splits when going back to the overworld / hub map from the memory sequence")]
        MemoryOverworld,
        [Description("Boss Gaining HP (Boss Start)"), ToolTip("Splits when the boss starts gaining HP")]
        BossGainHP,
        [Description("Checkpoint (Activated)"), ToolTip("Splits when activating a checkpoint in a map")]
        Checkpoint,

        [Description("Black Knight - Plains (Kill)"), ToolTip("Splits when killing Black Knight in Plains")]
        BlackKnight1Kill,
        [Description("Black Knight - Plains (Gold)"), ToolTip("Splits when getting gold from Black Knight in Plains")]
        BlackKnight1Gold,

        [Description("King Knight - Pridemoor Keep (Kill)"), ToolTip("Splits when killing King Knight")]
        KingKnightKill,
        [Description("King Knight - Pridemoor Keep (Gold)"), ToolTip("Splits when getting gold from King Knight")]
        KingKnightGold,

        [Description("Specter Knight - Lich Yard (Kill)"), ToolTip("Splits when killing Specter Knight")]
        SpecterKnightKill,
        [Description("Specter Knight - Lich Yard (Gold)"), ToolTip("Splits when getting gold from Specter Knight")]
        SpecterKnightGold,

        [Description("Plague Knight - Explodatorium (Kill)"), ToolTip("Splits when killing Plague Knight")]
        PlagueKnightKill,
        [Description("Plague Knight - Explodatorium (Gold)"), ToolTip("Splits when getting gold from Plague Knight")]
        PlagueKnightGold,

        [Description("Treasure Knight - Iron Whale (Kill)"), ToolTip("Splits when killing Treasure Knight")]
        TreasureKnightKill,
        [Description("Treasure Knight - Iron Whale (Gold)"), ToolTip("Splits when getting gold from Treasure Knight")]
        TreasureKnightGold,

        [Description("Mole Knight - Lost City (Kill)"), ToolTip("Splits when killing Mole Knight")]
        MoleKnightKill,
        [Description("Mole Knight - Lost City (Gold)"), ToolTip("Splits when getting gold from Mole Knight")]
        MoleKnightGold,

        [Description("Tinker Knight - Clock Tower (Kill First)"), ToolTip("Splits when killing first phase of Tinker Knight")]
        TinkerKnightFirstKill,
        [Description("Tinker Knight - Clock Tower (Kill Both)"), ToolTip("Splits when killing both phases of Tinker Knight")]
        TinkerKnightKill,
        [Description("Tinker Knight - Clock Tower (Gold)"), ToolTip("Splits when getting gold from Tinker Knight")]
        TinkerKnightGold,

        [Description("Polar Knight - Stranded Ship (Kill)"), ToolTip("Splits when killing Polar Knight")]
        PolarKnightKill,
        [Description("Polar Knight - Stranded Ship (Gold)"), ToolTip("Splits when getting gold from Polar Knight")]
        PolarKnightGold,

        [Description("Propeller Knight - Flying Machine (Kill)"), ToolTip("Splits when killing Propeller Knight")]
        PropellerKnightKill,
        [Description("Propeller Knight - Flying Machine (Gold)"), ToolTip("Splits when getting gold from Propeller Knight")]
        PropellerKnightGold,

        [Description("Black Knight - Tower of Fate (Kill)"), ToolTip("Splits when killing Black Knight in Tower of Fate Entrance")]
        BlackKnight3Kill,
        [Description("Black Knight - Tower of Fate (Gold)"), ToolTip("Splits when getting gold from Black Knight in Tower of Fate Entrance")]
        BlackKnight3Gold,

        [Description("Ascent Boss Rush - Tower of Fate (Reach Room)"), ToolTip("Splits when reaching the boss rush in the Tower of Fate Ascent")]
        BossRushReach,
        [Description("Ascent Boss Rush - Tower of Fate (Kill)"), ToolTip("Splits when killing all bosses in Tower of Fate Ascent")]
        BossRushKill,

        [Description("Enchantress 1 - Tower of Fate (Kill)"), ToolTip("Splits when killing Enchantress 1")]
        Enchantress1Kill,
        [Description("Enchantress 2 - Tower of Fate (Kill)"), ToolTip("Splits when killing Enchantress 2 / Nightmare Reize / Plague of Shadows / Mega King")]
        Enchantress2Kill,
        [Description("Enchantress 3 - Tower of Fate (Kill)"), ToolTip("Splits when killing Corrupted Essence / Dead King")]
        Enchantress3Kill,

        [Description("Black Knight 2 - Plague Knight (Kill)"), ToolTip("Splits when killing Black Knight 2")]
        BlackKnight2Kill,
        [Description("Black Knight 2 - Plague Knight (Gold)"), ToolTip("Splits when getting gold from Black Knight 2")]
        BlackKnight2Gold,

        [Description("Dark Reize - Specter Knight (Kill)"), ToolTip("Splits when killing Dark Reize")]
        DarkReizeKill,
        [Description("Dark Reize - Specter Knight (Gold)"), ToolTip("Splits when getting gold from Dark Reize")]
        DarkReizeGold,

        [Description("Shield Knight - Specter Knight (Kill)"), ToolTip("Splits when killing Shield Knight")]
        ShieldKnightKill,

        [Description("King Pridemoor - King Knight (Kill)"), ToolTip("Splits when killing King Pridemoor")]
        KingPridemoorKill,
        [Description("King Pridemoor - King Knight (Gold)"), ToolTip("Splits when getting gold from King Pridemoor")]
        KingPridemoorGold,

        [Description("Troupple King - King Knight (Kill)"), ToolTip("Splits when killing Troupple King")]
        TrouppleKingKill,
        [Description("Troupple King - King Knight (Gold)"), ToolTip("Splits when getting gold from Troupple King")]
        TrouppleKingGold,

        [Description("King Birder - King Knight (Kill)"), ToolTip("Splits when killing Troupple King")]
        KingBirderKill,
        [Description("King Birder - King Knight (Gold)"), ToolTip("Splits when getting gold from Troupple King")]
        KingBirderGold,
    }
    public enum Character {
        ShovelKnight = 0,
        PlagueKnight = 1,
        SpecterKnight = 2,
        KingKnight = 3
    }
    public enum Level {
        PlainsOfPassage = 13,
        PridemoorKeep = 14,
        LichYard = 15,
        Explodatorium = 16,
        IronWhale = 17,
        LostCity = 18,
        ClockworkTower = 19,
        StrandedShip = 20,
        FlyingMachine = 21,
        TowerOfFateEntrance = 22,
        TowerOfFateAscent = 23,
        TowerOfFateEnchantress = 24,
        ValleyOfDawn = 25,
        MossyMountain = 26,
        SpectralRavine = 27,
        SunkenTown = 29,
        BoundingBattlements = 30,
        EnchantedConclave = 31,
        GrandHall = 32,
        BackyardLab = 33,
        TurnCoatsTower = 35,
        CardHouse1 = 36,
        KingGem1 = 37,
        SeersStudy = 38,
        RollingRoadhouse = 40,
        AlchemicalAqueducts = 43,
        ExcavationStation = 46,
        GooGorge = 47,
        FloatingFrogFen = 49,
        AxolonglAlcove = 50,
        RoyalPond = 51,
        CardHouse2 = 55,
        KingsRoost = 66,
        Glidewing = 79,
        PlainsMorningEnd = 82,
        CardHouseNightEnd = 84,
        PridemoorEnd = 85,
        ExcavationStationEnd = 87,
        GrandHallIntro = 96,
        RoyalPondIntro = 97,
        GrandHallOutro = 100,
        KingEncounterBaz = 103,
        KingEncounterPlagueKnight = 107,
        Village = 115,
        Overworld = 116,
        DarkVillage = 117,
        ArmorOutpost = 118,
        TroupplePond = 119,
        HallOfChampions = 120,
        DreamSequence = 121,
        Shortcut = 124,
        SepiaTowerIntro = 126,
        SepiaTowerShieldKnight = 127,
        SepiaTowerOfDeath = 128,
        SepiaCampFire = 129,
        EncounterGem1 = 130,
        EncounterGem2 = 131,
        ForestOfPhasing = 132,
        KnucklersQuarry = 133,
        FrigidFlight = 134,
        EncounterBlackKnight = 135,
        EncounterPhantomStriker = 136,
        EncounterReize = 137,
        EncounterBaz = 139,
        EncounterKnight1 = 141,
        EncounterKnight2 = 142,
        CompanyLogo = 288,
        MainMenu = 289,
        ProfileSelect = 290,
        RespawnScreen = 291,
        IntroCinematic = 296,
        Feats = 297,
        Challenges = 298,
        SoundTest = 305,
        ShowdownMenu = 340,
        ShowdownBattleMenu = 341,
        ShowdownFeats = 343,
        None = 453
    }
}