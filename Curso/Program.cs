using CursoEfCore.Domain;
using CursoEfCore.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace CursoEFCore
{

    class Program
    {
        static void Main(string[] args)
        {
            /*using var db = new Data.ApplicationContext();
            db.Database.Migrate();
            var existe = db.Database.GetPendingMigrations().Any();
            if(existe)
            {
                // 
            }*/


            bool resp = true;

            while (resp)
            {
                Console.WriteLine("O que gostaria de testar?");
                Console.WriteLine("* Inserir dados, digite 1");
                Console.WriteLine("* Inserir dados em massa, digite 2");
                Console.WriteLine("* Consultar dados, digite 3");
                Console.WriteLine("* Cadastrar um pedido, digite 4");
                Console.WriteLine("* Consultar carregamento adiantado, digite 5");
                Console.WriteLine("* Atualizar dados, digite 6");
                Console.WriteLine("* Remover registro, digite 7");
                var resposta = Console.ReadLine();

                ValidarInputOpcoes(resposta);

                switch (resposta)
                {
                    case "1":
                        InserirDados();
                        break;
                    case "2":
                        InserirDadosEmMassa();
                        break;
                    case "3":
                        ConsultarDados();
                        break;
                    case "4":
                        CadastrarPedido();
                        break;
                    case "5":
                        ConsultarPedidoCarregamentoAdiantado();
                        break;
                    case "6":
                        AtualizarDados();
                        break;
                    case "7":
                        RemoverRegistro();
                        break;
                }

                Console.WriteLine("Gostaria de continaur (S) - Sim e (N) - Não");
                var resposta2 = Console.ReadLine().ToLower();

                if (resposta2 != "s")
                    resp = false;
            }
        }

        #region Métodos Auxiliares
        private static void ValidarInputOpcoes(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Você deve informar uma posicao");
                return;
            }
            if (input is not "1" and not "2" and not "3" and not "4" and not "5" and not "6" and not "7")
            {
                Console.WriteLine("Opção invalida");
                return;
            }
        }
        #endregion Métodos Auxiliares

        #region RemoverRegistro
        private static void RemoverRegistro()
        {
            using var db = new CursoEfCore.Data.ApplicationContext();

            var cliente = db.Clientes.Find(5);
            if (cliente == null)
            {
                Console.WriteLine("Nenhum cliente encontrado");
                return;
            }
            //db.Clientes.Remove(cliente);
            //db.Remove(cliente);
            db.Entry(cliente).State = EntityState.Deleted;

            var registro = db.SaveChanges();

            Console.WriteLine($"Foram deletado(s) {registro} registro(s)");
        }
        #endregion RemoverRegistro

        #region AtualizarDados
        private static void AtualizarDados()
        {
            using var db = new CursoEfCore.Data.ApplicationContext();
            //var cliente = db.Clientes.Find(1);

            var cliente = new Cliente
            {
                Id = 1
            };

            var clienteDesconectado = new
            {
                Nome = "Cliente Desconectado Passo 3",
                Telefone = "7966669999"
            };

            db.Attach(cliente);
            db.Entry(cliente).CurrentValues.SetValues(clienteDesconectado);

            //db.Clientes.Update(cliente);
            var registro = db.SaveChanges();

            Console.WriteLine($"Foram atualizado(s) {registro} registro(s)");
        }
        #endregion AtualizarDados

        #region ConsultarPdidoCarregamentoAdiantado
        private static void ConsultarPedidoCarregamentoAdiantado()
        {
            using var db = new CursoEfCore.Data.ApplicationContext();
            var pedidos = db
                .Pedidos
                .Include(p => p.Itens)
                    .ThenInclude(p => p.Produto)
                .ToList();

            Console.WriteLine($"Foi consultado {pedidos.Count} pedidos");
        }
        #endregion ConsultarPdidoCarregamentoAdiantado

        #region CadastrarPedido
        private static void CadastrarPedido()
        {
            using var db = new CursoEfCore.Data.ApplicationContext();

            var cliente = db.Clientes.FirstOrDefault();
            var produto = db.Produtos.FirstOrDefault();

            if (cliente == null)
            {
                Console.WriteLine("Nenhum cliente cadastrado");
                return;
            }

            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                IniciadoEm = DateTime.Now,
                FinalizadoEm = DateTime.Now,
                Observacao = "Pedido Teste",
                Status = StatusPedido.Analise,
                TipoFrete = TipoFrete.SemFrete,
                Itens = new List<PedidoItem>
                 {
                     new PedidoItem
                     {
                         ProdutoId = produto.Id,
                         Desconto = 0,
                         Quantidade = 1,
                         Valor = 10,
                     }
                 }
            };

            db.Pedidos.Add(pedido);

            var registro = db.SaveChanges();

            Console.WriteLine($"Total Registro(s) cadastrados: {registro} registro(s)");
        }
        #endregion CadastrarPedido

        #region ConsultarDados
        private static void ConsultarDados()
        {
            using var db = new CursoEfCore.Data.ApplicationContext();
            //var consultaPorSintaxe = (from c in db.Clientes where c.Id>0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                .Where(p => p.Id > 0)
                .OrderBy(p => p.Id)
                .ToList();

            if (consultaPorMetodo.Count <= 0)
            {
                Console.WriteLine("Nenhum dado cadastrado");
                return;
            }

            foreach (var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id} {cliente.Nome}");
                //db.Clientes.Find(cliente.Id);
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }
        #endregion ConsultarDados

        #region InserirDadosEmMassa
        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Rafael Almeida",
                CEP = "99999000",
                Cidade = "Itabaiana",
                Estado = "SE",
                Telefone = "99000001111",
            };

            var listaClientes = new[]
            {
                new Cliente
                {
                    Nome = "Teste 1",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "99000001115",
                },
                new Cliente
                {
                    Nome = "Teste 2",
                    CEP = "99999000",
                    Cidade = "Itabaiana",
                    Estado = "SE",
                    Telefone = "99000001116",
                },
            };


            using var db = new CursoEfCore.Data.ApplicationContext();
            //db.AddRange(produto, cliente);
            db.Set<Cliente>().AddRange(listaClientes);
            //db.Clientes.AddRange(listaClientes);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s) adicionados: {registros} registro(s)");
        }
        #endregion InserirDadosEmMassa

        #region InserirDados
        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.MercadoriaParaRevenda,
                Ativo = true
            };

            using var db = new CursoEfCore.Data.ApplicationContext();
            db.Produtos.Add(produto);
            db.Set<Produto>().Add(produto);
            db.Entry(produto).State = EntityState.Added;
            db.Add(produto);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s) adicionados: {registros} registro(s)");
        }
        #endregion InserirDados
    }
}
