namespace SecureWebClient.ViewModels
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Xml.Linq;

    /// <summary>
    /// ユーザーの選択状態を保持するプリファレンスクラス。
    /// </summary>
    public class UserPreferences
    {
        /// <summary>選択されたURL。</summary>
        public string? SelectedUrl { get; set; }
        /// <summary>選択されたタイプ。</summary>
        public string? SelectedType { get; set; }
        /// <summary>選択された詳細。</summary>
        public string? SelectedDetail { get; set; }
    }

    /// <summary>
    /// メイン画面のViewModel。各種コンボボックスのデータや選択状態を管理します。
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>URL選択用コンボボックスのアイテム一覧。</summary>
        public ObservableCollection<ComboBoxItemViewModel> UrlComboBoxItems { get; }
        /// <summary>タイプ選択用コンボボックスのアイテム一覧。</summary>
        public ObservableCollection<ComboBoxItemViewModel> TypeComboBoxItems { get; }
        /// <summary>詳細選択用コンボボックスのアイテム一覧。</summary>
        public ObservableCollection<ComboBoxItemViewModel> DetailComboBoxItems { get; }

        /// <summary>
        /// 詳細選択が変更されたときに発生するイベント。
        /// </summary>
        public event EventHandler? SelectedDetailChanged;

        private string? _selectedUrl;
        /// <summary>
        /// 選択中のURL。
        /// </summary>
        public string? SelectedUrl
        {
            get => _selectedUrl;
            set
            {
                if (_selectedUrl != value)
                {
                    _selectedUrl = value;
                    OnPropertyChanged(nameof(SelectedUrl));
                    SelectedDetailChanged?.Invoke(this, EventArgs.Empty);
                    SavePreferences();
                }
            }
        }

        private string? _selectedType;
        /// <summary>
        /// 選択中のタイプ。
        /// </summary>
        public string? SelectedType
        {
            get => _selectedType;
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged(nameof(SelectedType));
                    LoadDetailItemsByType(_selectedType);
                    SavePreferences();
                }
            }
        }

        private string? _selectedDetail;
        /// <summary>
        /// 選択中の詳細。
        /// </summary>
        public string? SelectedDetail
        {
            get => _selectedDetail;
            set
            {
                if (_selectedDetail != value)
                {
                    _selectedDetail = value;
                    OnPropertyChanged(nameof(SelectedDetail));
                    SelectedDetailChanged?.Invoke(this, EventArgs.Empty);
                    SavePreferences();
                }
            }
        }

        private XDocument? _xmlDoc;
        private const string PrefsFile = "userprefs.json";
        private UserPreferences _prefs = new();

        /// <summary>
        /// MainViewModelの新しいインスタンスを初期化します。
        /// </summary>
        public MainViewModel()
        {
            UrlComboBoxItems = new ObservableCollection<ComboBoxItemViewModel>();
            TypeComboBoxItems = new ObservableCollection<ComboBoxItemViewModel>();
            DetailComboBoxItems = new ObservableCollection<ComboBoxItemViewModel>();
            LoadComboBoxItemsFromXml("setting.xml");
            LoadPreferences();
            SetInitialSelections();
        }

        /// <summary>
        /// XMLファイルからコンボボックスのアイテムを読み込みます。
        /// </summary>
        /// <param name="xmlPath">XMLファイルのパス。</param>
        private void LoadComboBoxItemsFromXml(string xmlPath)
        {
            if (!File.Exists(xmlPath))
                return;

            _xmlDoc = XDocument.Load(xmlPath);

            // URL
            var urlItems = _xmlDoc.Root?.Element("URL")?.Element("Items");
            if (urlItems != null)
            {
                foreach (var item in urlItems.Elements("Item"))
                {
                    UrlComboBoxItems.Add(new ComboBoxItemViewModel
                    {
                        Value = (string?)item.Element("Value") ?? string.Empty,
                        DisplayName = (string?)item.Element("DisplayName") ?? string.Empty
                    });
                }
            }

            // Type
            var typeItems = _xmlDoc.Root?.Element("Type")?.Element("Items");
            if (typeItems != null)
            {
                foreach (var item in typeItems.Elements("Item"))
                {
                    TypeComboBoxItems.Add(new ComboBoxItemViewModel
                    {
                        Value = (string?)item.Element("Value") ?? string.Empty,
                        DisplayName = (string?)item.Element("DisplayName") ?? string.Empty
                    });
                }
            }
        }

        /// <summary>
        /// 選択されたタイプに応じて詳細アイテムを読み込みます。
        /// </summary>
        /// <param name="typeName">タイプ名。</param>
        private void LoadDetailItemsByType(string? typeName)
        {
            DetailComboBoxItems.Clear();
            if (_xmlDoc == null || string.IsNullOrEmpty(typeName))
                return;

            var detailTypes = _xmlDoc.Root?.Element("Detail")?.Element("Types");
            var typeElement = detailTypes?.Elements("Type")
                .FirstOrDefault(x => (string?)x.Attribute("name") == typeName);

            var items = typeElement?.Element("Items");
            if (items != null)
            {
                foreach (var item in items.Elements("Item"))
                {
                    DetailComboBoxItems.Add(new ComboBoxItemViewModel
                    {
                        Value = (string?)item.Element("Value") ?? string.Empty,
                        DisplayName = (string?)item.Element("DisplayName") ?? string.Empty
                    });
                }
            }

            // Detailの初期選択
            if (DetailComboBoxItems.Count > 0)
            {
                if (!string.IsNullOrEmpty(_prefs.SelectedDetail) &&
                    DetailComboBoxItems.Any(x => x.Value == _prefs.SelectedDetail))
                {
                    SelectedDetail = _prefs.SelectedDetail;
                }
                else
                {
                    SelectedDetail = DetailComboBoxItems[0].Value;
                }
            }
            else
            {
                SelectedDetail = null;
            }
        }

        /// <summary>
        /// ユーザーのプリファレンス情報をファイルから読み込みます。
        /// </summary>
        private void LoadPreferences()
        {
            if (File.Exists(PrefsFile))
            {
                try
                {
                    var json = File.ReadAllText(PrefsFile);
                    var prefs = JsonSerializer.Deserialize<UserPreferences>(json);
                    if (prefs != null)
                        _prefs = prefs;
                }
                catch { }
            }
        }

        /// <summary>
        /// ユーザーのプリファレンス情報をファイルに保存します。
        /// </summary>
        private void SavePreferences()
        {
            _prefs.SelectedUrl = SelectedUrl;
            _prefs.SelectedType = SelectedType;
            _prefs.SelectedDetail = SelectedDetail;
            try
            {
                var json = JsonSerializer.Serialize(_prefs);
                File.WriteAllText(PrefsFile, json);
            }
            catch { }
        }

        /// <summary>
        /// 初期選択状態を設定します。
        /// </summary>
        private void SetInitialSelections()
        {
            // URL
            if (UrlComboBoxItems.Count > 0)
            {
                if (!string.IsNullOrEmpty(_prefs.SelectedUrl) &&
                    UrlComboBoxItems.Any(x => x.Value == _prefs.SelectedUrl))
                {
                    SelectedUrl = _prefs.SelectedUrl;
                }
                else
                {
                    SelectedUrl = UrlComboBoxItems[0].Value;
                }
            }

            // Type
            if (TypeComboBoxItems.Count > 0)
            {
                if (!string.IsNullOrEmpty(_prefs.SelectedType) &&
                    TypeComboBoxItems.Any(x => x.Value == _prefs.SelectedType))
                {
                    SelectedType = _prefs.SelectedType;
                }
                else
                {
                    SelectedType = TypeComboBoxItems[0].Value;
                }
            }
        }

        /// <summary>
        /// プロパティ変更イベント。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// プロパティ変更通知を発行します。
        /// </summary>
        /// <param name="propertyName">プロパティ名。</param>
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}