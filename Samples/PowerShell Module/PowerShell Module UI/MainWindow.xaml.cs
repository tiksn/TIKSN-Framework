using PoshCode.Controls;
using System;
using System.Management.Automation.Runspaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace PowerShell_Module_UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var importCommand = new Command("Import-Module");
			importCommand.Parameters.Add("Name", @".\PowerShell_Module.dll");

			var processes = await poshConsole.InvokeAsync(new[] { importCommand });

			var getCommand = new Command("Get-Command");
			getCommand.Parameters.Add("Module", "PowerShell_Module");

			var selectCommand = new Command("Select-Object");
			selectCommand.Parameters.Add("Property", "Name");

			await poshConsole.InvokeAsync(new[] { getCommand, selectCommand });
		}

		private void PromptForChoice(object sender, PromptForChoiceEventArgs e)
		{
			// Ensure this is invoked synchronously on the UI thread
			if (!Dispatcher.CheckAccess())
			{
				Dispatcher.Invoke(DispatcherPriority.Render, (Action)(() =>
				{
					PromptForChoice(sender, e);
				}));
				return;
			}

			// Disable the console ...
			poshConsole.CommandBox.IsEnabled = false;

			#region PromptForChoiceWindowCouldBeInXaml
			// Create a window with a stack panel inside
			var content = new Grid { Margin = new Thickness(6) };

			content.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
			content.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

			var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
			content.Children.Add(buttons);
			buttons.SetValue(Grid.RowProperty, 1);

			var dialog = new Window
			{
				SizeToContent = SizeToContent.WidthAndHeight,
				HorizontalContentAlignment = HorizontalAlignment.Stretch,
				VerticalContentAlignment = VerticalAlignment.Stretch,
				Title = e.Caption,
				Content = content,
				MinHeight = 100,
				MinWidth = 300,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Owner = this,
				Tag = -1 // We must initialize the tag
			};

			// Make buttons for each choice
			var index = 0;
			foreach (var choice in e.Choices)
			{
				var item = new Button
				{
					Content = choice.Label.Replace('&', '_'),
					ToolTip = choice.HelpMessage,
					IsDefault = e.SelectedIndex == index,
					Padding = new Thickness(10, 4, 10, 4),
					Margin = new Thickness(4),
					Tag = index // set the button Tag to it's index
				};

				// when the button is clicked, set the window tag to the button's index, and close the window.
				item.Click += (o, args) =>
				{
					dialog.Tag = (args.OriginalSource as FrameworkElement)?.Tag;
					Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
					{
						dialog.Close();
					}));
				};
				buttons.Children.Add(item);
				index++;
			}

			// Handle the Caption and Message
			if (string.IsNullOrWhiteSpace(e.Caption))
			{
				e.Caption = e.Message;
			}
			if (!string.IsNullOrWhiteSpace(e.Message))
			{
				content.Children.Insert(0, new TextBlock
				{
					Text = e.Message,
					FontSize = 16,
					FontWeight = FontWeight.FromOpenTypeWeight(700),
					Effect = new System.Windows.Media.Effects.DropShadowEffect
					{
						Color = Colors.CadetBlue,
						Direction = 0,
						ShadowDepth = 0,
						BlurRadius = 5
					}
				});
			}
			#endregion CouldBeInXaml

			dialog.ShowDialog();
			e.SelectedIndex = (int)dialog.Tag;

			// Reenable the console
			poshConsole.CommandBox.IsEnabled = true;

		}
	}
}
