using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Model;

namespace Common
{
    public interface IUser
    {
        string LogIn(string username, string password);
        void LogOut(string token);
        Member GetMemberInfo(string token);
        bool EditMemberInfo(string firstName, string lastName, string token);
    }
}
