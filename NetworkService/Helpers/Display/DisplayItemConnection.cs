namespace NetworkService.Helpers.Display
{
    public class DisplayItemConnection : BindableBase
    {
        private DisplayItem item1;
        private DisplayItem item2;

        public DisplayItemConnection(DisplayItem item1, DisplayItem item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }
        public DisplayItem Item1
        {
            get { return item1; }
            set
            {
                if (item1 != value && item2 != value)
                {
                    item1 = value;
                    OnPropertyChanged(nameof(Item1));
                }
            }
        }
        public DisplayItem Item2
        {
            get { return item2; }
            set
            {
                if (item2 != value && item1 != value)
                {
                    item2 = value;
                    OnPropertyChanged(nameof(Item2));
                }
            }
        }
    }
}
