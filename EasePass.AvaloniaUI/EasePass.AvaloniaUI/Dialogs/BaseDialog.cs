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
        public string? Title { get => _titleBlock?.Text; set => _titleBlock?.Text = value; }

        public BaseDialog()
        {
            this.BorderThickness = new Thickness(2);
            this.CornerRadius = new CornerRadius(10);
            //todo proper colors from theme
            this.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            this.Background = new SolidColorBrush(Color.FromArgb(255, 35, 35, 35));

            _buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 10,
                Margin = new Thickness(20)
            };

            _titleBlock = new TextBlock
            {
                FontWeight = FontWeight.Bold,
                FontSize = 18,
                Margin = new Thickness(20, 20, 20, 0),
                Text = Title
            };

            _contentContainer = new ContentControl();

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
            if(e.Key == Avalonia.Input.Key.Escape)
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
                        ConverterParameter = 0.4 //size in percent 
                    });
                    rootLayout.MinWidth = 200;
                    rootLayout.MaxWidth = 600; 
                    rootLayout.ClearValue(Layoutable.MaxWidthProperty); 
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
            var btn = new Button { Content = text, MinWidth = 80 };
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