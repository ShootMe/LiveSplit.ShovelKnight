using System;
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
        IFrames,
        Playthroughs
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

        [Description("Valley Of Dawn (Complete)"), ToolTip("Splits when completing Valley Of Dawn (King Knight)")]
        ValleyOfDawn,
        [Description("Mossy Moutain (Complete)"), ToolTip("Splits when completing Mossy Moutain (King Knight)")]
        MossyMountain,
        [Description("Spectral Ravine (Complete)"), ToolTip("Splits when completing Spectral Ravine (King Knight)")]
        SpectralRavine,
        [Description("Backyard Lab (Complete)"), ToolTip("Splits when completing Backyard Lab (King Knight)")]
        BackyardLab,
        [Description("Enchanted Conclave (Complete)"), ToolTip("Splits when completing Enchanted Conclave (King Knight)")]
        EnchantedConclave,
        [Description("King Pridemoor - King Knight (Kill)"), ToolTip("Splits when killing King Pridemoor")]
        KingPridemoorKill,
        [Description("King Pridemoor - King Knight (Gold)"), ToolTip("Splits when getting gold from King Pridemoor")]
        KingPridemoorGold,

        [Description("Floating Frog Fen (Complete)"), ToolTip("Splits when completing Floating Frog Fen (King Knight)")]
        FloatingFrogFen,
        [Description("Lunkeroths Lagoon (Complete)"), ToolTip("Splits when completing Lunkeroths Lagoon (King Knight)")]
        LunkerothsLagoon,
        [Description("Pressure Plant (Complete)"), ToolTip("Splits when completing Pressure Plant (King Knight)")]
        PressurePlant,
        [Description("Ratsploder Runway (Complete)"), ToolTip("Splits when completing Ratsploder Runway (King Knight)")]
        RatsploderRunway,
        [Description("Troupple King - King Knight (Kill)"), ToolTip("Splits when killing Troupple King")]
        TrouppleKingKill,
        [Description("Troupple King - King Knight (Gold)"), ToolTip("Splits when getting gold from Troupple King")]
        TrouppleKingGold,

        [Description("Torque Lift Torsion (Complete)"), ToolTip("Splits when completing Torque Lift Torsion (King Knight)")]
        TorqueLiftTorsion,
        [Description("Shock Assembly (Complete)"), ToolTip("Splits when completing Shock Assembly (King Knight)")]
        ShockAssembly,
        [Description("Cyclone Sierra (Complete)"), ToolTip("Splits when completing Cyclone Sierra (King Knight)")]
        CycloneSierra,
        [Description("Heavyweight Heights (Complete)"), ToolTip("Splits when completing Heavyweight Heights (King Knight)")]
        HeavyweightHeights,
        [Description("King Birder - King Knight (Kill)"), ToolTip("Splits when killing Troupple King")]
        KingBirderKill,
        [Description("King Birder - King Knight (Gold)"), ToolTip("Splits when getting gold from Troupple King")]
        KingBirderGold,

        [Description("Shrouded Spires (Complete)"), ToolTip("Splits when completing Shrouded Spires (King Knight)")]
        ShroudedSpires,
        [Description("Lava Well (Complete)"), ToolTip("Splits when completing Lava Well (King Knight)")]
        LavaWell,
        [Description("Warp Wrap Keep (Complete)"), ToolTip("Splits when completing Warp Wrap Keep (King Knight)")]
        WarpWrapKeep
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
        EctoplasmChasm = 28,
        SunkenTown = 29,
        BoundingBattlements = 30,
        EnchantedConclave = 31,
        GrandHall = 32,
        BackyardLab = 33,
        DuelistsGrave = 34,
        TurnCoatsTower = 35,
        CardHouse1 = 36,
        KingWorld1Gem = 37,
        SeersStudy = 38,
        KingWorld1Bonus = 39,
        RollingRoadhouse = 40,
        RatsploderRunway = 41,
        PressurePlant = 42,
        AlchemicalAqueducts = 43,
        LunkerothsLagoon = 44,
        DeepSeaTrench = 45,
        ExcavationStation = 46,
        GooGorge = 47,
        BohtosBigBounce = 48,
        FloatingFrogFen = 49,
        AxolonglAlcove = 50,
        RoyalPond = 51,
        BubblingBayou = 52,
        VolcanicVault = 53,
        SprintersShoals = 54,
        CardHouse2 = 55,
        SunkenStockpile = 56,
        KingWorld2Bonus = 57,
        SlipperySummit = 58,
        SpinwulfSactuary = 59,
        ShockAssembly = 60,
        TorqueLiftTorsion = 61,
        AerialBrigade = 62,
        LadderFactory = 63,
        HeavyweightHeights = 64,
        CycloneSierra = 65,
        KingsRoost = 66,
        KingWorld3Relic = 67,
        CardHouse3 = 68,
        KingWorld3Gem = 69,
        KingWorld3Bonus1 = 70,
        KingWorld3Bonus2 = 71,
        KingWorld3Bonus3 = 72,
        FairyGlade = 73,
        ShroudedSpires = 74,
        LavaWell = 75,
        WarpWrapKeep = 76,
        KingEnchantress = 77,
        CardHouse4 = 78,
        Glidewing = 79,
        Homestead = 80,
        FloatEnd = 81,
        PlainsMorningEnd = 82,
        PlainsNightEnd = 83,
        CardHouseNightEnd = 84,
        PridemoorEnd = 85,
        LichYardEnd = 86,
        ExplodatoriumEnd = 87,
        IronWhaleEnd = 88,
        LostCityEnd = 89,
        TroupplePondEnd = 90,
        StrandedShipEnd = 91,
        ClockworkEnd = 92,
        FlyingMachineEnd = 93,
        KingsRoostEnd = 94,
        TowerOfFateEnd = 95,
        GrandHallIntro = 96,
        RoyalPondIntro = 97,
        KingsRoostIntro = 98,
        TowerOfFateIntro = 99,
        GrandHallOutro = 100,
        RoyalPondOutro = 101,
        KingsRoostOutro = 102,
        KingEncounterBaz = 103,
        KingEncounterPhantomStriker = 104,
        EerieManor = 105,
        KingEncounterMoleKnight = 106,
        KingEncounterPlagueKnight = 107,
        KingEncounterTresureKnight = 108,
        KingEncounterTinkerKnight = 109,
        KingEncounterPolarKnight = 110,
        KingEncounterPropellerKnight = 111,
        KingEncounterSpecterKnight = 112,
        VoidCrater = 113,
        GigaCardia = 114,
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