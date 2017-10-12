using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Apo_Chan.Custom
{
    public class FloatingActionButtonView : View
    {
        public static readonly BindableProperty ImageNameProperty =
            BindableProperty.Create("ImageName", typeof(string), typeof(FloatingActionButtonView), string.Empty);
        public string ImageName
        {
            get { return (string)GetValue(ImageNameProperty); }
            set { SetValue(ImageNameProperty, value); }
        }

        public static readonly BindableProperty ColorNormalProperty =
            BindableProperty.Create("ColorNormal", typeof(Color), typeof(FloatingActionButtonView), Color.White);
        public Color ColorNormal
        {
            get { return (Color)GetValue(ColorNormalProperty); }
            set { SetValue(ColorNormalProperty, value); }
        }

        public static readonly BindableProperty ColorPressedProperty =
            BindableProperty.Create("ColorPressed", typeof(Color), typeof(FloatingActionButtonView), Color.White);
        public Color ColorPressed
        {
            get { return (Color)GetValue(ColorPressedProperty); }
            set { SetValue(ColorPressedProperty, value); }
        }

        public static readonly BindableProperty ColorRippleProperty =
            BindableProperty.Create("ColorRipple", typeof(Color), typeof(FloatingActionButtonView), Color.White);
        public Color ColorRipple
        {
            get { return (Color)GetValue(ColorRippleProperty); }
            set { SetValue(ColorRippleProperty, value); }
        }

        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create("Size", typeof(FloatingActionButtonSize), typeof(FloatingActionButtonView), FloatingActionButtonSize.Normal);
        public FloatingActionButtonSize Size
        {
            get { return (FloatingActionButtonSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly BindableProperty HasShadowProperty =
            BindableProperty.Create("HasShadow", typeof(bool), typeof(FloatingActionButtonView), true);
        public bool HasShadow
        {
            get { return (bool)GetValue(HasShadowProperty); }
            set { SetValue(HasShadowProperty, value); }
        }

        public delegate void ShowHideDelegate(bool animate = true);
        public delegate void AttachToListViewDelegate(ListView listView);

        public ShowHideDelegate Show { get; set; }
        public ShowHideDelegate Hide { get; set; }
        public Action<object, EventArgs> Clicked { get; set; }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command", typeof(ICommand), typeof(FloatingActionButtonView), null, propertyChanged: HandleCommandChanged);
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create("CommandParameter", typeof(object), typeof(FloatingActionButtonView), null);
        public object CommandParameter
        {
            get { return (object)this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        private void InternalHandleCommand(ICommand oldValue, ICommand newValue)
        {
            // TOOD: attach to CanExecuteChanged
        }

        private static void HandleCommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as FloatingActionButtonView).InternalHandleCommand(oldValue as ICommand, newValue as ICommand);
        }

        public virtual void SendClicked()
        {
            var param = this.CommandParameter;

            if (this.Command != null && this.Command.CanExecute(param))
            {
                this.Command.Execute(param);
            }

            if (this.Clicked != null)
            {
                this.Clicked(this, EventArgs.Empty);
            }
        }
    }

    public enum FloatingActionButtonSize
    {
        Normal,
        Mini
    }
}
