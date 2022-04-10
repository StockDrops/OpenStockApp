using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OpenStockApp.Core.Maui.Views
{
    public class LazyLoadingCollectionView : CollectionView
    {
        public static readonly BindableProperty LoadMoreCommandProperty =
            BindableProperty.Create(nameof(LoadMoreCommand), typeof(ICommand), typeof(LazyLoadingCollectionView), default(ICommand)); //  <LazyLoadingListView, ICommand>(bp => bp.LoadMoreCommand, default(ICommand));

        public ICommand LoadMoreCommand
        {
            get { return (ICommand)GetValue(LoadMoreCommandProperty); }
            set { SetValue(LoadMoreCommandProperty, value); }
        }

        public LazyLoadingCollectionView()
        {
            //RegisterLayzyLoading();
        }

        //public LazyLoadingCollectionView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        //{
        //    RegisterLayzyLoading();
        //}

        //void RegisterLayzyLoading()
        //{
        //      += InfiniteListView_Appearing;
        //    Scrolled += InfiniteListView_Scrolled;
        //}

        //int called = 0;
        //int lastItemIndex = 0;
        //void InfiniteListView_Appearing(object? sender, ItemVisibilityEventArgs e)
        //{
            
        //    //// if last item is in view: load more items
        //    if (ItemsSource is ICollection items)
        //    {
        //        if(items.Count == e.ItemIndex + 1)
        //            ScrollTo(e.Item, ScrollToPosition.Center, true);
        //    }
        //    lastItemIndex = e.ItemIndex;
        //    //lastItem = e.Item;
        //    //called++;
        //}

        double maxScroll = 0;
        
        int lastPageNumber = 1;
        void InfiniteListView_Scrolled(object? sender, ScrolledEventArgs e) // https://social.msdn.microsoft.com/Forums/en-US/9d30819c-7994-4f23-9004-af28a1b05afd/how-to-notify-when-hit-bottom-of-a-listview?forum=xamarinforms
        {
            if(ItemsSource is ICollection items && items.Count == 0)
            {
                maxScroll = 0;
                lastPageNumber = 1;
            }
            if(maxScroll <= e.ScrollY)
            {
                maxScroll = e.ScrollY;
            }
            else
            {
                if(e.ScrollY < Height && LoadMoreCommand.CanExecute(lastPageNumber))
                {
                    lastPageNumber++;
                    LoadMoreCommand.Execute(lastPageNumber);

                }
            }

            //if(maxScroll < e.ScrollY)
            //{
            //    //Console.WriteLine(this.Height);
            //    var pageNumber = (int)Math.Ceiling(e.ScrollY / Height);
            //    if (pageNumber > lastPageNumber)
            //    {
            //        LoadMoreCommand.Execute(pageNumber);
            //        lastPageNumber = pageNumber;
            //    }
            //    maxScroll = e.ScrollY;
            //}
            

           
        }
    }
}
