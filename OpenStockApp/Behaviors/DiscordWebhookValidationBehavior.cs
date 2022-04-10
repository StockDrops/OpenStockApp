using CommunityToolkit.Maui.Behaviors;
using OpenStockApp.Contracts;
using OpenStockApp.Discord.Models;
using OpenStockApp.Discord.Services;
using System.Net.Http.Json;

namespace OpenStockApp.Behaviors
{
    //public class FormValidationBehavior : BaseValidationBehavior
    //{
    //    ///<inheritdoc/>
    //    public FormValidationBehavior() : base()
    //    {
    //        DefaultVariableMultiValueConverter = new VariableMultiValueConverter { ConditionType = MultiBindingCondition.All };
    //        ConfigurationValueBindingRequired += OnConfigurationRequired;
    //    }
    //    /// <summary>
    //    /// Default VariableMultiValueConverter
    //    /// </summary>
    //    protected virtual VariableMultiValueConverter DefaultVariableMultiValueConverter { get; }
    //    /// <summary>
    //    /// Backing BindableProperty for the <see cref="VariableMultiValueConverter"/> property.
    //    /// </summary>
    //    public static readonly BindableProperty VariableMultiValueConverterProperty =
    //        BindableProperty.Create(nameof(VariableMultiValueConverter), typeof(VariableMultiValueConverter), typeof(FormValidationBehavior), defaultValueCreator: CreateDefaultConverter);
    //    static object CreateDefaultConverter(BindableObject bindable) => ((FormValidationBehavior)bindable).DefaultVariableMultiValueConverter;
    //    /// <summary>
    //    /// Converter in charge of converting all the IsValid from the individual behaviors to a single boolean that shows in the IsValid.
    //    /// </summary>
    //    public VariableMultiValueConverter VariableMultiValueConverter
    //    {
    //        get => (VariableMultiValueConverter)GetValue(VariableMultiValueConverterProperty);
    //        set => SetValue(VariableMultiValueConverterProperty, value);
    //    }
    //    /// <summary>
    //    /// This is the backing field of the References which will hold references to all the behaviors.
    //    /// </summary>
    //    readonly ObservableCollection<BaseValidationBehavior> _behaviors = new();
    //    /// <summary>
    //    /// All  behaviors that are part of this <see cref="FormValidationBehavior"/>. This is a bindable property.
    //    /// </summary>
    //    public IList<BaseValidationBehavior> Behaviors => _behaviors;

    //    public static readonly BindableProperty RequiredProperty =
    //        BindableProperty.CreateAttached("Required", typeof(bool), typeof(FormValidationBehavior), false, propertyChanged: OnRequiredPropertyChanged);
    //    /// <summary>
    //    /// Get's the RequiredProperty.
    //    /// </summary>
    //    /// <param name="bindable"></param>
    //    /// <returns></returns>
    //    public static bool GetRequired(BindableObject bindable)
    //    {
    //        return (bool)bindable.GetValue(RequiredProperty);
    //    }
    //    /// <summary>
    //    /// Method to set the error on the attached property for a behavior/>.
    //    /// </summary>
    //    /// <param name="bindable">The <see cref="ValidationBehavior"/> on which we set the attached Error property value</param>
    //    /// <param name="value">The value to set</param>
    //    public static void SetRequired(BindableObject bindable, bool value)
    //    {
    //        bindable.SetValue(RequiredProperty, value);
    //    }


    //    static void OnRequiredPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        switch (bindable)
    //        {
    //            case BaseValidationBehavior vb:
    //                ConfigurationValueBindingRequired?.Invoke(vb, EventArgs.Empty);
    //                break;
    //        }
    //    }
    //    /// <summary>
    //    /// Event Handler used to tell the class that the binding needs to be updated.
    //    /// </summary>
    //    public static event EventHandler ConfigurationValueBindingRequired;

    //    private void OnConfigurationRequired(object sender, EventArgs e)
    //    {
    //        Behaviors.Add((BaseValidationBehavior)sender);
    //        ConfigureValueBinding();
    //    }
    //    protected override void ConfigureValueBinding()
    //    {
    //        base.ConfigureValueBinding();
    //    }
    //    ///<inheritdoc/>
    //    protected override BindingBase CreateBinding()
    //    {
    //        var bindings = new List<BindingBase>();
    //        foreach (var behavior in Behaviors)
    //        {

    //            bindings.Add(new Binding
    //            {
    //                Source = behavior,
    //                Path = nameof(behavior.IsValid)
    //            });

    //        }
    //        return new MultiBinding
    //        {
    //            Bindings = bindings,
    //            Converter = VariableMultiValueConverter
    //        };
    //    }

    //    protected override ValueTask<bool> ValidateAsync(object? value, CancellationToken token)
    //    {
    //        if (value is bool convertedResult)
    //        {
    //            return new ValueTask<bool>(convertedResult);
    //        }
    //        return new ValueTask<bool>(false);
    //    }
    //}

    /// <summary>
    /// Use this on value changed. This won't do IO operations that will be bad for behavior.
    /// </summary>
    public class ShortDiscordWebhookValidationBehavior : UriValidationBehavior, IValidationRule
    {
        private readonly string _validUrlPrefix;
        public string ValidationMessage
        {
            get => (string)GetValue(ValidationMessageProperty);
            set => SetValue(ValidationMessageProperty, value);
        }
        public static readonly BindableProperty ValidationMessageProperty =
            BindableProperty.Create(nameof(ValidationMessage), typeof(string), typeof(DiscordWebhookValidationBehavior), string.Empty, BindingMode.OneWay);

        public ShortDiscordWebhookValidationBehavior() : base()
        {
            _validUrlPrefix = "https://discord.com/api/webhooks/";
        }

        /// <inheritdoc/>
        protected override async ValueTask<bool> ValidateAsync(string value, CancellationToken token)
        {
            if (string.IsNullOrEmpty(value))
            {
                ValidationMessage = "Please enter a webhook URL.";
                return false;
            }
            //check if it's an URI first
            if (!await base.ValidateAsync(value, token))
            {
                ValidationMessage = "The entered URL is not a valid URL.";
                return false;
            }
            if (!value.StartsWith(_validUrlPrefix))
            {
                ValidationMessage = $"The webhook URL must start with {_validUrlPrefix}";
                return false;
            }
            return true;
        }
    }
    public class DiscordWebhookValidationBehavior : ShortDiscordWebhookValidationBehavior, IValidationRule
    {
        /// <inheritdoc/>
        protected override async ValueTask<bool> ValidateAsync(string value, CancellationToken token)
        {
            if (!await base.ValidateAsync(value, token))
            {
                return false;
            }
            try
            {
                var isValidServer = await TryValidateServerAsync(value, token);
                if (!isValidServer)
                {
                    ValidationMessage = "Oopss! Couldn't validate the webhook URL with Discord.";
                    return false;
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
            
            return true;
        }
        protected async ValueTask<bool> TryValidateServerAsync(string value, CancellationToken token)
        {
            try
            {
                var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(20); // Wait 2 seconds max
                var httpResponseMessage = await httpClient.SendAsync(new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(value)
                }, token);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var webhook = await httpResponseMessage.Content.ReadFromJsonAsync<DiscordWebhook>(cancellationToken: token);
                    if(webhook != null && !string.IsNullOrEmpty(webhook.Guild_Id))
                        return !DiscordWebhookService.DiscordWebhookServerBlacklist.Contains(webhook.Guild_Id);
                    return false;
                }
                return false;

            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch
            {
                // If an error occurs during the api call just allow the server
                return true;
            }
        }
    }
}
