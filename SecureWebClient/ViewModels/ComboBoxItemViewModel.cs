using System.ComponentModel;

public class ComboBoxItemViewModel : INotifyPropertyChanged
{
    private string _value;
    private string _DisplayName;

    public string Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public string DisplayName
    {
        get => _DisplayName;
        set
        {
            if (_DisplayName != value)
            {
                _DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}