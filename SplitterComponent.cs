using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.ShovelKnight {
	public class SplitterComponent : IComponent {
		public string ComponentName { get { return "Shovel Knight Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		private static string LOGFILE = "_ShovelKnight.log";
		internal static string[] keys = { "CurrentSplit", "BossKills", "Level", "LevelName", "LevelLoad", "Character", "HP", "BossHP", "Gold", "Mana", "ExtraItems", "Checkpoint" };
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private SplitterMemory mem;
		private int currentSplit = -1, lastLogCheck = 0;
		private int lastBossHP = 0, lastMaxBossHP = 0, lastHP = 0, lastGold = 0, lastCheckpoint = 0, bossKills = 0;
		private Level lastLevel, lastLevelLoading;
		private bool hasLog = false;
		private SplitterSettings settings;

		public SplitterComponent(LiveSplitState state) {
			mem = new SplitterMemory();
			foreach (string key in keys) {
				currentValues[key] = "";
			}

			if (state != null) {
				Model = new TimerModel() { CurrentState = state };
				state.OnReset += OnReset;
				state.OnPause += OnPause;
				state.OnResume += OnResume;
				state.OnStart += OnStart;
				state.OnSplit += OnSplit;
				state.OnUndoSplit += OnUndoSplit;
				state.OnSkipSplit += OnSkipSplit;
			}

			settings = new SplitterSettings(Model);
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

			if (Model != null) {
				HandleSplits();
			}

			LogValues();
		}
		private void HandleSplits() {
			bool shouldSplit = false;
			Level level = mem.LevelID();
			Level levelLoading = mem.LevelIDLoading();

			if (currentSplit == -1) {
				shouldSplit = level == Level.ProfileSelect && levelLoading == Level.IntroCinematic;
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit < Model.CurrentState.Run.Count && currentSplit < settings.Splits.Count) {
					SplitName split = settings.Splits[currentSplit];

					int? bossHP = mem.BossHP();
					int? maxBossHP = mem.BossMaxHP();
					int? HP = mem.HP();
					int? gold = mem.Gold();
					int checkpoint = mem.Checkpoint();

					switch (split) {
						case SplitName.BossEndOverworld: shouldSplit = (levelLoading == Level.Overworld && lastLevelLoading != Level.Overworld) || levelLoading == Level.DarkVillage && lastLevelLoading != Level.DarkVillage; break;
						case SplitName.MemoryOverworld: shouldSplit = (level == Level.SepiaTowerIntro || level == Level.SepiaTowerOfDeath || level == Level.SepiaCampFire || level == Level.SepiaTowerShieldKnight) && ((levelLoading == Level.Overworld && lastLevelLoading != Level.Overworld) || (levelLoading == Level.DarkVillage && lastLevelLoading != Level.DarkVillage)); break;
						case SplitName.BossGainHP: shouldSplit = bossHP >= 12 && maxBossHP >= 2 && lastMaxBossHP == 0; break;
						case SplitName.Checkpoint: shouldSplit = checkpoint > 0 && checkpoint > lastCheckpoint && lastCheckpoint != 0; break;

						case SplitName.BlackKnight1Kill: shouldSplit = level == Level.PlainsOfPassage && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.BlackKnight1Gold: shouldSplit = level == Level.PlainsOfPassage && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.KingKnightKill: shouldSplit = level == Level.PridemoorKeep && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.KingKnightGold: shouldSplit = level == Level.PridemoorKeep && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.SpecterKnightKill: shouldSplit = level == Level.LichYard && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.SpecterKnightGold: shouldSplit = level == Level.LichYard && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.PlagueKnightKill: shouldSplit = level == Level.Explodatorium && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.PlagueKnightGold: shouldSplit = level == Level.Explodatorium && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.TreasureKnightKill: shouldSplit = level == Level.IronWhale && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.TreasureKnightGold: shouldSplit = level == Level.IronWhale && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.MoleKnightKill: shouldSplit = level == Level.LostCity && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.MoleKnightGold: shouldSplit = level == Level.LostCity && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.TinkerKnightFirstKill:
							if (bossKills == 0 && level == Level.ClockworkTower && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
								shouldSplit = true;
							}
							break;
						case SplitName.TinkerKnightKill:
							if (bossKills < 2 && level == Level.ClockworkTower && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
								shouldSplit = bossKills == 2;
							}
							break;
						case SplitName.TinkerKnightGold:
							if (bossKills < 2 && level == Level.ClockworkTower && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
							} else if (bossKills == 2 && level == Level.ClockworkTower && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10) {
								shouldSplit = true;
							}
							break;

						case SplitName.PolarKnightKill: shouldSplit = level == Level.StrandedShip && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.PolarKnightGold: shouldSplit = level == Level.StrandedShip && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.PropellerKnightKill: shouldSplit = level == Level.FlyingMachine && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.PropellerKnightGold: shouldSplit = level == Level.FlyingMachine && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.BlackKnight3Kill: shouldSplit = level == Level.TowerOfFateEntrance && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.BlackKnight3Gold: shouldSplit = level == Level.TowerOfFateEntrance && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.BossRushReach:
							PointF? pos = mem.Position();
							shouldSplit = pos.HasValue && level == Level.TowerOfFateAscent && pos.Value.X > 670 && pos.Value.Y < -185;
							break;
						case SplitName.BossRushKill:
							if (bossKills < 9 && level == Level.TowerOfFateAscent && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
								shouldSplit = bossKills == 9;
							}
							break;

						case SplitName.Enchantress1Kill:
							if (bossKills == 0 && level == Level.TowerOfFateEnchantress && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
								shouldSplit = true;
							}
							break;
						case SplitName.Enchantress2Kill:
							if (bossKills < 2 && level == Level.TowerOfFateEnchantress && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
								shouldSplit = bossKills == 2;
							}
							break;
						case SplitName.Enchantress3Kill:
							if (bossKills < 3 && level == Level.TowerOfFateEnchantress && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10) {
								bossKills++;
								shouldSplit = bossKills == 3;
							}
							break;

						case SplitName.BlackKnight2Kill: shouldSplit = level == Level.EnocunterGem1 && mem.Character() == Character.PlagueKnight && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.BlackKnight2Gold: shouldSplit = level == Level.EnocunterGem1 && mem.Character() == Character.PlagueKnight && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.DarkReizeKill: shouldSplit = level == Level.DarkVillage && mem.Character() == Character.SpecterKnight && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
						case SplitName.DarkReizeGold: shouldSplit = level == Level.DarkVillage && mem.Character() == Character.SpecterKnight && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 10; break;

						case SplitName.ShieldKnightKill: shouldSplit = level == Level.SepiaTowerShieldKnight && mem.Character() == Character.SpecterKnight && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 10; break;
					}

					if (shouldSplit && split.ToString().IndexOf("Kill") > 0 && split != SplitName.TinkerKnightFirstKill && split != SplitName.TinkerKnightKill && split != SplitName.BossRushKill && split != SplitName.Enchantress1Kill && split != SplitName.Enchantress2Kill && split != SplitName.Enchantress3Kill) {
						bossKills++;
					}

					if (bossHP.HasValue) { lastBossHP = bossHP.Value; }
					if (maxBossHP.HasValue) { lastMaxBossHP = maxBossHP.Value; }
					if (HP.HasValue) { lastHP = HP.Value; }
					if (gold.HasValue) { lastGold = gold.Value; }
					lastCheckpoint = checkpoint;
				}
			}

			if (level != lastLevel || (lastHP == 0 && level != Level.TowerOfFateEnchantress)) {
				bossKills = 0;
			}
			HandleSplit(shouldSplit, (lastLevel != Level.MainMenu && level == Level.MainMenu) || (lastLevel != Level.ProfileSelect && level == Level.ProfileSelect));

			lastLevel = level;
			lastLevelLoading = levelLoading;
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit < 0) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				string prev = "", curr = "";
				foreach (string key in keys) {
					prev = currentValues[key];
					curr = prev;

					switch (key) {
						case "CurrentSplit":
							if (currentSplit < 0) {
								curr = "Not Running (-1)";
							} else if (currentSplit < settings.Splits.Count) {
								curr = settings.Splits[currentSplit].ToString() + " (" + currentSplit + ")";
							} else {
								curr = "(" + currentSplit.ToString() + ")";
							}
							break;
						case "BossKills": curr = bossKills.ToString(); break;
						case "Level": curr = mem.LevelID().ToString(); break;
						case "LevelName": curr = mem.LevelName(); break;
						case "LevelLoad": curr = mem.LevelIDLoading().ToString(); break;
						case "Character": curr = mem.Character().ToString(); break;
						case "ExtraItems": curr = mem.ExtraItems().ToString(); break;
						case "Checkpoint": curr = mem.Checkpoint().ToString(); break;
						case "HP":
							int? hp = mem.HP();
							if (hp.HasValue) {
								curr = hp.Value + " / " + mem.MaxHP().Value;
							}
							break;
						case "BossHP":
							int? bossHP = mem.BossHP();
							if (bossHP.HasValue) {
								curr = bossHP.Value + " / " + mem.BossMaxHP().Value;
							}
							break;
						case "Gold":
							int? gold = mem.Gold();
							if (gold.HasValue) {
								curr = gold.Value.ToString();
							}
							break;
						case "Mana":
							int? items = mem.Mana();
							if (items.HasValue) {
								curr = items.Value.ToString();
							}
							break;
						case "Pos":
							PointF? pos = mem.Position();
							if (pos.HasValue) {
								curr = pos.Value.X.ToString("0") + "," + pos.Value.Y.ToString("0");
							}
							break;
					}

					if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
					if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
					if (!prev.Equals(curr)) {
						WriteLogWithTime(key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			IList<ILayoutComponent> components = lvstate.Layout.LayoutComponents;
			for (int i = components.Count - 1; i >= 0; i--) {
				ILayoutComponent component = components[i];
				if (component.Component is SplitterComponent && invalidator == null && width == 0 && height == 0) {
					components.Remove(component);
				}
			}

			GetValues();
		}
		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			WriteLog("---------New Game v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + "----------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (Console.IsOutputRedirected) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				} else {
					Console.WriteLine(data);
				}
			}
		}
		private void WriteLogWithTime(string data) {
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
		}

		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() { }
	}
}