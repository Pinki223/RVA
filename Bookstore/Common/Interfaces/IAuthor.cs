using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Model;

namespace Common
{
    public interface IAuthor
    {
        List<Author> GetAuthors();
        Author CreateAuthor(string firstName, string lastName, string shortDesc, string token);
    }
}
