using MvvmGen;
using NJsonSchema;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuppyApp.Wpf.ViewModels
{
    [ViewModel]
    internal partial class CallParameterViewModel : INotifyDataErrorInfo
    {
        private string _label;
        private string _value;
        private bool _isRequired;
        private JsonObjectType _propType;

        public bool IsRequired
        {
            get => _isRequired;
            set
            {
                if (_isRequired != value)
                {
                    _isRequired = value;
                    OnPropertyChanged("IsRequired");
                }
            }
        }

        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    OnPropertyChanged("Label");
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                    ValidateValue();
                }
            }
        }

        public decimal MinValue { get; set; }

        private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();

        public bool HasErrors => _errorsByPropertyName.Any();


        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsByPropertyName.ContainsKey(propertyName) ?
            _errorsByPropertyName[propertyName] : null;
        }

        

        public CallParameterViewModel(string label, string value, bool isRequired, JsonObjectType propTypeName)
        {
            typeValidators = new()
            {
                { JsonObjectType.Integer, ValidateInt }
            };
            _label = label;
            _value = value;
            _isRequired = isRequired;
            _propType = propTypeName;
        }

        private IEnumerable<string> ValidateInt(string v)
        {
            if (Int32.TryParse(v, out int j))
            {
                if (j < MinValue)
                {
                    return new string[] { $"Cannot be less than {MinValue}" };
                }
                else
                    return new string[] { };
            }
            else
            {
                return new string[] { $"Must be an Integer" };

            }
        }
        private readonly Dictionary<JsonObjectType, Func<string, IEnumerable<string>>> typeValidators;


        private void ValidateValue()
        {
            ClearErrors(nameof(Value));
            if (string.IsNullOrWhiteSpace(Value) && IsRequired)
            {
                AddError(nameof(Value), "Cannot be empty.");
            }
            if (typeValidators.TryGetValue(_propType, out var validator))
            {
                var errors = validator(Value);
                foreach (var error in errors)
                {
                    AddError(nameof(Value), error);
                }
            }
        }
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
                _errorsByPropertyName[propertyName] = new List<string>();

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        private void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }

    }
}
