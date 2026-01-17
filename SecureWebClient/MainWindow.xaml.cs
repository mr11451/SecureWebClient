using SecureWebClient.ViewModels;
using System.ComponentModel.Design;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SecureWebClient
{
    /// <summary>
    /// MainWindowのロジックを提供します。
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly CommandComposer _commandComposer;

        /// <summary>
        /// MainWindowの新しいインスタンスを初期化します。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            var httpService = new HttpService();
            _commandComposer = new CommandComposer(httpService);
            if (DataContext is MainViewModel vm)
            {
                vm.SelectedDetailChanged += MainViewModel_SelectedDetailChanged;
            }
            MainViewModel_SelectedDetailChanged(null, EventArgs.Empty);
        }

        /// <summary>
        /// 送信ボタンがクリックされたときに呼び出されます。
        /// </summary>
        /// <param name="sender">イベントの送信元。</param>
        /// <param name="e">イベントデータ。</param>
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not MainViewModel vm)
                return;

            var url = vm.SelectedUrl;
            var detail = vm.SelectedDetail;
            var path = _commandComposer.ComposeRequestUrl(url, detail);
            var method = _commandComposer.Method;
            var bodyText = _commandComposer.BodyText;
            string sendTime = DateTime.Now.ToString("HH:mm:ss.fff");
            LogTextBlock.Text += $"{sendTime} {url} {detail}" + Environment.NewLine;
            LogTextBlock.Text += $"({sendTime} {path} {method})" + Environment.NewLine;
            if (!string.IsNullOrEmpty(bodyText))
            {
                LogTextBlock.Text += $"{sendTime} {bodyText}" + Environment.NewLine;
            }
            string log;
            try
            {
                var result = await _commandComposer.CallHttpServiceAsync();
                log = result ?? "(no response)";
            }
            catch (Exception ex)
            {
                log = $"エラー: {ex.Message}";
            }
            ResultTextBox.Text = log;
            string receiveTime = DateTime.Now.ToString("HH:mm:ss.fff");
            LogTextBlock.Text += $"{receiveTime} {log}" + Environment.NewLine;
        }

        /// <summary>
        /// 詳細選択が変更されたときに呼び出されます。
        /// </summary>
        /// <param name="sender">イベントの送信元。</param>
        /// <param name="e">イベントデータ。</param>
        private void MainViewModel_SelectedDetailChanged(object? sender, EventArgs e)
        {
            if (DataContext is not MainViewModel vm)
                return;

            var url = vm.SelectedUrl;
            var detail = vm.SelectedDetail;

            // URL組み立て
            var path = _commandComposer.ComposeRequestUrl(url, detail);

            // 結果をUIに表示
            var method = _commandComposer.Method;
            ResultTextBox.Text = $"{method} {path}";
        }
    }
}