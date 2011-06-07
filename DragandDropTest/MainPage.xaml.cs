using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace DragandDropTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        DispatcherTimer dt;
        superRect dragItem;
        superRect droppedOn;
        List<Item> items;

        public MainPage()
        {

            InitializeComponent();
            items = new List<Item>();
            items.Add(new Item { name = "one", sortOrder = 1 , c = Colors.Purple});
            items.Add(new Item { name = "two", sortOrder = 2, c = Colors.Red });
            items.Add(new Item { name = "three", sortOrder = 3 , c = Colors.Magenta });
            items.Add(new Item { name = "four", sortOrder = 4, c = Colors.LightGray });
            items.Add(new Item { name = "five", sortOrder = 5, c = Colors.Green });
            items.Add(new Item { name = "six", sortOrder = 6, c = Colors.Yellow  });
            items.Add(new Item { name = "seven", sortOrder = 7, c = Colors.Gray });
            items.Add(new Item { name = "eight", sortOrder = 8, c = Colors.Blue });
            
            PopulateCanvas();
        }

        private void PopulateCanvas()
        {
            var minheight = 0;
            List<Item> ordered = items.OrderBy(s => s.sortOrder).ToList();

            foreach (var i in ordered)
            {
                superRect r = new superRect();
                r.Tag = i.name;
                r.RectTextLocal = i.name;
                r.RectColourLocal = new SolidColorBrush(i.c);
                r.IsHitTestVisible = true;
                r.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(Rectangle_ManipulationStarted);
                r.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(Rectangle_ManipulationDelta);
                r.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(Rectangle_ManipulationCompleted);
                r.DataContext = i;
               
                int left = i.sortOrder % 2 == 0 ? 300 : 0;
                int top =  minheight ;
                i.left = left;
                i.top = top;


                Canvas.SetLeft(r, i.left);
                Canvas.SetTop(r, i.top);

                cvs.Children.Add(r);
                
                if (i.sortOrder % 2 == 0)
                    minheight += 250;
 
            }
            
        }


        private void Rectangle_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {

            superRect dragItem = sender as superRect;
            Canvas.SetZIndex(dragItem, 100);
            e.ManipulationContainer = (superRect)sender;
            e.Handled = true;
        }

    
        private void Rectangle_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            superRect rect = sender as superRect;
            Item iRect = (Item) rect.DataContext;

            Canvas.SetTop(rect, Canvas.GetTop(rect) +  e.DeltaManipulation.Translation.Y);
            Canvas.SetLeft(rect, Canvas.GetLeft(rect) + e.DeltaManipulation.Translation.X);

            var cLeft = Canvas.GetLeft(rect);
            var cTop = Canvas.GetTop(rect);

            List<superRect> rects = cvs.Children
                                       .Where(et => et.GetType() == typeof(superRect))
                                       .Select(et => (superRect)et)
                                       .ToList();


            List<superRect> itemsNear2 = rects
                                       .Where(et => Canvas.GetLeft(et) > (cLeft - 150))
                                       .Where(et => Canvas.GetLeft(et) < (cLeft + 150))
                                       .Where(et => Canvas.GetTop(et) > (cTop - 150))
                                       .Where(et => Canvas.GetTop(et) < (cTop + 150))
                                       .Where(et=> ((Item)et.DataContext).name != iRect.name)
                                       .ToList();


            if (itemsNear2.Any())
            {
                foreach (var r in rects)
                {
                    if (itemsNear2.Contains(r))
                    {
                        ScaleTransform st = new ScaleTransform();
                        st.ScaleX = 1.2;
                        st.ScaleY = 1.2;
                        r.RenderTransform = st;
                    }
                    else
                    {
                        r.RenderTransform = null;
                    }
                }
            }
          



            e.Handled = true;


        }

        

        private void Rectangle_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            superRect rect = sender as superRect;
            Item iRect = (Item)rect.DataContext;
            var dropLeft = Canvas.GetLeft(rect);
            var dropTop = Canvas.GetTop(rect);

            List<superRect> rects = cvs.Children
                                      .Where(et => et.GetType() == typeof(superRect))
                                      .Select(et => (superRect)et)
                                      .ToList();


            superRect itemEnd = cvs.Children
                                      .Where(et => et.GetType() == typeof(superRect))
                                      .Select(et => (superRect)et)
                                      .Where(et => Canvas.GetLeft(et) > (dropLeft - 150))
                                      .Where(et => Canvas.GetLeft(et) < (dropLeft + 150))
                                      .Where(et => Canvas.GetTop(et) > (dropTop - 150))
                                      .Where(et => Canvas.GetTop(et) < (dropTop + 150))
                                      .Where(et => ((Item)et.DataContext).name != iRect.name)
                                      .FirstOrDefault();

            if (itemEnd != null)
            {
                Item iEnd = (Item)itemEnd.DataContext;
                Item iMover = (Item)rect.DataContext;

                reOrder(iMover, iEnd);
            }
        }

        private void reOrder(Item iMoverSO, Item iEndSO)
        {
            var iFrom = iMoverSO.sortOrder;
            var iTo = iEndSO.sortOrder;

            List<superRect> rects = new List<superRect>();
            
            if (iFrom > iTo)
            {
                rects   = (from et in cvs.Children
                                     let so = ((Item)((superRect)et).DataContext).sortOrder
                                     where et.GetType() == typeof(superRect)
                                     where so<=iFrom
                                     where so>=iTo
                                     select (superRect)et
                                    ).ToList();
            }
            if (iFrom < iTo)
            {
                rects = (from et in cvs.Children
                         let so = ((Item)((superRect)et).DataContext).sortOrder
                         where et.GetType() == typeof(superRect)
                         where so >= iFrom
                         where so <= iTo
                         select (superRect)et
                                    ).ToList();
            }

            
            foreach (var r in rects)
            {
                Item dc = (Item) r.DataContext;

                if (iFrom > iTo)
                    dc.sortOrder += 1;
                else
                    dc.sortOrder -= 1;
            }

           
           iMoverSO.sortOrder = iTo;

            //iEndSO.sortOrder = iFrom;
            //iMoverSO.sortOrder = iTo;

            var removeCanvas = cvs.Children.Where(element => element is superRect).ToList();
            removeCanvas.ForEach(element => cvs.Children.Remove(element));

            PopulateCanvas();
            

        }
    }

    public class Item
    {
        public int sortOrder { get; set; }
        public string name { get; set; }
        public int top { get; set; }
        public int left{ get; set; }
        public Color c { get; set;  }
    }
}