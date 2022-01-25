
namespace InfoPC
{
    public class CheckBoxAdapterItem : BaseViewModel
    {
        public bool IsChecked { get; set; }
        public string NameAdapter { get; set; }
        public CheckBoxAdapterItem(bool ch, string text)
        {
            IsChecked = ch;
            NameAdapter = text;
        }
    }
}
