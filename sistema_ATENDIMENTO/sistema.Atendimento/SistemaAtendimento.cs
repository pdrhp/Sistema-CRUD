using sistema.Modelos.Conta;
using sistema_ATENDIMENTO.sistema.Exceptions;
using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using sistema_ATENDIMENTO.sistema.Modelos.Conta;

namespace sistema_ATENDIMENTO.Sistema.Atendimento
{

    public class SistemaAtendimento
    {
        // CRUD Create Read Update Delete

        private string arquivoCSV = "contas.csv";

        // Lista de contas Pré-Cadastrada
        private List<ContaCorrente> _listaDeContas = new List<ContaCorrente>()
        {
            new ContaCorrente(95, "123456-X") { Saldo = 100, Titular = new Cliente{Cpf = "111111", Nome = "Jose"}} ,
            new ContaCorrente(95, "987654-X") { Saldo = 200, Titular = new Cliente{Cpf = "222222", Nome = "Claudio"} },
            new ContaCorrente(94, "123456-W") { Saldo = 60, Titular = new Cliente { Cpf = "3333333", Nome = "Tiago" } }
        };

        private void CarregarContasDoCSV()
        {
            if (File.Exists(arquivoCSV))
            {
                using (var reader = new StreamReader(arquivoCSV))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    _listaDeContas = csv.GetRecords<ContaCorrenteCSV>()
                        .Select(contaCSV => new ContaCorrente(contaCSV.NumeroAgencia, contaCSV.Conta)
                        {
                            Saldo = contaCSV.Saldo,
                            Titular = new Cliente
                            {
                                Nome = contaCSV.NomeTitular,
                                Cpf = contaCSV.CpfTitular,
                                Profissao = contaCSV.ProfissaoTitular
                            },
                            Nome_Agencia = contaCSV.NomeAgencia
                        }).ToList();
                }
            }
        }
        private void SalvarContasNoCSV()
        {
            using (var writer = new StreamWriter(arquivoCSV))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(_listaDeContas.Select(conta => new ContaCorrenteCSV
                {
                    NumeroAgencia = conta.Numero_agencia,
                    Conta = conta.Conta,
                    Saldo = conta.Saldo,
                    NomeTitular = conta.Titular.Nome,
                    NomeAgencia = conta.Nome_Agencia,
                    CpfTitular = conta.Titular.Cpf,
                    ProfissaoTitular = conta.Titular.Profissao
                }));
            }
        }

        //Metodo Inicial de escolha de opção
        public void AtendimentoCliente()
        {
            CarregarContasDoCSV();
            try
            {
                char opcao = '0';
                while (opcao != '7')
                {
                    Console.Clear();
                    Console.WriteLine("===============================");
                    Console.WriteLine("===       Atendimento       ===");
                    Console.WriteLine("===1 - Cadastrar Conta      ===");
                    Console.WriteLine("===2 - Listar Contas        ===");
                    Console.WriteLine("===3 - Remover Conta        ===");
                    Console.WriteLine("===4 - Ordenar Contas       ===");
                    Console.WriteLine("===5 - Pesquisar Conta      ===");
                    Console.WriteLine("===6 - Alterar uma Conta    ===");
                    Console.WriteLine("===7 - Sair do Sistema      ===");
                    Console.WriteLine("===============================");
                    Console.WriteLine("\n\n");
                    Console.Write("Digite a opção desejada: ");
                    try
                    {
                        opcao = Console.ReadLine()[0];
                    }
                    catch (Exception excecao)
                    {
                        throw new SistemaException(excecao.Message);
                    }
                    switch (opcao)
                    {
                        case '1':
                            CadastrarConta();
                            break;
                        case '2':
                            ListarContas();
                            break;
                        case '3':
                            RemoverContas();
                            break;
                        case '4':
                            OrdernarContas();
                            break;
                        case '5':
                            PesquisarContas();
                            break;
                        case '6':
                            AtualizarContas();
                            break;
                        case '7':
                            EncerrarAplicacao();
                            break;
                        default:
                            Console.WriteLine("Opcao não implementada.");
                            break;
                    }
                }
            }
            catch (SistemaException excecao)
            {
                Console.WriteLine($"{excecao.Message}");
            }

        }

        private void EncerrarAplicacao()
        {
            SalvarContasNoCSV();
            Console.WriteLine("... Encerrando a aplicação ...");
            Console.ReadKey();
        }


        //Metodo de Cadastro de contas
        private void CadastrarConta()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("===   CADASTRO DE CONTAS    ===");
            Console.WriteLine("===============================");
            Console.WriteLine("\n");
            Console.WriteLine("=== Informe dados da conta ===");

            Console.Write("Número da Agência: ");
            int numeroAgencia = int.Parse(Console.ReadLine());

            ContaCorrente conta = new ContaCorrente(numeroAgencia);

            Console.WriteLine($"Numero Da conta [NOVA] : {conta.Conta}");

            Console.Write("Informe o saldo inicial: ");
            conta.Saldo = double.Parse(Console.ReadLine());

            Console.Write("Infome nome do Titular: ");
            conta.Titular.Nome = Console.ReadLine();

            Console.Write("Infome CPF do Titular: ");
            conta.Titular.Cpf = Console.ReadLine();

            Console.Write("Infome Profissão do Titular: ");
            conta.Titular.Profissao = Console.ReadLine();

            _listaDeContas.Add(conta);


            Console.WriteLine("... Conta cadastrada com sucesso! ...");
            Console.ReadKey();
        }


        //Metodo de Atualização de contas e alteração de dados
        private void AtualizarContas()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("===    ATUALIZAR CONTAS     ===");
            Console.WriteLine("===============================");
            Console.WriteLine("\n");
            Console.WriteLine("Deseja pesquisar a conta para atualizar por (1) NUMERO DA CONTA ou (2)CPF TITULAR :");
            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    {
                        Console.Write("Informe o número da Conta: ");
                        string _numeroConta = Console.ReadLine();
                        string opcaoAtualizar = "";
                        ContaCorrente consultaConta = ConsultaPorNumeroConta(_numeroConta);
                        Console.WriteLine(consultaConta.ToString());
                        Console.WriteLine("Você deseja atualizar esta conta? (S/N)");
                        opcaoAtualizar = Console.ReadLine().ToLower();
                        if (opcaoAtualizar == "s")
                            AlterarDados(consultaConta);

                        break;
                    }
                case 2:
                    {
                        Console.Write("Informe o CPF da Conta: ");
                        string _cpf = Console.ReadLine();
                        string opcaoAtualizar = "";
                        ContaCorrente consultaCpf = ConsultaPorCPFTitular(_cpf);
                        Console.WriteLine(consultaCpf.ToString());
                        Console.WriteLine("Você deseja atualizar esta conta? (S/N)");
                        opcaoAtualizar = Console.ReadLine().ToLower();
                        if (opcaoAtualizar == "s")
                            AlterarDados(consultaCpf);

                        break;
                    }
                default:
                    Console.WriteLine("Opção não implementada.");
                    Console.ReadKey();
                    break;

            }
        }
        private void AlterarDados(ContaCorrente contaQueVaiSerAlterada)
        {

            Console.Write("Número da conta: ");
            contaQueVaiSerAlterada.Conta = Console.ReadLine();

            Console.Write("Número da Agência: ");
            contaQueVaiSerAlterada.Numero_agencia = int.Parse(Console.ReadLine());

            Console.Write("Infome nome do Titular: ");
            contaQueVaiSerAlterada.Titular.Nome = Console.ReadLine();

            Console.Write("Informe o novo Saldo: ");
            contaQueVaiSerAlterada.Saldo = double.Parse(Console.ReadLine());

            Console.Write("Infome CPF: ");
            contaQueVaiSerAlterada.Titular.Cpf = Console.ReadLine();

            Console.Write("Infome Profissão: ");
            contaQueVaiSerAlterada.Titular.Profissao = Console.ReadLine();
        }


        //Metodos de Consultas de contas registradas
        private void PesquisarContas()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("===    PESQUISAR CONTAS     ===");
            Console.WriteLine("===============================");
            Console.WriteLine("\n");
            Console.Write("Deseja pesquisar por (1) NUMERO DA CONTA ou (2)CPF TITULAR ou (3)NUMERO AGENCIA : ");
            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    {
                        Console.Write("Informe o número da Conta: ");
                        string _numeroConta = Console.ReadLine();
                        ContaCorrente consultaConta = ConsultaPorNumeroConta(_numeroConta);
                        Console.WriteLine(consultaConta.ToString());
                        Console.ReadKey();
                        break;
                    }
                case 2:
                    {
                        Console.Write("Informe o CPF do Titular: ");
                        string _cpf = Console.ReadLine();
                        ContaCorrente consultaCpf = ConsultaPorCPFTitular(_cpf);
                        Console.WriteLine(consultaCpf.ToString());
                        Console.ReadKey();
                        break;
                    }
                case 3:
                    {
                        Console.WriteLine("Informe o N da Agência: ");
                        int _numeroAgencia = int.Parse(Console.ReadLine());
                        var contasPorAgencia = ConsultaPorAgencia(_numeroAgencia);
                        ExibirListaDeContas(contasPorAgencia);
                        Console.ReadKey();
                        break;
                    }
                default:
                    Console.WriteLine("Opção não implementada.");
                    Console.ReadKey();
                    break;
            }
        }
        private void ExibirListaDeContas(List<ContaCorrente> listaDeContas)
        {
            if (listaDeContas == null)
            {
                Console.WriteLine("... A consulta não retornou dados");
            }
            else
            {
                foreach (var item in listaDeContas)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }
        private List<ContaCorrente> ConsultaPorAgencia(int numeroAgencia)
        {
            var consulta = (
                        from conta in _listaDeContas
                        where conta.Numero_agencia == numeroAgencia
                        select conta).ToList();
            return consulta;
        }
        private ContaCorrente ConsultaPorCPFTitular(string cpf)
        {
            //ContaCorrente conta = null;
            //for (int i = 0; i < _listaDeContas.Count; i++)
            //{
            //    if (_listaDeContas[i].Titular.Cpf.Equals(cpf))
            //    {
            //        conta = _listaDeContas[i];
            //    }
            //}
            //return conta;
            return _listaDeContas.Where(conta => conta.Titular.Cpf == cpf).FirstOrDefault();
        }
        private ContaCorrente ConsultaPorNumeroConta(string numeroConta)
        {
            //ContaCorrente conta = null;
            //for (int i = 0; i < _listaDeContas.Count; i++)
            //{
            //    if (_listaDeContas[i].Conta.Equals(numeroConta))
            //    {
            //        conta = _listaDeContas[i];
            //    }
            //}
            //return conta;

            //return _listaDeContas.Where(conta => conta.Conta == numeroConta).FirstOrDefault();

            return (from conta in _listaDeContas where conta.Conta == numeroConta select conta).FirstOrDefault();
        }


        //Metodo de ordenação de contas por Agencia
        private void OrdernarContas()
        {
            _listaDeContas.Sort();
            Console.WriteLine("... Lista de contas ordenadas ...");
            Console.ReadKey();
        }


        //Metodo de remoção de contas
        private void RemoverContas()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("===      REMOVER CONTAS     ===");
            Console.WriteLine("===============================");
            Console.WriteLine("\n");
            Console.Write("Informe o número da Conta: ");
            string numeroConta = Console.ReadLine();
            ContaCorrente conta = null;
            foreach (var item in _listaDeContas)
            {
                if (item.Conta.Equals(numeroConta))
                {
                    conta = item;
                }
            }
            if (conta != null)
            {
                _listaDeContas.Remove(conta);
                Console.WriteLine("... Conta removida da lista! ...");
            }
            else
            {
                Console.WriteLine(" ... Conta para remoção não encontrada ...");
            }
            Console.ReadKey();
        }


        //Metodo de listagem de contas existentes
        private void ListarContas()
        {
            Console.Clear();
            Console.WriteLine("===============================");
            Console.WriteLine("===     LISTA DE CONTAS     ===");
            Console.WriteLine("===============================");
            Console.WriteLine("\n");
            if (_listaDeContas.Count <= 0)
            {
                Console.WriteLine("...Não há contas cadastradas! ...");
                Console.ReadKey();
                return;
            }

            foreach (ContaCorrente item in _listaDeContas)
            {
                //Console.WriteLine("===  Dados da Conta  ===");
                //Console.WriteLine("Número da Conta : " + item.Conta);
                //Console.WriteLine("Saldo da Conta : " + item.Saldo);
                //Console.WriteLine("Titular da Conta: " + item.Titular.Nome);
                //Console.WriteLine("CPF do Titular  : " + item.Titular.Cpf);
                //Console.WriteLine("Profissão do Titular: " + item.Titular.Profissao);
                //Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Console.WriteLine(item.ToString());
                Console.ReadKey();
            }


        }

    }
}
