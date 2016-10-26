using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Browser.Common.Cotrols
{
    public class MyComboBox : ComboBox
    {
        public event Action<KeyEventArgs> OnAfterTextBoxKeyDown = delegate { };
        public event Action OnAfterSelectionChanged = delegate { };
        public TextBox textbox;

        int prevIndex = 0;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            object element = this.GetTemplateChild("PART_EditableTextBox");
            if (element != null)
            {
                textbox = element as TextBox;
                textbox.PreviewKeyDown += Textbox_PreviewKeyDown;
                textbox.PreviewMouseDown += Textbox_PreviewMouseDown;
                textbox.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
        }

        private void Textbox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            prevIndex = textbox.CaretIndex;
        }

        private void Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) return;
            if (e.Key != Key.Enter)
            {
                prevIndex = textbox.CaretIndex;
                if (!string.IsNullOrEmpty(textbox.SelectedText) && !string.IsNullOrWhiteSpace(textbox.SelectedText))
                {
                    if (e.Key != Key.Enter && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Down && e.Key != Key.Up)
                    {
                        textbox.Text = textbox.Text.Replace(textbox.SelectedText, "");
                    }
                    else
                        textbox.CaretIndex = prevIndex = textbox.SelectedText.Length;
                }
            }

            if (e.Key == Key.Delete || e.Key == Key.Back) return;

            OnAfterTextBoxKeyDown(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter) return;

            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Down || e.Key == Key.Up)
                OnAfterTextBoxKeyDown(e);
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            textbox.CaretIndex = prevIndex;
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            try
            {
                if (this.SelectedValue == null)
                    return;
                textbox.Text = this.SelectedValue.ToString();

                OnAfterSelectionChanged();
            }
            catch
            {
            }
            textbox.CaretIndex = prevIndex;
        }
    }
}
