using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Text.Json.Nodes;

namespace EasePass.Models
{
    internal class JsonNodeViewModel : INotifyPropertyChanged
    {
        private string _key;
        public string Key
        {
            get => _key;
            set
            {
                if (_key != value)
                {
                    _key = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
                }
            }
        }
        private JsonNode _value;
        public JsonNode Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if(Value is JsonArray)
                    {
                        Children.Clear();
                        var array = Value as JsonArray;
                        for (int i = 0; i < array.Count; i++)
                        {
                            Children.Add(new JsonNodeViewModel { Key = i.ToString(), Value = array[i] });
                        }
                    }
                    if(Value is JsonObject)
                    {
                        Children.Clear();
                        var obj = Value as JsonObject;
                        foreach (var kvp in obj)
                        {
                            Children.Add(new JsonNodeViewModel { Key = kvp.Key, Value = kvp.Value });
                        }
                    }
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpandable)));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));
                }
            }
        }

        public ObservableCollection<JsonNodeViewModel> Children { get; set; } = new();

        public bool IsExpandable => Value is JsonObject or JsonArray;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
