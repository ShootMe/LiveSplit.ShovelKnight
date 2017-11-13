using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.ShovelKnight {
	public class SplitterComponent : IComponent {
		public string ComponentName { get { return "Shovel Knight Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "State", "Level", "LevelLoad", "HP", "BossHP", "Gold", "Items", "Checkpoint" };
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private SplitterMemory mem;
		private int currentSplit = -1, state = 0, lastLogCheck = 0;
		private int lastBossHP = 0, lastHP = 0, lastGold = 0, lastCheckpoint = 0;
		private Level lastLevel;
		private bool hasLog = false;
		private SplitterSettings settings;
		private static string LOGFILE = "_ShovelKnight.log";

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
			Level? level = mem.LevelID();
			Level? levelLoading = mem.LevelIDLoading();

			if (currentSplit == -1) {
				shouldSplit = level == Level.ProfileSelect && levelLoading == Level.IntroCinematic;
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit < Model.CurrentState.Run.Count && currentSplit < settings.Splits.Count) {
					SplitName split = settings.Splits[currentSplit];

					int? bossHP = mem.BossHP();
					int? maxBossHP = mem.BossMaxHP();
					int? HP = mem.HP();
					int? gold = mem.Gold();
					int? checkpoint = mem.Checkpoint();

					switch (split) {
						case SplitName.BossEndOverworld: shouldSplit = level == Level.BossEnd && levelLoading == Level.Overworld; break;
						case SplitName.Checkpoint: shouldSplit = checkpoint > 0 && checkpoint > lastCheckpoint; break;

						case SplitName.BlackKnight1Kill: shouldSplit = level == Level.Plains && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 12; break;
						case SplitName.BlackKnight1Gold: shouldSplit = level == Level.Plains && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 12; break;

						case SplitName.KingKnightKill: shouldSplit = level == Level.PridemoorKeep && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.KingKnightGold: shouldSplit = level == Level.PridemoorKeep && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.SpecterKnightKill: shouldSplit = level == Level.LichYard && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.SpecterKnightGold: shouldSplit = level == Level.LichYard && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.PlagueKnightKill: shouldSplit = level == Level.Explodatorium && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.PlagueKnightGold: shouldSplit = level == Level.Explodatorium && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.TreasureKnightKill: shouldSplit = level == Level.IronWhale && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.TreasureKnightGold: shouldSplit = level == Level.IronWhale && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.MoleKnightKill: shouldSplit = level == Level.LostCity && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.MoleKnightGold: shouldSplit = level == Level.LostCity && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.TinkerKnightKill:
							if (state < 2 && level == Level.ClockTower && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20) {
								state++;
								shouldSplit = state == 2;
							} else if (HP == 0 && lastHP > 0) {
								state = 0;
							}
							break;
						case SplitName.TinkerKnightGold:
							if (state < 2 && level == Level.ClockTower && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20) {
								state++;
							} else if (state == 2 && level == Level.ClockTower && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20) {
								shouldSplit = true;
							} else if (HP == 0 && lastHP > 0) {
								state = 0;
							}
							break;

						case SplitName.PolarKnightKill: shouldSplit = level == Level.StrandedShip && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.PolarKnightGold: shouldSplit = level == Level.StrandedShip && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.PropellerKnightKill: shouldSplit = level == Level.FlyingMachine && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.PropellerKnightGold: shouldSplit = level == Level.FlyingMachine && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.BlackKnight3Kill: shouldSplit = level == Level.TowerOfFateEntrance && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.BlackKnight3Gold: shouldSplit = level == Level.TowerOfFateEntrance && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.BossRushReach:
							PointF? pos = mem.Position();
							shouldSplit = pos.HasValue && level == Level.TowerOfFateAscent && pos.Value.X > 670 && pos.Value.Y < -185;
							break;
						case SplitName.BossRushKill:
							if (state < 9 && level == Level.TowerOfFateAscent && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20) {
								state++;
								shouldSplit = state == 9;
							} else if (HP == 0 && lastHP > 0) {
								state = 0;
							}
							break;

						case SplitName.Enchantress1Kill: shouldSplit = level == Level.TowerOfFateEnchantress && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.Enchantress2Kill: shouldSplit = level == Level.TowerOfFateEnchantress && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.Enchantress3Kill: shouldSplit = level == Level.TowerOfFateEnchantress && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;

						case SplitName.BlackKnight2Kill: shouldSplit = level == Level.GemOverworld1 && mem.Character() == Character.PlagueKnight && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.BlackKnight2Gold: shouldSplit = level == Level.GemOverworld1 && mem.Character() == Character.PlagueKnight && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.DarkReizeKill: shouldSplit = level == Level.DarkReize && mem.Character() == Character.SpecterKnight && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
						case SplitName.DarkReizeGold: shouldSplit = level == Level.DarkReize && mem.Character() == Character.SpecterKnight && bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP == 20; break;

						case SplitName.ShieldKnightKill: shouldSplit = level == Level.ShieldKnight && mem.Character() == Character.SpecterKnight && bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP == 20; break;
					}

					if (bossHP.HasValue) { lastBossHP = bossHP.Value; }
					if (HP.HasValue) { lastHP = HP.Value; }
					if (gold.HasValue) { lastGold = gold.Value; }
					if (checkpoint.HasValue) { lastCheckpoint = checkpoint.Value; }
				}
			}

			HandleSplit(shouldSplit, (lastLevel != Level.MainMenu && level == Level.MainMenu) || (lastLevel != Level.ProfileSelect && level == Level.ProfileSelect));

			if (level.HasValue) { lastLevel = level.Value; }
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
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "Level":
							Level? level = mem.LevelID();
							if (level.HasValue) {
								curr = level.Value.ToString();
							}
							break;
						case "LevelLoad":
							Level? levelLoad = mem.LevelIDLoading();
							if (levelLoad.HasValue) {
								curr = levelLoad.Value.ToString();
							}
							break;
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
						case "Items":
							int? items = mem.Items();
							if (items.HasValue) {
								curr = items.Value.ToString();
							}
							break;
						case "Checkpoint":
							int? cp = mem.Checkpoint();
							if (cp.HasValue) {
								curr = cp.Value.ToString();
							}
							break;
						case "Pos":
							PointF? pos = mem.Position();
							if (pos.HasValue) {
								curr = pos.Value.X.ToString("0") + "," + pos.Value.Y.ToString("0");
							}
							break;
					}

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
			state = 0;
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
			state = 0;
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			state = 0;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
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