using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Research
{
    interface ITableRepository
    {
        void InsertMany(int count);
        void DeleteAll();
        void NextSearch(); 
    }
}
