using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sistema_ATENDIMENTO.sistema.Exceptions
{
    public class SistemaException : Exception
    {
        public SistemaException()
        {
        }

        public SistemaException(string message) : base("Aconteceu uma Exceção => " + message)
        {
        }

        public SistemaException(string message, Exception inner) : base(message, inner)
        {
        }
    }
    
}
