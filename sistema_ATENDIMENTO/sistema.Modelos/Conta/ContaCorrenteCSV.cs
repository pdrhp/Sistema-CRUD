using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistema_ATENDIMENTO.sistema.Modelos.Conta
{
    public class ContaCorrenteCSV
    {
        public int NumeroAgencia { get; set; }
        public string Conta { get; set; }
        public double Saldo { get; set; }
        public string NomeTitular { get; set; }
        public string NomeAgencia { get; set; }
        public string CpfTitular { get; set; }
        public string ProfissaoTitular { get; set; }
    }
}
