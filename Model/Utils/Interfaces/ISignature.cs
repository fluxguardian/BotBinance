namespace Model.Utils.Interfaces
{
    interface ISignature
    {
        string GenerateSignature(string secretKey, string parametres);
    }
}
