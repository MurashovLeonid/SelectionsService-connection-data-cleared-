using System;
using System.Collections.Generic;
using System.Text;

namespace Superbrands.Selection.Domain.Requests
{
    public class SizeInfo
    {
        public string Sku { get; set; }
        public int Count { get; set; }
        public double Bwp { get; private set; }

        public void SetBwp(double? bwp)
        {
            Bwp = bwp ?? 0;
        }
    }     
}
