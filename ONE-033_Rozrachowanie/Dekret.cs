using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ONE_033_Rozrachowanie
{
    public class Dekret
    {
        public int DT_GIDNumer { get; set; }
        public int DT_GIDLp { get; set; }
        public int DT_DC { get; set; } 
        public string DT_Kwota { get; set; } 
        public string KAZ_NumerDokumentu { get; set; }

        public Dekret(string DT_GIDNumer, string DT_GIDLp, string DT_DC, string DT_Kwota, string KAZ_NumerDokumentu)
        {
            this.DT_GIDNumer = Convert.ToInt32(DT_GIDNumer);
            this.DT_GIDLp = Convert.ToInt32(DT_GIDLp);
            this.DT_DC = Convert.ToInt32(DT_DC);
            this.DT_Kwota = DT_Kwota;
            this.KAZ_NumerDokumentu = KAZ_NumerDokumentu;
        }
    }
}
