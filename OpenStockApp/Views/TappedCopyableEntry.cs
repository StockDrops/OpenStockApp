using CommunityToolkit.Maui.Alerts;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenStockApp.Views
{
    public class TappedCopyableEntry : Entry
    {
        public static readonly BindableProperty CopiedCommandProperty = BindableProperty.Create(nameof(CopiedCommand), typeof(ICommand), typeof(TappedCopyableEntry), null);
        public static readonly BindableProperty CopiedCommandParameterProperty = BindableProperty.Create(nameof(CopiedCommandParameter), typeof(string), typeof(TappedCopyableEntry), null);
        public ICommand? CopiedCommand
        {
            get => (ICommand)GetValue(CopiedCommandProperty);
            set => SetValue(CopiedCommandProperty, value);
        }
        public string? CopiedCommandParameter
        {
            get => (string?)GetValue(CopiedCommandParameterProperty);
            set => SetValue(CopiedCommandParameterProperty, value);
        }

        public TappedCopyableEntry() : base()
        {
            GestureRecognizers.Add(new TapGestureRecognizer 
            {

                Command = new Command(() =>
                {
                    if (!string.IsNullOrEmpty(Text))
                        Dispatcher.Dispatch(async () =>
                        {
                            await Clipboard.SetTextAsync(Text);
                            if (CopiedCommand?.CanExecute(CopiedCommandParameter) == true)
                                CopiedCommand.Execute(CopiedCommandParameter);
                        });
                })
            });
#if WINDOWS
            GestureRecognizers.Add(new ClickGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (!string.IsNullOrEmpty(Text))
                        Dispatcher.Dispatch(async () =>
                        {
                            await Clipboard.SetTextAsync(Text);
                            if (CopiedCommand?.CanExecute(CopiedCommandParameter) == true)
                                CopiedCommand.Execute(CopiedCommandParameter);
                        });
                })
            });
            SetDefaultCopiedCommand();
#endif
        }
        protected void SetDefaultCopiedCommand()
        {
            CopiedCommand = new Command(() => { Toast.Make($"Copied {CopiedCommandParameter} to Clipboard").Show(); });
        }


    }
}
