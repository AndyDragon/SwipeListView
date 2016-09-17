using Demo.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Universal.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundOpacity = 1;
                StatusBar.GetForCurrentView().ForegroundColor = Colors.White;
                StatusBar.GetForCurrentView().BackgroundColor = Color.FromArgb(0xFF, 0x00, 0x7a, 0xff);
            }
        }

        private void ItemSwipedLeft(EmailObject email)
        {
            //await new MessageDialog(email.Body, "Swiped Item Left").ShowAsync();
            email.Unread = !email.Unread;
        }

        private void ItemSwipedRight(EmailObject email)
        {
            //await new MessageDialog(email.Body, "Swiped Item Right").ShowAsync();
            (Resources["Emails"] as EmailCollection).Remove(email);
        }

        private async void ItemClicked(EmailObject email)
        {
            await new MessageDialog(email.Body, "Clicked Item").ShowAsync();
        }

        private void SwipeListView_ItemSwipe(object sender, ItemSwipeEventArgs e)
        {
            var item = e.SwipedItem as EmailObject;
            if (item != null)
            {
                var state = SwipeListViewItemState.Collapsed;
                var listViewItem = this.List.ContainerFromItem(item) as SwipeListViewItem;
                if (listViewItem != null)
                {
                    state = listViewItem.ItemState;
                }

                if (state != SwipeListViewItemState.PersistedLeft && state != SwipeListViewItemState.PersistedRight)
                {
                    if (listViewItem != null)
                    {
                        listViewItem.ResetSwipe();
                    }

                    if (e.Direction == SwipeListDirection.Left)
                    {
                        this.ItemSwipedLeft(item);
                    }
                    else
                    {
                        this.ItemSwipedRight(item);
                    }
                }
            }
        }

        private void SwipeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as EmailObject;
            var listViewItem = this.List.ContainerFromItem(item) as SwipeListViewItem;
            if (listViewItem != null)
            {
                if (listViewItem.ItemState == SwipeListViewItemState.PersistedLeft)
                {
                    listViewItem.ResetSwipe();
                }
                else if (listViewItem.ItemState == SwipeListViewItemState.PersistedRight)
                {
                    listViewItem.ResetSwipe();
                }
                else
                {
                    listViewItem.ResetSwipe();
                    this.ItemClicked(item);
                }
            }
            else
            {
                this.ItemClicked(item);
            }
        }

        private void SwipeListView_ItemClickLeft(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var item = button.DataContext as EmailObject;
            var listViewItem = this.List.ContainerFromItem(item) as SwipeListViewItem;
            if (listViewItem != null)
            {
                listViewItem.ResetSwipe();
                this.ItemSwipedLeft(item);
            }
        }

        private void SwipeListView_ItemClickRight(object sender, RoutedEventArgs e)
        {
            var button = e.OriginalSource as Button;
            var item = button.DataContext as EmailObject;
            var listViewItem = this.List.ContainerFromItem(item) as SwipeListViewItem;
            if (listViewItem != null)
            {
                listViewItem.ResetSwipe();
                this.ItemSwipedRight(item);
            }
        }
    }
}
