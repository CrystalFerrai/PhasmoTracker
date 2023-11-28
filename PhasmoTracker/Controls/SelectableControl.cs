// Copyright 2023 Crystal Ferrai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PhasmoTracker.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PhasmoTracker.Controls
{
	/// <summary>
	/// Custom toggle button used for selecting or rejecting options
	/// </summary>
	[TemplatePart(Name = PART_ContentArea, Type = typeof(ContentPresenter))]
	internal class SelectableControl : ContentControl
	{
		private const string PART_ContentArea = "PART_ContentArea";

		private readonly Pen mStrikethroughPen;
		private readonly Pen mStrikethroughUnavailablePen;
		private readonly TextDecorationCollection mStrikethroughDecoration;
		private readonly TextDecorationCollection mStrikethroughUnavailableDecoration;

		private ContentPresenter? mContentArea;
		private TextBlock? mContentText;

		public CornerRadius CornerRadius
		{
			get { return (CornerRadius)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}
		public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(SelectableControl),
			new PropertyMetadata(new CornerRadius(0.0)));

		public bool? IsSelected
		{
			get { return (bool?)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool?), typeof(SelectableControl),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((SelectableControl)d).IsSelectedChanged((bool?)e.OldValue, (bool?)e.NewValue)));

		public bool IsAvailable
		{
			get { return (bool)GetValue(IsAvailableProperty); }
			set { SetValue(IsAvailableProperty, value); }
		}
		public static readonly DependencyProperty IsAvailableProperty = DependencyProperty.Register(nameof(IsAvailable), typeof(bool), typeof(SelectableControl),
			new PropertyMetadata(true, (d, e) => ((SelectableControl)d).IsAvailableChanged((bool?)e.OldValue, (bool?)e.NewValue)));

		public bool IsHighlighted
		{
			get { return (bool)GetValue(IsHighlightedProperty); }
			set { SetValue(IsHighlightedProperty, value); }
		}
		public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register(nameof(IsHighlighted), typeof(bool), typeof(SelectableControl),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

		public bool IsReadOnly
		{
			get { return (bool)GetValue(IsReadOnlyProperty); }
			set { SetValue(IsReadOnlyProperty, value); }
		}
		public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(SelectableControl),
			new PropertyMetadata(false));

		public SelectionMode SelectionMode
		{
			get { return (SelectionMode)GetValue(SelectionModeProperty); }
			set { SetValue(SelectionModeProperty, value); }
		}
		public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register(nameof(SelectionMode), typeof(SelectionMode), typeof(SelectableControl),
			new PropertyMetadata(SelectionMode.All));

		public bool CanHighlight
		{
			get { return (bool)GetValue(CanHighlightProperty); }
			set { SetValue(CanHighlightProperty, value); }
		}
		public static readonly DependencyProperty CanHighlightProperty = DependencyProperty.Register(nameof(CanHighlight), typeof(bool), typeof(SelectableControl),
			new PropertyMetadata(false, (d, e) => ((SelectableControl)d).CanHighlightChanged((bool?)e.OldValue, (bool?)e.NewValue)));

		public Brush DefaultBrush
		{
			get { return (Brush)GetValue(DefaultBrushProperty); }
			set { SetValue(DefaultBrushProperty, value); }
		}
		public static readonly DependencyProperty DefaultBrushProperty = DependencyProperty.Register(nameof(DefaultBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		public Brush SelectedBrush
		{
			get { return (Brush)GetValue(SelectedBrushProperty); }
			set { SetValue(SelectedBrushProperty, value); }
		}
		public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(nameof(SelectedBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		public SelectableType SelectableType
		{
			get { return (SelectableType)GetValue(SelectableTypeProperty); }
			set { SetValue(SelectableTypeProperty, value); }
		}
		public static readonly DependencyProperty SelectableTypeProperty = DependencyProperty.Register(nameof(SelectableType), typeof(SelectableType), typeof(SelectableControl),
			new PropertyMetadata(SelectableType.WithCircle));

		public Brush RejectedBrush
		{
			get { return (Brush)GetValue(RejectedBrushProperty); }
			set { SetValue(RejectedBrushProperty, value); }
		}
		public static readonly DependencyProperty RejectedBrushProperty = DependencyProperty.Register(nameof(RejectedBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null, (d, e) => ((SelectableControl)d).RejectedBrushChanged((Brush)e.OldValue, (Brush)e.NewValue)));

		public bool RejectedHasStrikethrough
		{
			get { return (bool)GetValue(RejectedHasStrikethroughProperty); }
			set { SetValue(RejectedHasStrikethroughProperty, value); }
		}
		public static readonly DependencyProperty RejectedHasStrikethroughProperty = DependencyProperty.Register(nameof(RejectedHasStrikethrough), typeof(bool), typeof(SelectableControl),
			new PropertyMetadata(true));

		public Brush UnavailableBrush
		{
			get { return (Brush)GetValue(UnavailableBrushProperty); }
			set { SetValue(UnavailableBrushProperty, value); }
		}
		public static readonly DependencyProperty UnavailableBrushProperty = DependencyProperty.Register(nameof(UnavailableBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null, (d, e) => ((SelectableControl)d).UnavailableBrushChanged((Brush)e.OldValue, (Brush)e.NewValue)));

		public Brush HoverBrush
		{
			get { return (Brush)GetValue(HoverBrushProperty); }
			set { SetValue(HoverBrushProperty, value); }
		}
		public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.Register(nameof(HoverBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		public Brush HoverSelectedBrush
		{
			get { return (Brush)GetValue(HoverSelectedBrushProperty); }
			set { SetValue(HoverSelectedBrushProperty, value); }
		}
		public static readonly DependencyProperty HoverSelectedBrushProperty = DependencyProperty.Register(nameof(HoverSelectedBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		public Brush HoverRejectedBrush
		{
			get { return (Brush)GetValue(HoverRejectedBrushProperty); }
			set { SetValue(HoverRejectedBrushProperty, value); }
		}
		public static readonly DependencyProperty HoverRejectedBrushProperty = DependencyProperty.Register(nameof(HoverRejectedBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		public Brush HoverUnavailableBrush
		{
			get { return (Brush)GetValue(HoverUnavailableBrushProperty); }
			set { SetValue(HoverUnavailableBrushProperty, value); }
		}
		public static readonly DependencyProperty HoverUnavailableBrushProperty = DependencyProperty.Register(nameof(HoverUnavailableBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		public Brush HighlightBrush
		{
			get { return (Brush)GetValue(HighlightBrushProperty); }
			set { SetValue(HighlightBrushProperty, value); }
		}
		public static readonly DependencyProperty HighlightBrushProperty = DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(SelectableControl),
			new PropertyMetadata(null));

		static SelectableControl()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectableControl), new FrameworkPropertyMetadata(typeof(SelectableControl)));
		}

		public SelectableControl()
		{
			mStrikethroughPen = new Pen(RejectedBrush, 3.0);
			mStrikethroughUnavailablePen = new Pen(UnavailableBrush, 3.0);
			mStrikethroughDecoration = new TextDecorationCollection()
			{
				new TextDecoration(TextDecorationLocation.Strikethrough, mStrikethroughPen, 0.0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended)
			};
			mStrikethroughUnavailableDecoration = new TextDecorationCollection()
			{
				new TextDecoration(TextDecorationLocation.Strikethrough, mStrikethroughUnavailablePen, 0.0, TextDecorationUnit.FontRecommended, TextDecorationUnit.FontRecommended)
			};

			Keyboard.AddPreviewKeyDownHandler(Application.Current.MainWindow, OnGlobalKeyDown);
			Keyboard.AddPreviewKeyUpHandler(Application.Current.MainWindow, OnGlobalKeyUp);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (mContentText is not null)
			{
				mContentText.TextDecorations = null;
				mContentText = null;
			}

			mContentArea = GetTemplateChild(PART_ContentArea) as ContentPresenter;
			if (mContentArea is null)
			{
				return;
			}

			mContentArea.ApplyTemplate();
			mContentText = VisualTreeUtility.FindDescendant<TextBlock>(mContentArea);
			UpdateText();
        }

		private void IsSelectedChanged(bool? oldValue, bool? newValue)
		{
			UpdateText();
		}

		private void IsAvailableChanged(bool? oldValue, bool? newValue)
		{
			UpdateText();
		}

		private void IsThreeStateChanged(bool? oldValue, bool? newValue)
		{
			UpdateText();
		}

		private void CanHighlightChanged(bool? oldValue, bool? newValue)
		{
			if (newValue == false && IsHighlighted)
			{
				IsHighlighted = false;
			}
		}

		private void RejectedBrushChanged(Brush oldValue, Brush newValue)
		{
			mStrikethroughPen.Brush = newValue;
			UpdateText();
		}

		private void UnavailableBrushChanged(Brush oldValue, Brush newValue)
		{
			mStrikethroughUnavailablePen.Brush = newValue;
			UpdateText();
		}

		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
		{
			return new PointHitTestResult(this, hitTestParameters.HitPoint);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			base.OnMouseDown(e);

			if (IsReadOnly) return;

			switch (e.ChangedButton)
			{
				case MouseButton.Left:
					if (IsSelected.HasValue && IsSelected.Value)
					{
						if (SelectionMode != SelectionMode.SelectOnly)
						{
							SetValue(IsSelectedProperty, null);
						}
					}
					else
					{
						SetValue(IsSelectedProperty, true);
					}
					e.Handled = true;
					break;
				case MouseButton.Right:
					if (SelectionMode == SelectionMode.SelectOnly) break;

					if (IsSelected.HasValue && !IsSelected.Value)
					{
						SetValue(IsSelectedProperty, null);
						e.Handled = true;
					}
					else if (SelectionMode != SelectionMode.NoUnselect)
					{
						SetValue(IsSelectedProperty, false);
						e.Handled = true;
					}
					break;
			}
		}

		protected override void OnMouseEnter(MouseEventArgs e)
		{
			base.OnMouseEnter(e);

            if (CanHighlight && !IsReadOnly)
            {
				SetValue(IsHighlightedProperty, Keyboard.Modifiers == ModifierKeys.Shift);
            }
        }

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			base.OnMouseLeave(e);

			if (CanHighlight)
			{
				SetValue(IsHighlightedProperty, false);
			}
		}

		private void OnGlobalKeyDown(object? sender, KeyEventArgs e)
		{
			if ((e.Key == Key.LeftShift || e.Key == Key.RightShift) && CanHighlight && IsMouseOver && !IsReadOnly)
			{
				SetValue(IsHighlightedProperty, true);
			}
		}

		private void OnGlobalKeyUp(object? sender, KeyEventArgs e)
		{
			if ((e.Key == Key.LeftShift || e.Key == Key.RightShift) && CanHighlight)
			{
				SetValue(IsHighlightedProperty, false);
			}
		}

		private void UpdateText()
		{
			if (mContentText is null)
			{
				return;
			}

			if (IsSelected.HasValue && !IsSelected.Value && RejectedHasStrikethrough)
			{
				mContentText.TextDecorations = IsAvailable ? mStrikethroughDecoration : mStrikethroughUnavailableDecoration;
			}
			else
			{
				mContentText.TextDecorations = null;
			}
		}
	}

	internal enum SelectionMode
	{
		All,
		NoUnselect,
		SelectOnly
	}

	internal enum SelectableType
	{
		WithoutCircle,
		WithCircle
	}
}
