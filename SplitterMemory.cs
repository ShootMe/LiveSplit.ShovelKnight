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

		public Character? Character() {
			try {
				return (Character)Program.Read<int>(Program.MainModule.BaseAddress, 0x7f6708);
			} catch {
				return null;
			}
		}
		public Level? LevelID() {
			try {
				return (Level)Program.Read<int>(Program.MainModule.BaseAddress, 0x7FFA3C);
			} catch {
				return null;
			}
		}
		public Level? LevelIDLoading() {
			try {
				return (Level)Program.Read<int>(Program.MainModule.BaseAddress, 0x7FFA40);
			} catch {
				return null;
			}
		}
		public int? Gold() {
			try {
				return Program.Read<int>(Program.MainModule.BaseAddress, 0x8004b0, 0x80, 0xd4, 0x2d0);
			} catch {
				return null;
			}
		}
		public int? ExtraItems() {
			try {
				return Program.Read<int>(Program.MainModule.BaseAddress, 0x7FBBFC);
			} catch {
				return null;
			}
		}
		public int? Mana() {
			try {
				return Program.Read<byte>(Program.MainModule.BaseAddress, 0x8004b0, 0x80, 0xd4, 0x2cc);
			} catch {
				return null;
			}
		}
		public PointF? Position() {
			try {
				return new PointF(Program.Read<float>(Program.MainModule.BaseAddress, 0x8004b0, 0x74, 0x24, 0xc), Program.Read<float>(Program.MainModule.BaseAddress, 0x8004b0, 0x74, 0x24, 0x10));
			} catch {
				return null;
			}
		}
		public int? HP() {
			try {
				return (int)Program.Read<float>(Program.MainModule.BaseAddress, 0x8004b0, 0x80, 0xd4, 0x1c);
			} catch {
				return null;
			}
		}
		//public void HP(int hp) {
		//	try {
		//		Program.Write<float>(Program.MainModule.BaseAddress, (float)hp, 0x8004b0, 0x80, 0xd4, 0x1c);
		//		Program.Write<byte>(Program.MainModule.BaseAddress, (byte)50, 0x8004b0, 0x2a8, 0x330);
		//	} catch { }
		//}
		public int? MaxHP() {
			try {
				return (int)Program.Read<float>(Program.MainModule.BaseAddress, 0x8004b0, 0x80, 0xd4, 0x20);
			} catch {
				return null;
			}
		}
		public int? BossHP() {
			try {
				return (int)Program.Read<byte>(Program.MainModule.BaseAddress, 0x8004b0, 0x80, 0x20, 0xa8, 0x5e4, 0x41);
			} catch {
				return null;
			}
		}
		public int? BossMaxHP() {
			try {
				return (int)Program.Read<byte>(Program.MainModule.BaseAddress, 0x8004b0, 0x80, 0x20, 0xa8, 0x5e4, 0x40);
			} catch {
				return null;
			}
		}
		public int? Checkpoint() {
			try {
				return (int)Program.Read<int>(Program.MainModule.BaseAddress, 0x7FC940);
			} catch {
				return null;
			}
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
		Plains = 11,
		PridemoorKeep = 12,
		LichYard = 13,
		Explodatorium = 14,
		IronWhale = 15,
		LostCity = 16,
		ClockTower = 17,
		StrandedShip = 18,
		FlyingMachine = 19,
		TowerOfFateEntrance = 20,
		TowerOfFateAscent = 21,
		TowerOfFateEnchantress = 22,
		Village = 23,
		DarkReize = 24,
		ArmorOutpost = 25,
		TroupplePond = 26,
		HallOfChampions = 27,
		BossEnd = 28,
		Shortcut = 31,
		ShieldKnight = 38,
		GemOverworld1 = 41,
		GemOverworld2 = 42,
		ForestOfPhasing = 43,
		KnucklersQuarry = 44,
		FrigidFlight = 45,
		BlackKnightOverworld = 46,
		PhantomStrikerOverworld = 47,
		ReizeOverworld = 48,
		BazOverworld = 50,
		OverworldKnight1 = 52,
		OverworldKnight2 = 53,
		MainMenu = 187,
		ProfileSelect = 188,
		Overworld = 189,
		CheckpointScreen = 190,
		IntroCinematic = 195,
		Feats = 196,
		None = 273
	}
	public enum SplitName {
		[Description("Manual Split (Not Automatic)"), ToolTip("Does not split automatically. Use this for custom splits not yet defined.")]
		ManualSplit,

		[Description("Boss End to Overworld (Transition)"), ToolTip("Splits when going from the end sequence after a boss to the overworld map.")]
		BossEndOverworld,
		[Description("Checkpoint (Activated)"), ToolTip("Splits when activating a checkpoint in a map.")]
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

		[Description("Tinker Knight - Clock Tower (Kill)"), ToolTip("Splits when killing Tinker Knight")]
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

		[Description("Tower of Fate - Ascent (Reach Boss Rush)"), ToolTip("Splits when reaching the boss rush in the Tower of Fate Ascent")]
		BossRushReach,
		[Description("Tower of Fate - Acent (Boss Rush Kill)"), ToolTip("Splits when killing all bosses in Tower of Fate Ascent")]
		BossRushKill,

		[Description("Enchantress 1 - Tower of Fate (Kill)"), ToolTip("Splits when killing Enchantress 1")]
		Enchantress1Kill,
		[Description("Enchantress 2 - Tower of Fate (Kill)"), ToolTip("Splits when killing Enchantress 2")]
		Enchantress2Kill,
		[Description("Enchantress 3 - Tower of Fate (Kill)"), ToolTip("Splits when killing Enchantress 3")]
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