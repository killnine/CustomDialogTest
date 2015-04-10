using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using CustomDialogTest.Views;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace CustomDialogTest.Helpers
{
    public static class ViewLocator
    {
        /// <summary>
        /// The function to get the type of a view for a given view model type.
        /// </summary>
        /// <remarks>
        /// By default it takes the full name of the view model type, calls <see cref="GetViewTypeNameFromViewModelTypeName"/>
        /// and gets a type with the resulting name from the IoC container.
        /// E.g. if <see cref="GetViewTypeNameFromViewModelTypeName"/> is not changed, for type <em>MyApp.ViewModels.MyViewModel</em>
        /// it will return the type <em>MyApp.Views.MyView</em>
        /// </remarks>
        public static Func<Type, Type> GetViewTypeFromViewModelType;

        /// <summary>
        /// This function returns for the full name of a view model type the corresponding name of the view type.
        /// </summary>
        /// <remarks>
        /// By default, this function simply replaces "ViewModel" with "View", e.g. for "MyApp.ViewModels.MyViewModel" it returns "MyApp.Views.MyView"
        /// </remarks>
        public static Func<string, string> GetViewTypeNameFromViewModelTypeName;

        static ViewLocator()
        {
            SimpleIoc.Default.Register<SelectionDialogView>();

            GetViewTypeNameFromViewModelTypeName = viewModeltypeName => viewModeltypeName.Replace("ViewModel", "View");
            GetViewTypeFromViewModelType = type =>
            {
                var viewModelTypeName = type.FullName;
                var viewTypeName = GetViewTypeNameFromViewModelTypeName(viewModelTypeName);
                var viewType = type.Assembly.GetType(viewTypeName);
                return viewType;
            };
        }

        /// <summary>
        /// Gets the view for the passed view model.
        /// </summary>
        /// <param name="lifetimeScope">The optional scope of the IoC container to get the registered view from.</param>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns>The view matching the view model.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the view cannot be found in the IoC containmer.</exception>
        /// <remarks>
        /// <para>
        /// To get the correct view type of the given <typeparamref name="TViewModel"/>, this method will
        /// call <see cref="GetViewTypeFromViewModelType"/>.
        /// </para>
        /// <para>
        /// If <typeparamref name="TViewModel"/> implements <see cref="IOnLoadedHandler"/> or <see cref="IOnClosingHandler"/>,
        /// this method will register the view's corresponding events and call <see cref="IOnLoadedHandler.OnLoadedAsync"/>
        /// and <see cref="IOnClosingHandler.OnClosing"/> respectively when those events are raised.
        /// </para>
        /// <para>
        /// Don't call <strong>InitializeComponent()</strong> in your view's constructor yourself! If your view contains
        /// a method called InitializeComponent, this method will call it automatically via reflection.
        /// This allows the user of the library to remove the code-behind of her/his XAML files.
        /// </para>
        /// </remarks>
        public static object GetViewForViewModel<TViewModel>()
        {
            var viewModel = ServiceLocator.Current.GetInstance((typeof(TViewModel)));
            return GetViewForViewModel(viewModel);
        }

        /// <summary>
        /// Gets the view for the passed view model.
        /// </summary>
        /// <param name="viewModel">The view model for which a view should be returned.</param>
        /// <returns>The view matching the view model.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the view cannot be found in the IoC containmer.</exception>
        /// <remarks>
        /// <para>
        /// To get the correct view type of the passed <paramref name="viewModel"/>, this method will
        /// call <see cref="GetViewTypeFromViewModelType"/>.
        /// </para>
        /// <para>
        /// If the <paramref name="viewModel"/> implements <see cref="IOnLoadedHandler"/> or <see cref="IOnClosingHandler"/>,
        /// this method will register the view's corresponding events and call <see cref="IOnLoadedHandler.OnLoadedAsync"/>
        /// and <see cref="IOnClosingHandler.OnClosing"/> respectively when those events are raised.
        /// </para>
        /// <para>
        /// Don't call <strong>InitializeComponent()</strong> in your view's constructor yourself! If your view contains
        /// a method called InitializeComponent, this method will call it automatically via reflection.
        /// This allows the user of the library to remove the code-behind of her/his XAML files.
        /// </para>
        /// </remarks>
        public static object GetViewForViewModel(object viewModel)
        {
            var viewType = GetViewTypeFromViewModelType(viewModel.GetType());
            if (viewType == null)
            {
                throw new InvalidOperationException("No View found for ViewModel of type " + viewModel.GetType());
            }

            var view = ServiceLocator.Current.GetInstance(viewType);
            
            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                AttachHandler(frameworkElement, viewModel);
                frameworkElement.DataContext = viewModel;
            }

            InitializeComponent(view);

            return view;
        }
        public interface IOnLoadedHandler
        {
            /// <summary>
            /// This method is called when the corresponding view's <see cref="Window.Closing"/> or
            /// <see cref="FrameworkElement.Unloaded"/> event was raised.
            /// </summary>
            /// <returns></returns>
            Task OnLoadedAsync();
        }

        public interface IOnClosingHandler
        {
            /// <summary>
            /// This method is called when the corresponding view's <see cref="FrameworkElement.Loaded"/> event was raised.
            /// </summary>
            /// <returns></returns>
            void OnClosing();
        }

        private static void AttachHandler(FrameworkElement view, object viewModel)
        {
            var onLoadedHandler = viewModel as IOnLoadedHandler;
            if (onLoadedHandler != null)
            {
                RoutedEventHandler handler = null;
                handler = async (sender, args) =>
                {
                    view.Loaded -= handler;
                    await onLoadedHandler.OnLoadedAsync();
                };
                view.Loaded += handler;
            }

            var onClosingHandler = viewModel as IOnClosingHandler;
            if (onClosingHandler != null)
            {
                var window = view as Window;
                if (window != null)
                {
                    CancelEventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        window.Closing -= handler;
                        onClosingHandler.OnClosing();
                    };
                    window.Closing += handler;
                }
                else
                {
                    RoutedEventHandler handler = null;
                    handler = (sender, args) =>
                    {
                        view.Unloaded -= handler;
                        onClosingHandler.OnClosing();
                    };
                    view.Unloaded += handler;
                }
            }
        }

        private static void InitializeComponent(object element)
        {
            var method = element.GetType().GetMethod("InitializeComponent", BindingFlags.Instance | BindingFlags.Public);
            if (method == null)
            {
                return;
            }
            method.Invoke(element, null);
        }
    }
}