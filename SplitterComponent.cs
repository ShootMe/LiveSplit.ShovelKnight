﻿using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.ShovelKnight {
    public class SplitterComponent : IComponent {
        public string ComponentName { get { return "Shovel Knight Autosplitter"; } }
        public TimerModel Model { get; set; }
        public IDictionary<string, Action> ContextMenuControls { get { return null; } }
        private static string LOGFILE = "_ShovelKnight.txt";
        private Dictionary<LogObject, string> currentValues = new Dictionary<LogObject, string>();
        private SplitterMemory mem;
        private SplitterSettings settings;
        private int currentSplit = -1, lastLogCheck = 0, lastPlaythroughs = 0;
        private int lastBossHP = 0, lastMaxBossHP = 0, lastHP = 0, lastGold = 0, lastCheckpoint = 0, bossKills = 0;
        private Level lastLevel, lastLevelLoading;
        private bool hasLog = false;
        private Thread updateLoop;

        public SplitterComponent(LiveSplitState state) {
            mem = new SplitterMemory();
            settings = new SplitterSettings();
            foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
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

                updateLoop = new Thread(UpdateLoop);
                updateLoop.IsBackground = true;
                updateLoop.Start();
            }
        }
        private void UpdateLoop() {
            while (updateLoop != null) {
                try {
                    GetValues();
                } catch (Exception ex) {
                    WriteLog(ex.ToString());
                }
                Thread.Sleep(8);
            }
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

            int? bossHP = mem.BossHP();
            int? maxBossHP = mem.BossMaxHP();
            int? HP = mem.HP();
            int? maxHP = mem.MaxHP();
            int? gold = mem.Gold();
            int checkpoint = mem.Checkpoint();

            Model.CurrentState.Run.Metadata.SetCustomVariable("Level", $"{level}");
            Model.CurrentState.Run.Metadata.SetCustomVariable("BossHP", bossHP.HasValue ? $"{bossHP} / {maxBossHP}" : null);
            Model.CurrentState.Run.Metadata.SetCustomVariable("PlayerHP", HP.HasValue ? $"{HP} / {maxHP}" : null);
            Model.CurrentState.Run.Metadata.SetCustomVariable("Checkpoint", $"{checkpoint}");
            Model.CurrentState.Run.Metadata.SetCustomVariable("PlayerGold", $"{gold}");
            Model.CurrentState.Run.Metadata.SetCustomVariable("PlayerMana", $"{mem.Mana()}");
            Model.CurrentState.Run.Metadata.SetCustomVariable("Character", $"{mem.Character()}");

            if (currentSplit == -1) {
                int playthroughs = mem.Playthroughs();
                shouldSplit = level == Level.ProfileSelect && playthroughs > lastPlaythroughs;
                lastPlaythroughs = playthroughs;
            } else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
                if (currentSplit < Model.CurrentState.Run.Count && currentSplit < settings.Splits.Count) {
                    SplitName split = settings.Splits[currentSplit];
                    
                    switch (split) {
                        case SplitName.BossEndOverworld: shouldSplit = ReturnToOverworld(levelLoading); break;
                        case SplitName.EnterLevel: shouldSplit = (levelLoading != Level.None && levelLoading != Level.MainMenu && levelLoading != Level.CompanyLogo && lastLevelLoading == Level.None) && (level == Level.Overworld || level == Level.DarkVillage); break;
                        case SplitName.MemoryOverworld: shouldSplit = (level == Level.SepiaTowerIntro || level == Level.SepiaTowerOfDeath || level == Level.SepiaCampFire || level == Level.SepiaTowerShieldKnight) && ((levelLoading == Level.Overworld && lastLevel != Level.Overworld) || (levelLoading == Level.DarkVillage && lastLevel != Level.DarkVillage)); break;
                        case SplitName.BossGainHP: shouldSplit = bossHP >= 10 && maxBossHP >= 2 && lastMaxBossHP == 0; break;
                        case SplitName.Checkpoint: shouldSplit = checkpoint > 0 && checkpoint > lastCheckpoint && lastCheckpoint != 0; break;

                        case SplitName.BlackKnight1Kill: shouldSplit = level == Level.PlainsOfPassage && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.BlackKnight1Gold: shouldSplit = level == Level.PlainsOfPassage && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.KingKnightKill: shouldSplit = level == Level.PridemoorKeep && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.KingKnightGold: shouldSplit = level == Level.PridemoorKeep && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.SpecterKnightKill: shouldSplit = level == Level.LichYard && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.SpecterKnightGold: shouldSplit = level == Level.LichYard && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.PlagueKnightKill: shouldSplit = level == Level.Explodatorium && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.PlagueKnightGold: shouldSplit = level == Level.Explodatorium && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.TreasureKnightKill: shouldSplit = level == Level.IronWhale && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.TreasureKnightGold: shouldSplit = level == Level.IronWhale && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.MoleKnightKill: shouldSplit = level == Level.LostCity && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.MoleKnightGold: shouldSplit = level == Level.LostCity && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.TinkerKnightFirstKill:
                            if (bossKills == 0 && level == Level.ClockworkTower && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                                shouldSplit = true;
                            }
                            break;
                        case SplitName.TinkerKnightKill:
                            if (bossKills < 2 && level == Level.ClockworkTower && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                                shouldSplit = bossKills == 2;
                            }
                            break;
                        case SplitName.TinkerKnightGold:
                            if (bossKills < 2 && level == Level.ClockworkTower && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                            } else if (bossKills == 2 && level == Level.ClockworkTower && BossGold(bossHP, maxBossHP, HP, gold)) {
                                shouldSplit = true;
                            }
                            break;

                        case SplitName.PolarKnightKill: shouldSplit = level == Level.StrandedShip && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.PolarKnightGold: shouldSplit = level == Level.StrandedShip && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.PropellerKnightKill: shouldSplit = level == Level.FlyingMachine && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.PropellerKnightGold: shouldSplit = level == Level.FlyingMachine && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.BlackKnight3Kill: shouldSplit = level == Level.TowerOfFateEntrance && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.BlackKnight3Gold: shouldSplit = level == Level.TowerOfFateEntrance && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.BossRushReach:
                            PointF? pos = mem.Position();
                            shouldSplit = pos.HasValue && level == Level.TowerOfFateAscent && pos.Value.X > 670 && pos.Value.Y < -185;
                            break;
                        case SplitName.BossRushKill:
                            if (bossKills < 9 && level == Level.TowerOfFateAscent && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                                shouldSplit = bossKills == 9;
                            }
                            break;

                        case SplitName.Enchantress1Kill:
                            if (bossKills == 0 && (level == Level.TowerOfFateEnchantress || level == Level.KingEnchantress) && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                                shouldSplit = true;
                            }
                            break;
                        case SplitName.Enchantress2Kill:
                            if (bossKills < 2 && (level == Level.TowerOfFateEnchantress || level == Level.KingEnchantress) && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                                shouldSplit = bossKills == 2;
                            }
                            break;
                        case SplitName.Enchantress3Kill:
                            if (bossKills < 3 && (level == Level.TowerOfFateEnchantress || level == Level.KingEnchantress) && BossKilled(bossHP, maxBossHP, HP)) {
                                bossKills++;
                                shouldSplit = bossKills == 3;
                            }
                            break;

                        case SplitName.BlackKnight2Kill: shouldSplit = (level == Level.EncounterBlackKnight || level == Level.EncounterKnight1 || level == Level.EncounterKnight2 || level == Level.EncounterGem1 || level == Level.EncounterGem2) && mem.Character() == Character.PlagueKnight && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.BlackKnight2Gold: shouldSplit = (level == Level.EncounterBlackKnight || level == Level.EncounterKnight1 || level == Level.EncounterKnight2 || level == Level.EncounterGem1 || level == Level.EncounterGem2) && mem.Character() == Character.PlagueKnight && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.DarkReizeKill: shouldSplit = level == Level.DarkVillage && mem.Character() == Character.SpecterKnight && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.DarkReizeGold: shouldSplit = level == Level.DarkVillage && mem.Character() == Character.SpecterKnight && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.ShieldKnightKill: shouldSplit = level == Level.SepiaTowerShieldKnight && mem.Character() == Character.SpecterKnight && BossKilled(bossHP, maxBossHP, HP); break;

                        case SplitName.ValleyOfDawn: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.ValleyOfDawn; break;
                        case SplitName.MossyMountain: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.MossyMountain; break;
                        case SplitName.SpectralRavine: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.SpectralRavine; break;
                        case SplitName.BackyardLab: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.BackyardLab; break;
                        case SplitName.EnchantedConclave: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.EnchantedConclave; break;
                        case SplitName.SunkenTown: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.SunkenTown; break;
                        case SplitName.EctoplasmChasm: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.EctoplasmChasm; break;
                        case SplitName.EerieManor: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.EerieManor; break;
                        case SplitName.DuelistsGrave: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.DuelistsGrave; break;
                        case SplitName.BoundingBattlements: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.BoundingBattlements; break;
                        case SplitName.TurncoatsTower: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.TurncoatsTower; break;
                        case SplitName.CardHouse1: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.CardHouse1; break;
                        case SplitName.KingPridemoorKill: shouldSplit = level == Level.GrandHall && mem.Character() == Character.KingKnight && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.KingPridemoorGold: shouldSplit = level == Level.GrandHall && mem.Character() == Character.KingKnight && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.FloatingFrogFen: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.FloatingFrogFen; break;
                        case SplitName.LunkerothsLagoon: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.LunkerothsLagoon; break;
                        case SplitName.PressurePlant: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.PressurePlant; break;
                        case SplitName.RatsploderRunway: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.RatsploderRunway; break;
                        case SplitName.BubblingBayou: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.BubblingBayou; break;
                        case SplitName.ExcavationStation: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.ExcavationStation; break;
                        case SplitName.BohtosBigBounce: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.BohtosBigBounce; break;
                        case SplitName.AlchemicalAqueducts: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.AlchemicalAqueducts; break;
                        case SplitName.VolcanicVault: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.VolcanicVault; break;
                        case SplitName.SprintersShoals: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.SprintersShoals; break;
                        case SplitName.DeepSeaTrench: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.DeepSeaTrench; break;
                        case SplitName.GooGorge: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.GooGorge; break;
                        case SplitName.AxolonglAlcove: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.AxolonglAlcove; break;
                        case SplitName.CardHouse2: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.CardHouse2; break;
                        case SplitName.TrouppleKingKill: shouldSplit = level == Level.RoyalPond && mem.Character() == Character.KingKnight && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.TrouppleKingGold: shouldSplit = level == Level.RoyalPond && mem.Character() == Character.KingKnight && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.TorqueLiftTorsion: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.TorqueLiftTorsion; break;
                        case SplitName.ShockAssembly: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.ShockAssembly; break;
                        case SplitName.CycloneSierra: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.CycloneSierra; break;
                        case SplitName.HeavyweightHeights: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.HeavyweightHeights; break;
                        case SplitName.SlipperySummit: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.SlipperySummit; break;
                        case SplitName.SpinwulfSactuary: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.SpinwulfSactuary; break;
                        case SplitName.AerialBrigade: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.AerialBrigade; break;
                        case SplitName.LadderFactory: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.LadderFactory; break;
                        case SplitName.VoidCrater: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.VoidCrater; break;
                        case SplitName.CardHouse3: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.CardHouse3; break;
                        case SplitName.KingBirderKill: shouldSplit = level == Level.KingsRoost && mem.Character() == Character.KingKnight && BossKilled(bossHP, maxBossHP, HP); break;
                        case SplitName.KingBirderGold: shouldSplit = level == Level.KingsRoost && mem.Character() == Character.KingKnight && BossGold(bossHP, maxBossHP, HP, gold); break;

                        case SplitName.ShroudedSpires: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.ShroudedSpires; break;
                        case SplitName.LavaWell: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.LavaWell; break;
                        case SplitName.WarpWrapKeep: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.WarpWrapKeep; break;
                        case SplitName.CardHouse4: shouldSplit = ReturnToOverworld(levelLoading) && lastLevel == Level.CardHouse4; break;
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

            if ((level != lastLevel && level != Level.RespawnScreen && lastLevel != Level.RespawnScreen) || (lastHP == 0 && level != Level.TowerOfFateEnchantress && level != Level.RespawnScreen)) {
                bossKills = 0;
            }
            HandleSplit(shouldSplit, false);

            lastLevel = level;
            lastLevelLoading = levelLoading;
        }
        private bool ReturnToOverworld(Level levelLoading) {
            return (levelLoading == Level.Overworld && lastLevelLoading != Level.Overworld && lastLevel != Level.Overworld && lastLevel != Level.Glidewing)
                || (levelLoading == Level.DarkVillage && lastLevelLoading != Level.DarkVillage && lastLevel != Level.DarkVillage)
                || (levelLoading >= Level.FloatEnd && levelLoading <= Level.TowerOfFateEnd && (lastLevelLoading < Level.FloatEnd || lastLevelLoading > Level.TowerOfFateEnd) && lastLevel != Level.Overworld);
        }
        private bool BossKilled(int? bossHP, int? maxBossHP, int? HP) {
            return bossHP == 0 && lastBossHP > 0 && HP > 0 && lastHP > 0 && maxBossHP >= 12;
        }
        private bool BossGold(int? bossHP, int? maxBossHP, int? HP, int? gold) {
            return bossHP == 0 && HP > 0 && gold > lastGold && maxBossHP >= 12;
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
                string prev = string.Empty, curr = string.Empty;
                foreach (LogObject key in Enum.GetValues(typeof(LogObject))) {
                    prev = currentValues[key];
                    curr = prev;

                    switch (key) {
                        case LogObject.CurrentSplit:
                            if (currentSplit < 0) {
                                curr = "Not Running (-1)";
                            } else if (currentSplit < settings.Splits.Count) {
                                curr = settings.Splits[currentSplit].ToString() + " (" + currentSplit + ")";
                            } else {
                                curr = "(" + currentSplit.ToString() + ")";
                            }
                            break;
                        case LogObject.BossKills: curr = bossKills.ToString(); break;
                        case LogObject.Playthroughs: curr = mem.Playthroughs().ToString(); break;
                        case LogObject.Level: curr = mem.LevelID().ToString(); break;
                        case LogObject.LevelName: curr = mem.LevelName(); break;
                        case LogObject.LevelLoad: curr = mem.LevelIDLoading().ToString(); break;
                        case LogObject.Character: curr = mem.Character().ToString(); break;
                        case LogObject.ExtraItems: curr = mem.ExtraItems().ToString(); break;
                        case LogObject.Checkpoint: curr = mem.Checkpoint().ToString(); break;
                        case LogObject.LevelTimer: curr = mem.LevelTimer().GetValueOrDefault(0).ToString("0"); break;
                        case LogObject.HP:
                            int? hp = mem.HP();
                            if (hp.HasValue) {
                                curr = hp.Value + " / " + mem.MaxHP().Value;
                            }
                            break;
                        case LogObject.BossHP:
                            int? bossHP = mem.BossHP();
                            if (bossHP.HasValue) {
                                curr = bossHP.Value + " / " + mem.BossMaxHP().Value;
                            }
                            break;
                        case LogObject.Gold:
                            int? gold = mem.Gold();
                            if (gold.HasValue) {
                                curr = gold.Value.ToString();
                            }
                            break;
                        case LogObject.Mana:
                            int? items = mem.Mana();
                            if (items.HasValue) {
                                curr = items.Value.ToString();
                            }
                            break;
                        case LogObject.Pos:
                            PointF? pos = mem.Position();
                            if (pos.HasValue) {
                                curr = pos.Value.X.ToString("0") + "," + pos.Value.Y.ToString("0");
                            }
                            break;
                        case LogObject.IFrames:
                            float? time = mem.IFrameDuration();
                            if (time.HasValue) {
                                curr = time.Value.ToString("0");
                            }
                            break;
                    }

                    if (string.IsNullOrEmpty(prev)) { prev = string.Empty; }
                    if (string.IsNullOrEmpty(curr)) { curr = string.Empty; }
                    if (!prev.Equals(curr)) {
                        WriteLogWithTime(key.ToString() + ": ".PadRight(16 - key.ToString().Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

                        currentValues[key] = curr;
                    }
                }
            }
        }
        public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
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
        public void Dispose() {
            if (updateLoop != null) {
                updateLoop = null;
            }
            if (Model != null) {
                Model.CurrentState.OnReset -= OnReset;
                Model.CurrentState.OnPause -= OnPause;
                Model.CurrentState.OnResume -= OnResume;
                Model.CurrentState.OnStart -= OnStart;
                Model.CurrentState.OnSplit -= OnSplit;
                Model.CurrentState.OnUndoSplit -= OnUndoSplit;
                Model.CurrentState.OnSkipSplit -= OnSkipSplit;
                Model = null;
            }
        }
    }
}