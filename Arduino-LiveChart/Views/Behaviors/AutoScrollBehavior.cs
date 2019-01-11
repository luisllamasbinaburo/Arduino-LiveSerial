using Arduino_LiveChart.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using static Arduino_LiveChart.ViewModels.MainViewModel;

namespace Arduino_LiveChart.Behaviors
{
    public class ScrollIntoViewBehavior : Behavior<DataGrid>
    {
        private DataGrid Grid
        {
            get
            {
                return AssociatedObject;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Grid.Loaded += Grid_Loaded; ;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            var ItemSource = Grid.ItemsSource as ObservableCollection<SerialMessage>;
            ItemSource.CollectionChanged += ItemSource_CollectionChanged;
        }

        private void ItemSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Grid.Items.Count > 0)
            Grid.ScrollIntoView(Grid.Items[Grid.Items.Count - 1]);
        }
    }
}
