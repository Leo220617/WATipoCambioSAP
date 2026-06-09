using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WATickets.Models
{
    public class RespuestaTCHacienda
    {
        public TipoCambio venta { get; set; }
        public TipoCambio compra { get; set; }
    }

    public class TipoCambio
    {
        public DateTime fecha { get; set; }
        public decimal valor { get; set; }
    }
}