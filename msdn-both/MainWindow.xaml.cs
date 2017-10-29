using ShComp.Ipc;
using System;
using System.Windows;

namespace msdn_both {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        private IpcMessageSender<String> ipc;
        public MainWindow() {
            InitializeComponent();
            this.addLog("コンストラクタ");
        }

        private void sendButtonCliek(object sender, RoutedEventArgs e) {
            String text = this.sendText.Text.Trim();
            this.sendText.Text = "";
            this.addLog($"送信ボタン押下[{text}]");
        }
        private void logClearButtonCliek(object sender, RoutedEventArgs e) {
            this.logText.Text = "";
        }
        private void addLog(String message) {
            String time = DateTime.Now.ToString("HH:mm:ss.fff");
            this.logText.Text += $"{time} {message}\n";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.addLog("onload");
            var threadId = System.Threading.Thread.CurrentThread.ManagedThreadId;
            this.addLog($"スレッドID=[{threadId}]");
            this.ipc = new IpcMessageSender<string>("sample", "sample");
            if (ipc.IsCadet) {
                this.addLog("すでに起動しています。");
                this.addLog($"hash={ipc.messageHolderHashcode}");
                ipc.SendMessage(string.Format("{0:HH時mm分ss秒}です。後続プロセスが起動しました。", DateTime.Now));
            } else {
                // IPCでオブジェクトを受信したとき発生します。
                this.addLog("先行プロセスです。後続プロセスの起動を待機しています。");
                this.addLog($"hash={ipc.messageHolderHashcode}");
                ipc.MessageReceived += (a, b) => {
                    this.addLog($"受信[{b.Message}]");
                };
            }
        }
    }
}
