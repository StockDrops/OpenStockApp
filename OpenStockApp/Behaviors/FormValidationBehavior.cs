using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Behaviors
{
	/// <summary>
	/// Class inteaded to group validators and combine all validators into a single validator.
	/// This one works with multiple entries.
	/// </summary>
	public class FormValidationBehavior : ValidationBehavior
	{
		///<inheritdoc/>
		public FormValidationBehavior() : base()
		{
			DefaultVariableMultiValueConverter = new VariableMultiValueConverter { ConditionType = MultiBindingCondition.All };
			ConfigurationValueBindingRequired += OnConfigurationRequired;
		}
		/// <summary>
		/// Default VariableMultiValueConverter
		/// </summary>
		protected virtual VariableMultiValueConverter DefaultVariableMultiValueConverter { get; }
		/// <summary>
		/// Backing BindableProperty for the <see cref="VariableMultiValueConverter"/> property.
		/// </summary>
		public static readonly BindableProperty VariableMultiValueConverterProperty =
			BindableProperty.Create(nameof(VariableMultiValueConverter), typeof(VariableMultiValueConverter), typeof(FormValidationBehavior), defaultValueCreator: CreateDefaultConverter);
		static object CreateDefaultConverter(BindableObject bindable) => ((FormValidationBehavior)bindable).DefaultVariableMultiValueConverter;
		/// <summary>
		/// Converter in charge of converting all the IsValid from the individual behaviors to a single boolean that shows in the IsValid.
		/// </summary>
		public VariableMultiValueConverter VariableMultiValueConverter
		{
			get => (VariableMultiValueConverter)GetValue(VariableMultiValueConverterProperty);
			set => SetValue(VariableMultiValueConverterProperty, value);
		}
		/// <summary>
		/// This is the backing field of the References which will hold references to all the behaviors.
		/// </summary>
		readonly ObservableCollection<ValidationBehavior> _behaviors = new();
		/// <summary>
		/// All  behaviors that are part of this <see cref="FormValidationBehavior"/>. This is a bindable property.
		/// </summary>
		public IList<ValidationBehavior> Behaviors => _behaviors;
		/// <summary>
		/// Backing field for the attached Required property.
		/// </summary>
		public static readonly BindableProperty RequiredProperty =
			BindableProperty.CreateAttached("Required", typeof(bool), typeof(FormValidationBehavior), false, propertyChanged: OnRequiredPropertyChanged);
		/// <summary>
		/// Get's the RequiredProperty.
		/// </summary>
		/// <param name="bindable"></param>
		/// <returns></returns>
		public static bool GetRequired(BindableObject bindable)
		{
			return (bool)bindable.GetValue(RequiredProperty);
		}
		/// <summary>
		/// Method to set the error on the attached property for a behavior/>.
		/// </summary>
		/// <param name="bindable">The <see cref="ValidationBehavior"/> on which we set the attached Error property value</param>
		/// <param name="value">The value to set</param>
		public static void SetRequired(BindableObject bindable, bool value)
		{
			bindable.SetValue(RequiredProperty, value);
		}


		static void OnRequiredPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (bindable is ValidationBehavior vb)
			{
				ConfigurationValueBindingRequired?.Invoke(vb, EventArgs.Empty);
			}
		}
		/// <summary>
		/// Event Handler used to tell the class that the binding needs to be updated.
		/// </summary>
		public static event EventHandler? ConfigurationValueBindingRequired;

		void OnConfigurationRequired(object? sender, EventArgs e)
		{
			if (sender is ValidationBehavior validationBehavior)
			{
				Behaviors.Add(validationBehavior);
				ConfigureValueBinding();
			}


		}
		/// <summary>
		/// Method that configures the binding to the ValueProperty
		/// </summary>
		protected void ConfigureValueBinding()
		{
			if (IsBound(ValueProperty))
				return;
			SetBinding(ValueProperty, CreateBinding());
		}
		/// <summary>
		/// Creates an automatic binding to the behaviors that have have set Required = "True"
		/// </summary>
		/// <returns></returns>
		protected BindingBase CreateBinding()
		{
			var bindings = new List<BindingBase>();
			foreach (var behavior in Behaviors)
			{

				bindings.Add(new Binding
				{
					Source = behavior,
					Path = nameof(behavior.IsValid)
				});

			}
			return new MultiBinding
			{
				Bindings = bindings,
				Converter = VariableMultiValueConverter
			};
		}
		///<inheritdoc/>
		protected override ValueTask<bool> ValidateAsync(object? value, CancellationToken token)
		{
			if (value is bool convertedResult)
			{
				return new ValueTask<bool>(convertedResult);
			}
			return new ValueTask<bool>(false);
		}
	}
}
