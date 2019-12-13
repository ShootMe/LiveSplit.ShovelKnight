using System.Threading;
namespace LiveSplit.ShovelKnight {
    public class SplitterTest {
        private static SplitterComponent comp = new SplitterComponent(null);
        public static void Main(string[] args) {
            Thread t = new Thread(GetVals);
            t.IsBackground = true;
            t.Start();
            System.Windows.Forms.Application.Run();
        }
        private static void GetVals() {
            while (true) {
                try {
                    comp.GetValues();

                    Thread.Sleep(8);
                } catch { }
            }
        }
    }
}