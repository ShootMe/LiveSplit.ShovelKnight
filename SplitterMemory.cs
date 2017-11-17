using LiveSplit.Memory;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
namespace LiveSplit.ShovelKnight {
	public partial class SplitterMemory {
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;

		public SplitterMemory() {
			lastHooked = DateTime.MinValue;
		}

		public Character Character() {
			return (Character)Program.Read<int>(Program.MainModule.BaseAddress, 0x7f6708);
		}
		public Level LevelID() {
			return (Level)Program.Read<int>(Program.MainModule.BaseAddress, 0x7FFA3C);
		}
		public string LevelName() {
			return Program.ReadAscii((IntPtr)Program.Read<uint>(Program.MainModule.BaseAddress, 0x7FFA30) + 0x38);
		}
		public Level LevelIDLoading() {
			return (Level)Program.Read<int>(Program.MainModule.BaseAddress, 0x7FFA40);
		}
		//public void LevelIDLoading(Level level) {
		//	try {
		//		Program.Write<int>(Program.MainModule.BaseAddress, (int)level, 0x7FFA40);
		//	} catch { }
		//}
		public int? Gold() {
			try {
				return Program.Read<int>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0xd4, 0x2d0);
			} catch {
				return null;
			}
		}
		public int ExtraItems() {
			return Program.Read<int>(Program.MainModule.BaseAddress, 0x7FBBFC);
		}
		public int? Mana() {
			try {
				return Program.Read<byte>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0xd4, 0x2cc);
			} catch {
				return null;
			}
		}
		public PointF? Position() {
			try {
				return new PointF(Program.Read<float>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0x94, 0x24, 0xc), Program.Read<float>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0x94, 0x24, 0x10));
			} catch {
				return null;
			}
		}
		//public void Position(float x, float y) {
		//	try {
		//		Program.Write<float>(Program.MainModule.BaseAddress, x, 0x7FFA30, 0x10, 0x78, 0x94, 0x24, 0xc);
		//		Program.Write<float>(Program.MainModule.BaseAddress, y, 0x7FFA30, 0x10, 0x78, 0x94, 0x24, 0x10);
		//	} catch { }
		//}
		public int? HP() {
			try {
				return (int)Program.Read<float>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0xd4, 0x1c);
			} catch {
				return null;
			}
		}
		//public void HP(int hp) {
		//	try {
		//		Program.Write<float>(Program.MainModule.BaseAddress, (float)hp, 0x7FFA30, 0x10, 0x78, 0xd4, 0x1c);
		//	} catch { }
		//}
		public int? MaxHP() {
			try {
				return (int)Program.Read<float>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0xd4, 0x20);
			} catch {
				return null;
			}
		}
		public int? BossHP() {
			try {
				return Program.Read<byte>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0x20, 0xa8, 0x5e4, 0x41);
			} catch {
				return null;
			}
		}
		public int? BossMaxHP() {
			try {
				return Program.Read<byte>(Program.MainModule.BaseAddress, 0x7FFA30, 0x10, 0x78, 0x20, 0xa8, 0x5e4, 0x40);
			} catch {
				return null;
			}
		}
		public int Checkpoint() {
			return Program.Read<int>(Program.MainModule.BaseAddress, 0x7FC940);
		}

		public bool HookProcess() {
			if ((Program == null || Program.HasExited) && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("ShovelKnight");
				Program = processes.Length == 0 ? null : processes[0];
				IsHooked = true;
			}

			if (Program == null || Program.HasExited) {
				IsHooked = false;
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
	public enum Character {
		ShovelKnight = 0,
		PlagueKnight = 1,
		SpecterKnight = 2,
		KingKnight = 3
	}
	public enum Level {
		PlainsOfPassage = 11,
		PridemoorKeep = 12,
		LichYard = 13,
		Explodatorium = 14,
		IronWhale = 15,
		LostCity = 16,
		ClockworkTower = 17,
		StrandedShip = 18,
		FlyingMachine = 19,
		TowerOfFateEntrance = 20,
		TowerOfFateAscent = 21,
		TowerOfFateEnchantress = 22,
		Village = 23,
		DarkVillage = 24,
		ArmorOutpost = 25,
		TroupplePond = 26,
		HallOfChampions = 27,
		DreamSequence = 28,
		Arena = 29, //Doesn't Exist
		Battleground = 30, //Doesn't Exist
		Shortcut = 31,
		Event1 = 32, //Doesn't Exist
		Battletoads = 33,
		BattletoadsBattleStart = 34,
		BattletoadsLevel = 35,
		BattletoadsIntro = 36,
		SepiaTowerIntro = 37,
		SepiaTowerShieldKnight = 38,
		SepiaTowerOfDeath = 39,
		SepiaCampFire = 40,
		EnocunterGem1 = 41,
		EncounterGem2 = 42,
		ForestOfPhasing = 43,
		KnucklersQuarry = 44,
		FrigidFlight = 45,
		EncounterBlackKnight = 46,
		EncounterPhantomStriker = 47,
		EncounterReize = 48,
		EncounterDarkReize = 49, //Doesn't Exist
		EncounterBaz = 50,
		EncounterKratos = 51, //Doesn't Exist
		EncounterKnight1 = 52,
		EncounterKnight2 = 53,
		Challenge01 = 54,
		Challenge02 = 55,
		Challenge03 = 56, //Doesn't Exist
		Challenge04 = 57,
		Challenge05 = 58,
		Challenge06 = 59,
		Challenge07 = 60,
		Challenge08 = 61, //Doesn't Exist
		Challenge09 = 62, //Doesn't Exist
		Challenge10 = 63,
		Challenge11 = 64, //Doesn't Exist
		Challenge12 = 65, //Doesn't Exist
		Challenge13 = 66, //Doesn't Exist
		Challenge14 = 67, //Doesn't Exist
		Challenge15 = 68, //Doesn't Exist
		Challenge16 = 69,
		Challenge17 = 70, //Doesn't Exist
		Challenge18 = 71,
		Challenge19 = 72,
		Challenge20 = 73, //Doesn't Exist
		Challenge21 = 74,
		Challenge22 = 75,
		Challenge23 = 76,
		Challenge24 = 77,
		Challenge25 = 78,
		Challenge26 = 79,
		Challenge27 = 80,
		Challenge28 = 81,
		Challenge29 = 82,
		Challenge30 = 83,
		Challenge31 = 84,
		Challenge32 = 85,
		Challenge33 = 86,
		Challenge34 = 87,
		Challenge35 = 88,
		Challenge36 = 89,
		Challenge37 = 90,
		Challenge38 = 91,
		Challenge39 = 92,
		Challenge40 = 93,
		Challenge41 = 94, //Doesn't Exist
		Challenge42 = 95, //Doesn't Exist
		Challenge43 = 96, //Doesn't Exist
		Challenge44 = 97, //Doesn't Exist
		Challenge45 = 98, //Doesn't Exist
		Challenge46 = 99, //Doesn't Exist
		Challenge47 = 100, //Doesn't Exist
		Challenge48 = 101, //Doesn't Exist
		Challenge49 = 102, //Doesn't Exist
		Challenge50 = 103, //Doesn't Exist
		ClockworkTower2 = 158, //King - Doesn't work
		FlyingMachine2 = 159, //King - Doesn't work
		StrandedShip2 = 160,
		AmbiioChallenge01 = 161, //Doesn't Exist
		AmbiioChallenge02 = 162, //Doesn't Exist
		AmbiioChallenge03 = 163, //Doesn't Exist
		AmbiioChallenge04 = 164, //Doesn't Exist
		AmbiioChallenge05 = 165, //Doesn't Exist
		AmbiioChallenge06 = 166, //Doesn't Exist
		AmbiioChallenge07 = 167, //Doesn't Exist
		AmbiioChallenge08 = 168, //Doesn't Exist
		AmbiioChallenge09 = 169, //Doesn't Exist
		AmbiioChallenge10 = 170, //Doesn't Exist
		AmbiioChallenge11 = 171, //Doesn't Exist
		AmbiioChallenge12 = 172, //Doesn't Exist
		AmbiioChallenge13 = 173, //Doesn't Exist
		AmbiioChallenge14 = 174, //Doesn't Exist
		AmbiioChallenge15 = 175, //Doesn't Exist
		AmbiioChallenge16 = 176, //Doesn't Exist
		AmbiioChallenge17 = 177, //Doesn't Exist
		AmbiioChallenge18 = 178, //Doesn't Exist
		AmbiioChallenge19 = 179, //Doesn't Exist
		AmbiioChallenge20 = 180, //Doesn't Exist
		AmbiioCoopChallenge1 = 181, //Doesn't Exist
		AmbiioCoopChallenge2 = 182, //Doesn't Exist
		AmbiioCoopChallenge3 = 183, //Doesn't Exist
		AmbiioCoopChallenge4 = 184, //Doesn't Exist
		AmbiioCoopChallenge5 = 185, //Doesn't Exist
		CompanyLogo = 186,
		MainMenu = 187,
		ProfileSelect = 188,
		Overworld = 189,
		RespawnScreen = 190,
		GameOver1 = 191,
		HallOfHonor = 192,
		GameOver2 = 193,
		RescaleScreen = 194,
		IntroCinematic = 195,
		Feats = 196,
		ChallengeSelect = 197,
		BodySwap = 198,
		CreditsShovelKnight = 199,
		CharacterSelect = 200,
		SoundTest = 204,
		None = 273
	}
	public enum SplitName {
		[Description("Manual Split (Not Automatic)"), ToolTip("Does not split automatically. Use this for custom splits not yet defined.")]
		ManualSplit,

		[Description("Return to Overworld / Hub (Transition)"), ToolTip("Splits when going back to the overworld / hub map at any point")]
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
		[Description("Enchantress 2 / Nightmare Reize / Plague of Shadows (Kill)"), ToolTip("Splits when killing Enchantress 2 / Nightmare Reize / Plague of Shadows")]
		Enchantress2Kill,
		[Description("Corrupted Essence - Tower of Fate (Kill)"), ToolTip("Splits when killing Corrupted Essence")]
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
	}
}