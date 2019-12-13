using LiveSplit.Memory;
using System;
using System.Diagnostics;
using System.Drawing;
namespace LiveSplit.ShovelKnight {
    public partial class SplitterMemory {
        public Process Program { get; set; }
        public bool IsHooked { get; set; } = false;
        public IntPtr BaseAddress { get; set; }
        public DateTime LastHooked;

        public SplitterMemory() {
            LastHooked = DateTime.MinValue;
            BaseAddress = IntPtr.Zero;
        }

        public Character Character() {
            return (Character)Program.Read<int>(BaseAddress, 0x8c0348).GetValueOrDefault(0);
        }
        public Level LevelID() {
            return (Level)Program.Read<int>(BaseAddress, 0x9172ec).GetValueOrDefault(0);
        }
        public string LevelName() {
            return Program.ReadAscii((IntPtr)Program.Read<uint>(BaseAddress, 0x9172e0) + 0x3c);
        }
        public Level LevelIDLoading() {
            return (Level)Program.Read<int>(BaseAddress, 0x9172f0).GetValueOrDefault(0);
        }
        public void LevelIDLoading(Level level) {
            Program.Write<int>(BaseAddress, (int)level, 0x9172f0);
        }
        public int? Gold() {
            return Program.Read<int>(BaseAddress, 0x8b5710, 0x3dc);
        }
        public int ExtraItems() {
            return Program.Read<int>(BaseAddress, 0x8d25e0).GetValueOrDefault(0);
        }
        public int? Mana() {
            return Program.Read<byte>(BaseAddress, 0x8b5710, 0x3d8);
        }
        public float? LevelTimer() {
            return Program.Read<float>(BaseAddress, 0x8b5710, 0x2c, 0x48);
        }
        public float? IFrameDuration() {
            return Program.Read<float>(BaseAddress, 0x8b5710, 0x200);
        }
        public int? HP() {
            return (int?)Program.Read<float>(BaseAddress, 0x8b5710, 0xe8);
        }
        public int? MaxHP() {
            return (int?)Program.Read<float>(BaseAddress, 0x8b5710, 0xec);
        }
        public PointF? Position() {
            float? x = Program.Read<float>(BaseAddress, 0x8b5710, 0x30, 0x24, 0xc);
            float? y = Program.Read<float>(BaseAddress, 0x8b5710, 0x30, 0x24, 0x10);
            if (x.HasValue && y.HasValue) {
                return new PointF(x.Value, y.Value);
            }
            return null;
        }
        public void Position(float x, float y) {
            Program.Write<float>(BaseAddress, x, 0x8b5710, 0x30, 0x24, 0xc);
            Program.Write<float>(BaseAddress, y, 0x8b5710, 0x30, 0x24, 0x10);
        }
        public int? BossHP() {
            //First Enemy HP
            //Program.Read<float>(BaseAddress, 0x8b5710, 0x2c, 0x4, 0x0, 0xa0, 0x164);

            //Boss UI HP
            return (int?)Program.Read<byte>(BaseAddress, 0x8b5710, 0x2c, 0x1e4, 0x650, 0x41);
        }
        public int? BossMaxHP() {
            return (int?)Program.Read<byte>(BaseAddress, 0x8b5710, 0x2c, 0x1e4, 0x650, 0x40);
        }
        public int Checkpoint() {
            return Program.Read<int>(BaseAddress, 0x8d45d8).GetValueOrDefault(0);
        }

        public bool HookProcess() {
            IsHooked = Program != null && !Program.HasExited;
            if (!IsHooked && DateTime.Now > LastHooked.AddSeconds(1)) {
                LastHooked = DateTime.Now;
                Process[] processes = Process.GetProcessesByName("ShovelKnight");
                Program = processes != null && processes.Length > 0 ? processes[0] : null;

                if (Program != null && !Program.HasExited) {
                    MemoryReader.Update64Bit(Program);
                    BaseAddress = Program.MainModule.BaseAddress;
                    IsHooked = true;
                }
            }

            return IsHooked;
        }
        public void Dispose() {
            if (Program != null) {
                Program.Dispose();
            }
        }
    }
}