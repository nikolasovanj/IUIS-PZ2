using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace NetworkService.Helpers
{
    public class NotificationHelper
    {
        public static NotificationContent CreateSuccessToastNotification(string content)
        {
            var notificationContent = new NotificationContent
            {
                Title = "Success",
                Message = content,
                Type = NotificationType.Success,
                TrimType = NotificationTextTrimType.AttachIfMoreRows, // Will show attach button on message
                RowsCount = 2, // Will show 2 rows and trim after
                //LeftButtonAction = () => SomeAction(), // Action on left button click, button will not show if it is null 
                //RightButtonAction = () => SomeAction(), // Action on right button click, button will not show if it is null
                //LeftButtonContent, // Left button content (string or what you want)
                //RightButtonContent, // Right button content (string or what you want)
                CloseOnClick = true, // Set to true if you want to close message when left mouse button click on message (base = true)

                Background = (Brush)Application.Current.Resources["PrimaryColor"],
                Foreground = (Brush)Application.Current.Resources["SecondaryColor"],
                
                // FontAwesome5 by Codinion NuGet package is required for this to work
                //Icon = new SvgAwesome()
                //{
                //    Icon = EFontAwesomeIcon.Regular_Star,
                //    Height = 25,
                //    Foreground = new SolidColorBrush(Colors.Yellow)
                //},

                //Image = new NotificationImage()
                //{
                //    Source = new BitmapImage(new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Test image.png")));,
                //    Position = ImagePosition.Top
                //}
            };

            return notificationContent;
        }
    }
}
