using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Misc
{
    class RevertableCommand : Command
    {
        private Action revert;

        public RevertableCommand(Action executeFunc, Action revertFunc) : base(executeFunc)
        {
            revert = revertFunc;
        }

        public void Revert()
        {
            if (revert != null)
                revert();
        }
    }
}
