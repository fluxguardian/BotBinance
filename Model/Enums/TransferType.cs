using System.ComponentModel;

namespace Model.Enums
{
    public enum TransferType
    {
        [Description("1")]
        ToMarginAccount,

        [Description("2")]
        ToMainAccount
    }
}
