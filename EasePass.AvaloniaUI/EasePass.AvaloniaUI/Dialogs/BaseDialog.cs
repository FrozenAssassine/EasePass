using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaDialogs.Views;
using EasePass.Converter;
using EasePass.Services;
using System.Linq;
using System.Threading.Tasks;

namespace EasePass.Dialogs
{
    public class BaseDialogClosingArgs
    {
        public DialogResult Result { get; set; }
        public bool Cancel { get; set; }
    }

    public class BaseDialog : BaseDialog<DialogResult>
    {
        private readonly StackPanel _buttonPanel;
        private readonly ContentControl _contentContainer;
        private readonly TextBlock _titleBlock;

        public delegate void DialogClosingEvent(object? sender, BaseDialogClosingArgs args);
        public event DialogClosingEvent? Closing;

        public string? PrimaryButtonText { get; set; }
        public string? SecondaryButtonText { get; set; }
        public string? CloseButtonText { get; set; }
        public string? Title { get => _titleBlock?.Text; set { if (_titleBlock != null) _titleBlock.Text = value; } }

        public BaseDialog()
        {
            this.BorderThickness = new Thickness(1);
            this.CornerRadius = new CornerRadius(12);
            this.BorderBrush = new SolidColorBrush(Color.FromArgb(60, 255, 255, 255));
            this.Background = new SolidColorBrush(Color.FromArgb(255, 40, 40, 40));
            this.Padding = new Thickness(0);

            _buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 8,
                Margin = new Thickness(20, 12, 20, 20)
            };

            _titleBlock = new TextBlock
            {
                FontWeight = FontWeight.Bold,
                FontSize = 20,
                Margin = new Thickness(24, 20, 24, 4),
                Text = Title
            };

            _contentContainer = new ContentControl
            {
                Margin = new Thickness(24, 8, 24, 4)
            };

            var rootLayout = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,*,Auto")
            };

            rootLayout.Children.Add(_titleBlock);
            Grid.SetRow(_titleBlock, 0);

            rootLayout.Children.Add(_contentContainer);
            Grid.SetRow(_contentContainer, 1);

            rootLayout.Children.Add(_buttonPanel);
            Grid.SetRow(_buttonPanel, 2);

            base.Content = rootLayout;

            this.KeyDown += BaseDialog_KeyDown;
        }

        private void BaseDialog_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Escape)
                Close(DialogResult.Cancel);
        }

        private void AdjustDialogSize()
        {
            if (base.Content is Grid rootLayout)
            {
                if (DialogService.topLevel != null)
                {
                    rootLayout.Bind(Layoutable.WidthProperty, new Binding("Bounds.Width")
                    {
                        Source = DialogService.topLevel,
                        Converter = new PercentageConverter(),
                        ConverterParameter = 0.45
                    });
                    rootLayout.MinWidth = 300;
                    rootLayout.MaxWidth = 650;
                }

                rootLayout.HorizontalAlignment = HorizontalAlignment.Center;
                rootLayout.VerticalAlignment = VerticalAlignment.Center;
            }
        }

        public new object? Content
        {
            get => _contentContainer.Content;
            set => _contentContainer.Content = value;
        }

        private void CreateButtons()
        {
            _buttonPanel.Children.Clear();
            if (!string.IsNullOrEmpty(SecondaryButtonText)) AddButton(SecondaryButtonText, DialogResult.Secondary);
            if (!string.IsNullOrEmpty(PrimaryButtonText)) AddButton(PrimaryButtonText, DialogResult.Primary, true);
            if (!string.IsNullOrEmpty(CloseButtonText)) AddButton(CloseButtonText, DialogResult.Cancel);
        }

        private void AddButton(string text, DialogResult result, bool isDefault = false)
        {
            var btn = new Button
            {
                Content = text,
                MinWidth = 90,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(16, 8)
            };
            btn.Click += (_, _) => Close(result);
            if (isDefault) btn.Classes.Add("accent");
            _buttonPanel.Children.Add(btn);
        }

        public async Task<DialogResult> ShowAsync(TopLevel? owner = null)
        {
            CreateButtons();
            AdjustDialogSize();

            var result = await base.ShowAsync();
            return result.GetValueOrDefault(DialogResult.Cancel);
        }

        public new void Close(DialogResult result)
        {
            var args = new BaseDialogClosingArgs { Result = result };
            Closing?.Invoke(this, args);

            if (args.Cancel) return;

            base.Close(result);
        }
    }
}