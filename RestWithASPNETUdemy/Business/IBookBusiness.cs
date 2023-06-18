using RestWithASPNETUdemy.Model;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace RestWithASPNETUdemy.Business
{
    public interface IBookBusiness
    {
        Book Create(Book book);
        Book FindById(long id);
        List<Book> FindAll();
        Book Update(Book book);
        void Delete(long id);
    }
}
