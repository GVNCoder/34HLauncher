using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Launcher.Services
{
    public class BadProviderException : Exception
    {
        public string ProviderName { get; private set; }

        public BadProviderException() : base()
        { }

        public BadProviderException(string message) : base(message)
        { }

        public BadProviderException(string message, Exception innerException) : base(message, innerException)
        { }

        public BadProviderException(string message, string providerName)
            : base(message)
        {
            ProviderName = providerName;
        }

        public BadProviderException(string message, string providerName, Exception innerException)
            : base(message, innerException)
        {
            ProviderName = providerName;
        }

        public override string ToString()
        {
            return $"{base.ToString()} -> Provider name: {ProviderName}";
        }
    }

    // NOTES:
    // add in future: enabled or disabled validation action
    // add in future: error code

    #region Delegates

    public delegate string ValidationAction(object value);
    public delegate string FieldNameProvider();
    public delegate object FieldValueProvider();

    public delegate string ValidationAction<in T>(T value);
    public delegate T FieldValueProvider<out T>();

    #endregion

    #region Interfaces

    public interface IValidationObjectWrapper<T> : IDataErrorInfo
    {
        T InnerObj { get; }
    }

    public interface IValidationLayer : IDataErrorInfo
    {
        // Layer component validator
        ModelValidationComponent ValidationComponent { get; }
    }

    public interface IFieldValidator
    {
        bool HasError { get; }
        string FieldName { get; }

        string CheckFieldToErrors(object fieldValue);
        string CheckFieldToErrors(FieldValueProvider valueProvider);
        void AddValidator(string errorHeader, ValidationAction validationAction);
        void RemoveValidator(string errorHeader);
    }

    public interface IFieldValidator<T>
    {
        bool HasError { get; }
        string FieldName { get; }

        string CheckFieldToErrors(T fieldValue);
        string CheckFieldToErrors(FieldValueProvider<T> valueProvider);
        void AddValidator(string errorHeader, ValidationAction<T> validationAction);
        void RemoveValidator(string errorHeader);
    }

    public interface IModelValidator
    {
        bool HasError { get; }
        bool IsValid { get; }

        void AddValidator(FieldNameProvider nameProvider, string errorHeader, ValidationAction validationAction);
        void RemoveValidator(FieldNameProvider nameProvider, string errorHeder);
        void RemoveValidation(FieldNameProvider nameProvider);

        string CheckFieldToErrors(FieldNameProvider nameProvider, FieldValueProvider valueProvider);
        string CheckFieldToErrors(FieldNameProvider nameProvider, object targetModel);
    }

    public interface IModelValidator<T>
    {
        bool HasError { get; }
        bool IsValid { get; }

        void AddValidator(FieldNameProvider nameProvider, string errorHeader, ValidationAction validationAction);
        void RemoveValidator(FieldNameProvider nameProvider, string errorHeder);
        void RemoveValidation(FieldNameProvider nameProvider);

        string CheckFieldToErrors(FieldNameProvider nameProvider, FieldValueProvider valueProvider);
        string CheckFieldToErrors(FieldNameProvider nameProvider, T targetModel);
    }

    #endregion

    #region Classes

    public class ModelValidationComponent
    {
        public IModelValidator ModelValidator { get; private set; }

        public ModelValidationComponent(IModelValidator modelValidator, Action<IModelValidator> initializationAction)
        {
            ModelValidator = modelValidator;
            initializationAction.Invoke(ModelValidator);
        }
    }

    public abstract class ModelValidationLayer<T> : IValidationObjectWrapper<T>
    {
        protected IModelValidator<T> ModelValidator;

        public bool HasError => ModelValidator.IsValid;
        public T InnerObj { get; protected set; }

        #region IDataErrorInfo implementation

        public virtual string this[string columnName] =>
            Error = ModelValidator.CheckFieldToErrors(() => columnName, InnerObj);

        public virtual string Error { get; protected set; }

        #endregion

        #region Constructors

        protected ModelValidationLayer(T modelForWrapping, IModelValidator<T> modelValidator)
        {
            InnerObj = modelForWrapping;
            ModelValidator = modelValidator;

            InitializeValidator();
        }

        #endregion

        protected abstract void InitializeValidator();
    }

    public abstract class BaseFieldValidationWrapper<T> : IValidationObjectWrapper<T>
    {
        protected IFieldValidator<T> _fieldValidator;

        public bool HasError => _fieldValidator.HasError;
        public T InnerObj { get; protected set; }

        #region IDataErrorInfo implementation

        public string this[string columnName] => Error = _fieldValidator.CheckFieldToErrors(() => InnerObj);
        public string Error { get; protected set; }

        #endregion

        #region Constructors

        protected BaseFieldValidationWrapper(T objectForWrapping, IFieldValidator<T> fieldValidator)
        {
            InnerObj = objectForWrapping;
            _fieldValidator = fieldValidator;

            InitializeValidator();
        }

        #endregion

        protected abstract void InitializeValidator();
    }

    public sealed class ValidatorContainer
    {
        public string ErrorHeader { get; private set; }
        public ValidationAction Action { get; private set; }

        public ValidatorContainer(string errorHeader, ValidationAction action)
        {
            if (string.IsNullOrWhiteSpace(errorHeader) || action == null)
                throw new InvalidOperationException("", new ArgumentNullException($"{nameof(errorHeader)} or {nameof(action)} is null"));
            ErrorHeader = errorHeader;
            Action = action;
        }
    }

    public sealed class FieldValidator<T> : IFieldValidator<T>
    {
        public IDictionary<string, ValidationAction<T>> Validators { get; private set; }

        #region IFieldValidator implementation

        public string FieldName { get; private set; }
        public bool HasError { get; private set; }

        public string CheckFieldToErrors(T fieldValue)
        {
            foreach (var validationPair in Validators)
            {
                var validationResult = validationPair.Value.Invoke(fieldValue);
                if (validationResult == string.Empty) continue;

                HasError = true;
                return validationResult;
            }

            HasError = false;
            return string.Empty;
        }

        public string CheckFieldToErrors(FieldValueProvider<T> valueProvider)
        {
            if (valueProvider == null)
                throw new InvalidOperationException("Null provider", new ArgumentNullException(nameof(valueProvider)));

            return CheckFieldToErrors(valueProvider.Invoke());
        }

        public void AddValidator(string errorHeader, ValidationAction<T> validationAction)
        {
            if (errorHeader == null || validationAction == null)
                throw new InvalidOperationException("Arguments is null",
                    new ArgumentNullException($"{nameof(errorHeader)} or {nameof(validationAction)} is null"));
            if (Validators.ContainsKey(errorHeader)) return;

            Validators.Add(errorHeader, validationAction);
        }

        public void RemoveValidator(string errorHeader)
        {
            if (errorHeader == null)
                throw new InvalidOperationException("Argument is null", new ArgumentNullException(nameof(errorHeader)));

            Validators.Remove(errorHeader);
        }

        #endregion

        #region Constructors

        private FieldValidator()
        {
            Validators = new Dictionary<string, ValidationAction<T>>(1);
            HasError = false;
        }

        public FieldValidator(string fieldName) : this()
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));
            FieldName = fieldName;
        }

        public FieldValidator(FieldNameProvider nameProvider) : this()
        {
            FieldName = ValidateAndGetName(nameProvider);
        }

        #endregion

        private string ValidateAndGetName(FieldNameProvider nameProvider)
        {
            if (nameProvider == null)
                throw new ArgumentNullException(nameof(nameProvider));
            var name = nameProvider.Invoke();
            if (string.IsNullOrWhiteSpace(name))
                throw new BadProviderException("Provider return not valid value", nameof(nameProvider));

            return name;
        }
        private object ValidateAndGetValue(FieldValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));
            return valueProvider.Invoke();
        }
    }

    public sealed class FieldValidator : IFieldValidator
    {
        public IDictionary<string, ValidationAction> Validators { get; private set; }

        #region IFieldValidator implementation

        public string FieldName { get; private set; }
        public bool HasError { get; private set; }

        public string CheckFieldToErrors(object fieldValue)
        {
            foreach (var validationPair in Validators)
            {
                var validationResult = validationPair.Value.Invoke(fieldValue);
                if (validationResult == string.Empty) continue;

                HasError = true;
                return validationResult;
            }

            HasError = false;
            return string.Empty;
        }

        public string CheckFieldToErrors(FieldValueProvider valueProvider)
        {
            return CheckFieldToErrors(ValidateAndGetValue(valueProvider));
        }

        public void AddValidator(string errorHeader, ValidationAction validationAction)
        {
            if (errorHeader == null || validationAction == null)
                throw new InvalidOperationException("Arguments is null",
                    new ArgumentNullException($"{nameof(errorHeader)} or {nameof(validationAction)} is null"));
            if (Validators.ContainsKey(errorHeader)) return;

            Validators.Add(errorHeader, validationAction);
        }

        public void RemoveValidator(string errorHeader)
        {
            if (errorHeader == null)
                throw new InvalidOperationException("Argument is null", new ArgumentNullException(nameof(errorHeader)));

            Validators.Remove(errorHeader);
        }

        #endregion

        #region Constructors

        private FieldValidator()
        {
            Validators = new Dictionary<string, ValidationAction>(1);
            HasError = false;
        }

        public FieldValidator(string fieldName) : this()
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentNullException(nameof(fieldName));
            FieldName = fieldName;
        }

        public FieldValidator(FieldNameProvider nameProvider) : this()
        {
            FieldName = ValidateAndGetName(nameProvider);
        }

        #endregion

        private string ValidateAndGetName(FieldNameProvider nameProvider)
        {
            if (nameProvider == null)
                throw new ArgumentNullException(nameof(nameProvider));
            var name = nameProvider.Invoke();
            if (string.IsNullOrWhiteSpace(name))
                throw new BadProviderException("Provider return not valid value", nameof(nameProvider));

            return name;
        }
        private object ValidateAndGetValue(FieldValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));
            return valueProvider.Invoke();
        }
    }

    public sealed class ModelValidator<T> : IModelValidator<T>
    {
        private readonly ICollection<FieldValidator> _fieldValidators;
        private readonly Type _modelType;

        public bool HasError { get; private set; }
        public bool IsValid => CheckModel();

        #region Constructors

        public ModelValidator()
        {
            _fieldValidators = new HashSet<FieldValidator>();
            _modelType = typeof(T);

            HasError = false;
        }

        #endregion

        #region IModelValidator implementation

        public void AddValidator(FieldNameProvider nameProvider, string errorHeader, ValidationAction validationAction)
        {
            var fieldName = ValidateAndGetName(nameProvider);

            if (_fieldValidators.All((item) => item.FieldName != fieldName))
                _fieldValidators.Add(new FieldValidator(fieldName));
            _fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName)?.AddValidator(errorHeader, validationAction);
        }

        public void RemoveValidator(FieldNameProvider nameProvider, string errorHeder)
        {
            var fieldName = ValidateAndGetName(nameProvider);
            _fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName)?.RemoveValidator(errorHeder);
        }

        public void RemoveValidation(FieldNameProvider nameProvider)
        {
            var fieldName = ValidateAndGetName(nameProvider);
            _fieldValidators.Remove(_fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName));
        }

        public string CheckFieldToErrors(FieldNameProvider nameProvider, FieldValueProvider valueProvider)
        {
            var fieldName = ValidateAndGetName(nameProvider);
            var fieldValue = ValidateAndGetValue(valueProvider);

            return ValidateField(fieldName, fieldValue);
        }

        public string CheckFieldToErrors(FieldNameProvider nameProvider, T targetModel)
        {
            if (targetModel == null)
                throw new ArgumentNullException(nameof(targetModel));

            var fieldName = ValidateAndGetName(nameProvider);
            var targetProperty = _modelType.GetProperty(fieldName);
            if (targetProperty == null) return string.Empty;

            var fieldValue = targetProperty.GetValue(targetModel);
            return _fieldValidators.Any((item) => item.FieldName == fieldName) ? ValidateField(fieldName, fieldValue) : string.Empty;
        }

        private string ValidateField(string fieldName, object fieldValue)
        {
            var fieldValidator = _fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName);
            foreach (var validator in fieldValidator.Validators)
            {
                var validationResult = validator.Value.Invoke(fieldValue);
                if (string.IsNullOrEmpty(validationResult)) continue;

                HasError = true;
                return $"{validator.Key}: {validationResult}";
            }

            return string.Empty;
        }

        // Not work
        private bool CheckModel()
        {
            return !(HasError = _fieldValidators.Any((item) => item.HasError));
        }

        #endregion

        private string ValidateAndGetName(FieldNameProvider nameProvider)
        {
            if (nameProvider == null)
                throw new ArgumentNullException(nameof(nameProvider));
            var name = nameProvider.Invoke();
            if (string.IsNullOrWhiteSpace(name))
                throw new BadProviderException("Provider return not valid value", nameof(nameProvider));

            return name;
        }
        private object ValidateAndGetValue(FieldValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));
            return valueProvider.Invoke();
        }
    }

    public sealed class ModelValidator : IModelValidator
    {
        private readonly ICollection<FieldValidator> _fieldValidators;
        private readonly Type _modelType;

        public bool HasError { get; private set; }
        public bool IsValid => CheckModel();

        #region Constructors

        public ModelValidator(Type modelType)
        {
            _fieldValidators = new HashSet<FieldValidator>();
            _modelType = modelType;

            HasError = false;
        }

        #endregion

        #region IModelValidator implementation

        public void AddValidator(FieldNameProvider nameProvider, string errorHeader, ValidationAction validationAction)
        {
            var fieldName = ValidateAndGetName(nameProvider);

            if (_fieldValidators.All((item) => item.FieldName != fieldName))
                _fieldValidators.Add(new FieldValidator(fieldName));
            _fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName)?.AddValidator(errorHeader, validationAction);
        }

        public void RemoveValidator(FieldNameProvider nameProvider, string errorHeder)
        {
            var fieldName = ValidateAndGetName(nameProvider);
            _fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName)?.RemoveValidator(errorHeder);
        }

        public void RemoveValidation(FieldNameProvider nameProvider)
        {
            var fieldName = ValidateAndGetName(nameProvider);
            _fieldValidators.Remove(_fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName));
        }

        public string CheckFieldToErrors(FieldNameProvider nameProvider, FieldValueProvider valueProvider)
        {
            var fieldName = ValidateAndGetName(nameProvider);
            var fieldValue = ValidateAndGetValue(valueProvider);

            return ValidateField(fieldName, fieldValue);
        }

        public string CheckFieldToErrors(FieldNameProvider nameProvider, object targetModel)
        {
            if (targetModel == null)
                throw new ArgumentNullException(nameof(targetModel));

            var fieldName = ValidateAndGetName(nameProvider);
            var targetProperty = _modelType.GetProperty(fieldName);
            if (targetProperty == null) return string.Empty;

            var fieldValue = targetProperty.GetValue(targetModel);
            return _fieldValidators.Any((item) => item.FieldName == fieldName) ? ValidateField(fieldName, fieldValue) : string.Empty;
        }

        private string ValidateField(string fieldName, object fieldValue)
        {
            var fieldValidator = _fieldValidators.FirstOrDefault((item) => item.FieldName == fieldName);
            foreach (var validator in fieldValidator.Validators)
            {
                var validationResult = validator.Value.Invoke(fieldValue);
                if (string.IsNullOrEmpty(validationResult)) continue;

                HasError = true;
                return $"{validator.Key}: {validationResult}";
            }

            return string.Empty;
        }

        // Not work
        private bool CheckModel()
        {
            return !(HasError = _fieldValidators.Any((item) => item.HasError));
        }

        #endregion

        private string ValidateAndGetName(FieldNameProvider nameProvider)
        {
            if (nameProvider == null)
                throw new ArgumentNullException(nameof(nameProvider));
            var name = nameProvider.Invoke();
            if (string.IsNullOrWhiteSpace(name))
                throw new BadProviderException("Provider return not valid value", nameof(nameProvider));

            return name;
        }
        private object ValidateAndGetValue(FieldValueProvider valueProvider)
        {
            if (valueProvider == null)
                throw new ArgumentNullException(nameof(valueProvider));
            return valueProvider.Invoke();
        }
    }

    #endregion
}