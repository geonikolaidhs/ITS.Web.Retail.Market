using DevExpress.Xpo;

namespace ITS.Retail.WebClient.Controllers
{
    public interface IBaseController
    {
        UnitOfWork XpoSession { get; }
    }
}